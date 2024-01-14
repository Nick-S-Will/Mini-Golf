using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Terrain
{
    public class CourseTile : MonoBehaviour
    {
        public static readonly Vector3 SCALE = new(2f, 1f, 2f);

        [SerializeField] private Vector3Int[] cells = new Vector3Int[] { Vector3Int.zero };
        [SerializeField] private Vector3Int[] availableDirections;
        [Space]
        [SerializeField] private Color cellColor = Color.white;
        [SerializeField] private Color directionColor = Color.blue;
        [SerializeField] private bool showCells;

        public Vector3Int[] AvailableDirections => availableDirections;
        public Vector3Int[] ShuffledDirections => Shuffle(availableDirections).ToArray();
        public Vector3Int[] LocalCells => cells;

        // Based on https://stackoverflow.com/a/1262619
        private static IList<T> Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        private void OnDrawGizmosSelected()
        {
            if (!showCells) return;

            Gizmos.color = cellColor;
            var offset = transform.position + SCALE.y / 2f * Vector3.up;
            foreach (var cell in cells)
            {
                Gizmos.DrawWireCube(offset + transform.rotation * Vector3.Scale(SCALE, cell), SCALE);
            }

            if (cells.Length == 0) return;

            Gizmos.color = directionColor;
            offset += transform.rotation * Vector3.Scale(SCALE, cells.Last());
            foreach (var cell in availableDirections)
            {
                Gizmos.DrawWireCube(offset + transform.rotation * Vector3.Scale(SCALE, cell), SCALE);
            }
        }
    }
}