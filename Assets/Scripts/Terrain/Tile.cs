using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Terrain
{
    [RequireComponent(typeof(MeshFilter))]
    public class Tile : MonoBehaviour
    {
        public static readonly Vector3 SCALE = new(2f, 1f, 2f);

        [SerializeField] private Vector3Int[] cells = new Vector3Int[] { Vector3Int.zero };
        [SerializeField] private List<Vector3Int> availableDirections;
        [Space]
        [SerializeField] private Color cellColor = Color.white;
        [SerializeField] private Color directionColor = Color.blue;
        [SerializeField] private bool showCells;

        public Vector3Int[] AvailableDirections => availableDirections.ToArray();
        public Vector3Int[] LocalCells => cells;

        private void OnDrawGizmosSelected()
        {
            if (!showCells) return;

            Gizmos.color = cellColor;
            foreach (var cell in cells)
            {
                Gizmos.DrawWireCube(transform.position + transform.rotation * Vector3.Scale(SCALE, cell), SCALE);
            }

            Gizmos.color = directionColor;
            var offset = transform.position + transform.rotation * Vector3.Scale(SCALE, cells.Last());
            foreach (var cell in availableDirections)
            {
                Gizmos.DrawWireCube(offset + transform.rotation * Vector3.Scale(SCALE, cell), SCALE);
            }
        }
    }
}