using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Terrain
{
    [RequireComponent(typeof(MeshCollider))]
    public class CourseGenerator : MonoBehaviour
    {
        private static Action<UnityEngine.Object> ContextDestroy => Application.isPlaying ? Destroy : DestroyImmediate;
        public static readonly Vector3 TILE_SCALE = new(2f, 1f, 2f);

        [SerializeField] private CourseTile[] startTilePrefabs, courseTilePrefabs, holeTilePrefabs;
        [SerializeField][Min(2f)] private int courseLength = 3;
        [Space]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showGrid;

        private List<CourseTile> tileInstances = new();
        private List<Vector3Int> usedCells = new();

        public void Generate() // TODO: Create collision mesh
        {
            if (startTilePrefabs.Length == 0 || courseTilePrefabs.Length == 0 || holeTilePrefabs.Length == 0)
            {
                Debug.LogError($"{nameof(startTilePrefabs)}, {nameof(courseTilePrefabs)}, or {nameof(holeTilePrefabs)} is empty");
                return;
            }

            Clear();

            for (int tileIndex = 0; tileIndex < courseLength; tileIndex++)
            {
                CourseTile[] tilePrefabs;
                if (tileIndex == 0) tilePrefabs = startTilePrefabs;
                else if (tileIndex == courseLength - 1) tilePrefabs = holeTilePrefabs;
                else tilePrefabs = courseTilePrefabs;
                var randomIndex = UnityEngine.Random.Range(0, tilePrefabs.Length);

                try { AddTile(tilePrefabs[randomIndex]); }
                catch (NoNextCellException e)
                {
                    Debug.LogError(e.Message);
                    break;
                }
            }
        }

        public void Clear()
        {
            foreach (var tileInstance in tileInstances) ContextDestroy(tileInstance.gameObject);
            tileInstances.Clear();
            usedCells.Clear();
        }

        private void AddTile(CourseTile tilePrefab)
        {
            var cell = GetCellFor(tilePrefab);
            var lastCell = usedCells.Count == 0 ? Vector3Int.back : usedCells.Last();
            var rotation = transform.rotation * Quaternion.LookRotation(Vector3.ProjectOnPlane(cell - lastCell, Vector3.up));
            var newTile = Instantiate(tilePrefab, CellToPosition(cell), rotation, transform);
            tileInstances.Add(newTile);

            usedCells.Add(cell);
            for (var i = 1; i <= newTile.Height; i++)
            {
                cell += Vector3Int.up;
                usedCells.Add(cell);
            }
        }

        private Vector3Int GetCellFor(CourseTile tilePrefab)
        {
            if (tileInstances.Count == 0) return Vector3Int.zero;

            var lastTile = tileInstances.Last();
            var shuffledDirections = lastTile.AvailableDirections
                .Select(cell => (cell, UnityEngine.Random.value))
                .OrderBy(tuple => tuple.value)
                .Select(tuple => tuple.cell).ToArray();


            foreach (var direction in shuffledDirections)
            {
                var cell = Vector3Int.RoundToInt(usedCells.Last() + lastTile.transform.rotation * direction);
                if (usedCells.Contains(cell)) continue;

                return cell;
            }

            throw new NoNextCellException(usedCells.Last(), tilePrefab);
        }

        private Vector3 CellToPosition(Vector3Int cell) => transform.rotation * Vector3.Scale(TILE_SCALE, cell);

        private void OnDrawGizmos()
        {
            if (!showGrid) return;

            Gizmos.color = gizmoColor;
            var offsetToCenter = TILE_SCALE.y / 2f * Vector3.up;
            foreach (var cell in usedCells)
            {
                Gizmos.DrawWireCube(CellToPosition(cell) + offsetToCenter, TILE_SCALE);
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