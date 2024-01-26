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
        [SerializeField] private int generationSeed;
        [SerializeField][Min(2f)] private int courseLength = 3;
        [SerializeField][Min(0f)] private float tileGenerationInterval = 1f;
        [Space]
        [SerializeField] private Gradient gizmoColorGradient;
        [SerializeField] private bool showUsedCells;
        [Space]
        [SerializeField] private UnityEvent OnGenerate;

        private List<CourseTile> tileInstances = new();
        private List<Vector3Int> usedCells = new();
        private Coroutine generationRoutine;

        private CourseTile LastTile => tileInstances.Count > 0 ? tileInstances.Last() : null;
        private Vector3Int LastCell => usedCells.Count > 0 ? usedCells.Last() : Vector3Int.back;

        public void Generate()
        {
            if (startTilePrefabs.Length == 0 || courseTilePrefabs.Length == 0 || holeTilePrefabs.Length == 0)
            {
                Debug.LogError($"{nameof(startTilePrefabs)}, {nameof(courseTilePrefabs)}, or {nameof(holeTilePrefabs)} is empty");
                return;
            }
            if (generationRoutine != null)
            {
                Debug.Log("Already generating...");
                return;
            }

            Clear();
            generationRoutine = StartCoroutine(GenerationRoutine());
        }
        private IEnumerator GenerationRoutine()
        {
            System.Random rng = new(generationSeed);

            for (int tileIndex = 0; tileIndex < courseLength; tileIndex++)
            {
                CourseTile[] tilePrefabs;
                if (tileIndex == 0) tilePrefabs = startTilePrefabs;
                else if (tileIndex == courseLength - 1) tilePrefabs = holeTilePrefabs;
                else tilePrefabs = courseTilePrefabs;
                var randomIndex = rng.Next(tilePrefabs.Length);

                try { SpawnTile(tilePrefabs[randomIndex], rng); }
                catch (NoNextCellException e)
                {
                    Debug.LogError(e.Message);
                    break;
                }

                if (Application.isPlaying && tileGenerationInterval > 0f) yield return new WaitForSeconds(tileGenerationInterval);
            }

            OnGenerate.Invoke();
            generationRoutine = null;
        }

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

            throw new NoNextCellException(LastCell, tilePrefab);
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

        private class NoNextCellException : Exception
        {
            private Vector3Int lastCell;
            private CourseTile nextTile;

            public NoNextCellException(Vector3Int lastCell, CourseTile nextTile) => (this.lastCell, this.nextTile) = (lastCell, nextTile);

            public override string Message => $"No cell found from {lastCell} for {nextTile.name}";
        }
    }
}