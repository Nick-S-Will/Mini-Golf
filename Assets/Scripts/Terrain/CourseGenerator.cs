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
        private HashSet<Vector3Int> usedCells = new();
        private Vector3Int lastCell, nextCell;

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

                AddTile(tilePrefabs[randomIndex]);
                if (tileIndex < courseLength - 1) UpdateNextCellPosition();
            }
        }

        public void Clear()
        {
            foreach (var tileInstance in tileInstances) ContextDestroy(tileInstance.gameObject);
            tileInstances.Clear();
            usedCells.Clear();
            lastCell = Vector3Int.back;
            nextCell = Vector3Int.zero;
        }

        private void AddTile(CourseTile tilePrefab)
        {
            var position = CellToPosition(nextCell);
            var rotation = transform.rotation * Quaternion.LookRotation(Vector3.ProjectOnPlane(nextCell - lastCell, Vector3.up));
            var newTile = Instantiate(tilePrefab, position, rotation, transform);
            tileInstances.Add(newTile);

            var cell = nextCell;
            for (var i = 0; i <= newTile.Height; i++)
            {
                usedCells.Add(cell);
                cell += Vector3Int.up;
            }
        }

        private Vector3 CellToPosition(Vector3Int cell) => transform.rotation * Vector3.Scale(TILE_SCALE, cell);

        private void UpdateNextCellPosition()
        {
            var lastTile = tileInstances.Last();
            var shuffledDirections = lastTile.AvailableDirections.Select(cell => (cell, UnityEngine.Random.value)).OrderBy(tuple => tuple.value).Select(tuple => tuple.cell).ToArray();
            var nextCellChanged = false;

            foreach (var direction in shuffledDirections)
            {
                var cell = Vector3Int.RoundToInt(nextCell + lastTile.transform.rotation * direction) + lastTile.Height * Vector3Int.up;
                if (usedCells.Contains(cell)) continue;

                lastCell = nextCell;
                nextCell = cell;
                nextCellChanged = true;
                break;
            }

            if (!nextCellChanged) Debug.LogWarning("No next cell found");
        }

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
    }
}