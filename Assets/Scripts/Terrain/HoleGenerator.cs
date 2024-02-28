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

        private MeshCollider MeshCollider => meshCollider != null ? meshCollider : GetComponent<MeshCollider>();

        private void Awake()
        {
            meshCollider = GetComponent<MeshCollider>();
        }

        private Tile LastTile => tileInstances.Count > 0 ? tileInstances.Last() : null;
        private Vector3Int LastCell => usedCells.Count > 0 ? usedCells.Last() : Vector3Int.back;

        #region Generate
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

            var combine = new CombineInstance[settings.TileCount];
            for (int tileIndex = 0; tileIndex < settings.TileCount; tileIndex++)
            {
                Tile[] tilePrefabOptions = GetTileOptionsFor(settings, tileIndex);
                var randomIndex = rng.Next(tilePrefabOptions.Length);

                var tile = SpawnTile(tilePrefabOptions[randomIndex], rng);
                combine[tileIndex].transform = tile.transform.localToWorldMatrix;
                combine[tileIndex].mesh = tile.GetComponent<MeshFilter>().sharedMesh;

                if (Application.isPlaying && spawnInterval > 0f) yield return new WaitForSeconds(spawnInterval);
            }

            var combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine);
            MeshCollider.sharedMesh = combinedMesh;

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
        #endregion

        public void Clear()
        {
            Action<UnityEngine.Object> contextDestroy = Application.isPlaying ? Destroy : DestroyImmediate;
            var tiles = transform.Cast<Transform>().ToArray();
            foreach (var tile in tiles) contextDestroy(tile.gameObject);
            tileInstances.Clear();
            usedCells.Clear();
            MeshCollider.sharedMesh = null;
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
                var startCell = LocalCellToCell(LastCell, LastTile, direction);
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

        private Vector3Int LocalCellToCell(Vector3Int baseCell, Tile baseTile, Vector3Int localCell) => LocalCellToCell(baseCell, baseTile.transform.localRotation, localCell);
        private Vector3Int LocalCellToCell(Vector3Int baseCell, Quaternion baseRotation, Vector3Int localCell) => baseCell + Vector3Int.RoundToInt(baseRotation * localCell);
        private Vector3 CellToPosition(Vector3Int cell) => transform.position + transform.rotation * Vector3.Scale(Tile.SCALE, cell);

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