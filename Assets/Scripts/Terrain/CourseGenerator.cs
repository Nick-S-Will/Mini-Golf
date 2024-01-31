using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Terrain
{
    [RequireComponent(typeof(MeshCollider))]
    public class CourseGenerator : MonoBehaviour
    {
        [Header("Tiles")]
        [SerializeField] private CourseTile[] startTilePrefabs;
        [SerializeField] private CourseTile[] courseTilePrefabs;
        [SerializeField] private HoleTile[] holeTilePrefabs;
        [Space]
        [SerializeField] private GenerationSettings settings;
        [SerializeField][Min(0f)] private float spawnInterval;
        [Space]
        [SerializeField] private Gradient gizmoColorGradient;
        [SerializeField] private bool showUsedCells;
        [Space]
        public UnityEvent OnGenerate;

        private List<CourseTile> tileInstances = new();
        private List<Vector3Int> usedCells = new();
        private Coroutine generationRoutine;

        private CourseTile LastTile => tileInstances.Count > 0 ? tileInstances.Last() : null;
        private Vector3Int LastCell => usedCells.Count > 0 ? usedCells.Last() : Vector3Int.back;

        public HoleTile HoleTile => generationRoutine == null ? (HoleTile)LastTile : null;

        #region Generate
        public void Generate() => Generate(settings);
        public void Generate(GenerationSettings settings)
        {
            if (startTilePrefabs.Length == 0 || courseTilePrefabs.Length == 0 || holeTilePrefabs.Length == 0)
            {
                Debug.LogError($"{nameof(startTilePrefabs)}, {nameof(courseTilePrefabs)}, or {nameof(holeTilePrefabs)} is empty");
                return;
            }

            if (generationRoutine != null) StopCoroutine(generationRoutine);
            Clear();

            generationRoutine = StartCoroutine(GenerationRoutine(settings));
        }
        private IEnumerator GenerationRoutine(GenerationSettings settings)
        {
            System.Random rng = new(settings.Seed);

            for (int tileIndex = 0; tileIndex < settings.CourseLength; tileIndex++)
            {
                CourseTile[] tilePrefabs;
                if (tileIndex == 0) tilePrefabs = startTilePrefabs;
                else if (tileIndex == settings.CourseLength - 1) tilePrefabs = holeTilePrefabs;
                else tilePrefabs = courseTilePrefabs;
                var randomIndex = rng.Next(tilePrefabs.Length);

                SpawnTile(tilePrefabs[randomIndex], rng);

                if (Application.isPlaying && spawnInterval > 0f) yield return new WaitForSeconds(spawnInterval);
            }

            generationRoutine = null;
            OnGenerate.Invoke();
        }
        #endregion

        public void Clear()
        {
            Action<UnityEngine.Object> contextDestroy = Application.isPlaying ? Destroy : DestroyImmediate;
            var tiles = transform.Cast<Transform>().ToArray();
            foreach (var tile in tiles) contextDestroy(tile.gameObject);
            tileInstances.Clear();
            usedCells.Clear();
        }

        private void SpawnTile(CourseTile tilePrefab, System.Random rng)
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
        }

        private Vector3Int GetCellFor(CourseTile tilePrefab, System.Random rng)
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

        private CourseTile ContextInstantiate(CourseTile original, Vector3 position, Quaternion rotation, Transform parent)
        {
            Func<UnityEngine.Object, Transform, UnityEngine.Object> instantiate = Instantiate;
#if UNITY_EDITOR
            if (!Application.isPlaying) instantiate = PrefabUtility.InstantiatePrefab;
#endif

            var newTile = (CourseTile)instantiate(original, parent);
            newTile.transform.SetPositionAndRotation(position, rotation);

            return newTile;
        }

        private Vector3Int LocalCellToCell(Vector3Int baseCell, CourseTile baseTile, Vector3Int localCell) => LocalCellToCell(baseCell, baseTile.transform.localRotation, localCell);
        private Vector3Int LocalCellToCell(Vector3Int baseCell, Quaternion baseRotation, Vector3Int localCell) => baseCell + Vector3Int.RoundToInt(baseRotation * localCell);
        private Vector3 CellToPosition(Vector3Int cell) => transform.position + transform.rotation * Vector3.Scale(CourseTile.SCALE, cell);

        private void OnDrawGizmosSelected()
        {
            if (gizmoColorGradient == null)
            {
                Debug.LogWarning($"{gizmoColorGradient} not set");
                return;
            }
            if (!showUsedCells || generationRoutine != null) return;

            var offsetToCenter = CourseTile.SCALE.y / 2f * Vector3.up;
            for (int i = 0; i < usedCells.Count; i++)
            {
                Gizmos.color = gizmoColorGradient.Evaluate(i / (usedCells.Count - 1f));
                Gizmos.DrawWireCube(CellToPosition(usedCells[i]) + offsetToCenter, CourseTile.SCALE);
            }
        }
    }
}