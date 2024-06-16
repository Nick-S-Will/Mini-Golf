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
        [SerializeField] private Hole settings;
        [SerializeField][Min(0f)] private float spawnInterval;
        [Space]
        [SerializeField] private Gradient cellGizmoColorGradient;
        [SerializeField] private Color boundsGizmoColor = Color.white;
        [SerializeField] private bool showUsedCells, showBounds;
        [Space]
        public UnityEvent<HoleTile> OnGenerate;

        private readonly List<Tile> tileInstances = new();
        private readonly List<Vector3Int> usedCells = new();
        private MeshCollider meshCollider;
        private Coroutine generationRoutine;

        private MeshCollider MeshCollider => meshCollider ? meshCollider : GetComponent<MeshCollider>();
        private Tile LastTile => tileInstances.Count > 0 ? tileInstances.Last() : null;
        private Vector3Int LastCell => usedCells.Count > 0 ? usedCells.Last() : Vector3Int.back;

        public Hole HoleData => settings;
        public HoleTile CurrentHoleTile { get; private set; }
        public Bounds HoleBounds => MeshCollider.bounds;

        public void Generate() => Generate(settings);
        public void Generate(Hole settings)
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
        private IEnumerator GenerationRoutine(Hole settings)
        {
            System.Random rng = new(settings.Seed);

            for (int tileIndex = 0; tileIndex < settings.TileCount; tileIndex++)
            {
                Tile[] tilePrefabOptions = GetTileOptionsFor(settings, tileIndex);
                var randomIndex = rng.Next(tilePrefabOptions.Length);
                var tile = SpawnTile(tilePrefabOptions[randomIndex], rng);

                if (tile is HoleTile holeTile) CurrentHoleTile = holeTile;

                if (Application.isPlaying && spawnInterval > 0f) yield return new WaitForSeconds(spawnInterval);
            }

            MeshCollider.sharedMesh = CalculateHoleMesh();
            OnGenerate.Invoke(CurrentHoleTile);

            generationRoutine = null;
        }

        public void Clear()
        {
            Action<UnityEngine.Object> contextDestroy = Application.isPlaying ? Destroy : DestroyImmediate;
            var children = transform.Cast<Transform>().ToArray();
            foreach (var child in children)
            {
                var tile = child.GetComponent<Tile>();
                if (tile) contextDestroy(tile.gameObject);
            }
            tileInstances.Clear();
            usedCells.Clear();
            CurrentHoleTile = null;
            MeshCollider.sharedMesh = null;
        }

        private Tile[] GetTileOptionsFor(Hole settings, int index)
        {
            if (index < 0 || index >= settings.TileCount) Debug.LogError($"Index '{index}' out of range [0, {settings.TileCount - 1}]");

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

        private Mesh CalculateHoleMesh()
        {
            var combines = new CombineInstance[tileInstances.Count];
            for (int tileIndex = 0; tileIndex < tileInstances.Count; tileIndex++)
            {
                combines[tileIndex].transform = tileInstances[tileIndex].transform.localToWorldMatrix;
                combines[tileIndex].mesh = tileInstances[tileIndex].CollisionMesh;
            }

            var combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combines, true);
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
            if (cellGizmoColorGradient == null)
            {
                Debug.LogWarning($"{nameof(cellGizmoColorGradient)} not set");
                return;
            }

            if (showUsedCells && usedCells.Count > 1)
            {
                for (int i = 0; i < usedCells.Count; i++)
                {
                    Gizmos.color = cellGizmoColorGradient.Evaluate(i / (usedCells.Count - 1f));
                    Gizmos.DrawWireCube(CellToPosition(usedCells[i]), Tile.SCALE);
                }
            }

            if (showBounds && MeshCollider.sharedMesh)
            {
                var bounds = MeshCollider.sharedMesh.bounds;
                Gizmos.color = boundsGizmoColor;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }
}