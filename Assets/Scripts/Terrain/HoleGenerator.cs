using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

using MiniGolf.Terrain.Data;

namespace MiniGolf.Terrain
{
    [RequireComponent(typeof(MeshCollider))]
    public class HoleGenerator : MonoBehaviour
    {
        [Header("Tiles")]
        [SerializeField] private Tile[] startTilePrefabs;
        [SerializeField] private Tile[] tilePrefabs;
        [SerializeField] private HoleTile[] holeTilePrefabs;
        [Space]
        [SerializeField] private HoleData settings;
        [SerializeField][Min(0f)] private float spawnInterval;
        [Space]
        [SerializeField] private Gradient gizmoColorGradient;
        [SerializeField] private bool showUsedCells;
        [Space]
        public UnityEvent<HoleTile> OnGenerate;

        private readonly List<Tile> tileInstances = new();
        private readonly List<Vector3Int> usedCells = new();
        private MeshCollider meshCollider;
        private Coroutine generationRoutine;

        public HoleData HoleData => settings;

        private MeshCollider MeshCollider => meshCollider ? meshCollider : GetComponent<MeshCollider>();
        private Tile LastTile => tileInstances.Count > 0 ? tileInstances.Last() : null;
        private Vector3Int LastCell => usedCells.Count > 0 ? usedCells.Last() : Vector3Int.back;

        private void Awake()
        {
            meshCollider = GetComponent<MeshCollider>();
        }

        public void Clear()
        {
            Action<UnityEngine.Object> contextDestroy = Application.isPlaying ? Destroy : DestroyImmediate;
            var tiles = transform.Cast<Transform>().ToArray();
            foreach (var tile in tiles) contextDestroy(tile.gameObject);
            tileInstances.Clear();
            usedCells.Clear();
            MeshCollider.sharedMesh = null;
        }

        public void Generate() => Generate(settings);
        public void Generate(HoleData settings)
        {
            if (startTilePrefabs.Length == 0 || tilePrefabs.Length == 0 || holeTilePrefabs.Length == 0)
            {
                Debug.LogError($"{nameof(startTilePrefabs)}, {nameof(tilePrefabs)}, or {nameof(holeTilePrefabs)} is empty");
                return;
            }

            if (generationRoutine != null) StopCoroutine(generationRoutine);
            Clear();

            generationRoutine = StartCoroutine(GenerationRoutine(settings));
        }
        private IEnumerator GenerationRoutine(HoleData settings)
        {
            System.Random rng = new(settings.Seed);

            for (int tileIndex = 0; tileIndex < settings.TileCount; tileIndex++)
            {
                Tile[] tilePrefabOptions = GetTileOptionsFor(settings, tileIndex);
                var randomIndex = rng.Next(tilePrefabOptions.Length);
                var tile = SpawnTile(tilePrefabOptions[randomIndex], rng);

                if (Application.isPlaying && spawnInterval > 0f) yield return new WaitForSeconds(spawnInterval);
            }

            MeshCollider.sharedMesh = CalculateHoleMesh();

            generationRoutine = null;
            OnGenerate.Invoke((HoleTile)LastTile);
        }

        private Tile[] GetTileOptionsFor(HoleData settings, int index)
        {
            if (index < 0 || index >= settings.TileCount) throw new ArgumentOutOfRangeException($"Index '{index}' out of range [0, {settings.TileCount - 1}]");

            if (index == 0) return startTilePrefabs;
            else if (index == settings.TileCount - 1) return holeTilePrefabs;
            else return tilePrefabs;
        }

        private Tile SpawnTile(Tile tilePrefab, System.Random rng)
        {
            var newCell = GetCellFor(tilePrefab, rng);
            var rotation = transform.rotation * Quaternion.LookRotation(Vector3.ProjectOnPlane(newCell - LastCell, Vector3.up));
            var newTile = ContextInstantiate(tilePrefab, CellToPosition(newCell), rotation, transform);
            tileInstances.Add(newTile);

            foreach (var localCell in newTile.LocalCells)
            {
                var cell = LocalCellToCell(newCell, rotation, localCell);
                usedCells.Add(cell);
            }

            return newTile;
        }

        private Vector3Int GetCellFor(Tile tilePrefab, System.Random rng)
        {
            if (usedCells.Count == 0) return Vector3Int.zero;

            var directions = LastTile.AvailableDirections;
            int n = directions.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (directions[n], directions[k]) = (directions[k], directions[n]);
            }

            foreach (var direction in directions)
            {
                var startCell = LocalCellToCell(LastCell, LastTile.transform.localRotation, direction);
                var rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(startCell - LastCell, Vector3.up));
                var availableCellCount = 0;
                foreach (var cell in tilePrefab.LocalCells)
                {
                    var endCell = LocalCellToCell(startCell, rotation, cell);
                    if (!usedCells.Contains(endCell)) availableCellCount++;
                    else break;
                }

                if (availableCellCount == tilePrefab.LocalCells.Length) return startCell;
            }

            throw new Exception($"No cell found from {LastCell} for {tilePrefab.name}");
        }

        private Tile ContextInstantiate(Tile original, Vector3 position, Quaternion rotation, Transform parent)
        {
            Func<UnityEngine.Object, Transform, UnityEngine.Object> instantiate = Instantiate;
#if UNITY_EDITOR
            if (!Application.isPlaying) instantiate = PrefabUtility.InstantiatePrefab;
#endif

            var newTile = (Tile)instantiate(original, parent);
            newTile.transform.SetPositionAndRotation(position, rotation);

            return newTile;
        }

        private Mesh CalculateHoleMesh(float threshold = 0.01f)
        {
            var combines = new CombineInstance[tileInstances.Count];
            for (int tileIndex = 0; tileIndex < tileInstances.Count; tileIndex++)
            {
                combines[tileIndex].transform = tileInstances[tileIndex].transform.localToWorldMatrix;
                combines[tileIndex].mesh = tileInstances[tileIndex].GetComponent<MeshFilter>().sharedMesh;
            }

            var combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combines, true);
            combinedMesh.RecalculateNormals();

            Vector3[] vertices = combinedMesh.vertices, normals = combinedMesh.normals;
            List<Vector3> newVertices = new(), newNormals = new();
            Dictionary<int, int> triangleMap = new();
            for (int i = 0; i < vertices.Length; i++)
            {
                var canAdd = true;
                for (int j = 0; j < vertices.Length; j++)
                {
                    if (i == j) continue;
                    if (Vector3.Distance(vertices[i], vertices[j]) > threshold) continue;
                    if ((normals[i] + normals[j]).magnitude > threshold) continue;

                    canAdd = false;
                    break;
                }
                if (!canAdd) continue;

                triangleMap.Add(i, newVertices.Count);
                newVertices.Add(vertices[i]);
                newNormals.Add(normals[i]);
            }

            int[] triangles = combinedMesh.triangles;
            List<int> newTriangles = new();
            for (int i = 0; i < triangles.Length; i += 3)
            {
                if (!triangleMap.ContainsKey(triangles[i])) continue;
                if (!triangleMap.ContainsKey(triangles[i + 1])) continue;
                if (!triangleMap.ContainsKey(triangles[i + 2])) continue;

                for (int j = 0; j < 3; j++) newTriangles.Add(triangleMap[triangles[i + j]]);
            }

            //print($"Before: {vertices.Length}, After: {newVertices.Count}. Removed: {vertices.Length - newVertices.Count}");
            combinedMesh.triangles = newTriangles.ToArray();
            combinedMesh.vertices = newVertices.ToArray();
            combinedMesh.normals = newNormals.ToArray();

            return combinedMesh;
        }

        private Vector3Int LocalCellToCell(Vector3Int baseCell, Quaternion baseRotation, Vector3Int localCell)
        {
            return baseCell + Vector3Int.RoundToInt(baseRotation * localCell);
        }
        private Vector3 CellToPosition(Vector3Int cell)
        {
            return transform.position + transform.rotation * Vector3.Scale(Tile.SCALE, cell);
        }

        private void OnDrawGizmosSelected()
        {
            if (gizmoColorGradient == null)
            {
                Debug.LogWarning($"{gizmoColorGradient} not set");
                return;
            }
            if (!showUsedCells || generationRoutine != null) return;

            for (int i = 0; i < usedCells.Count; i++)
            {
                Gizmos.color = gizmoColorGradient.Evaluate(i / (usedCells.Count - 1f));
                Gizmos.DrawWireCube(CellToPosition(usedCells[i]), Tile.SCALE);
            }
        }
    }
}