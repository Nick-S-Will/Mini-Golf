using System;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Terrain
{
    public class Tile : MonoBehaviour
    {
        public static readonly Vector3 SCALE = new(2f, 1f, 2f);

        [Tooltip("Optional mesh to replace mesh filter's mesh in collider")]
        [SerializeField] private MeshFilter[] groundMeshes;
        [SerializeField] private MeshFilter[] wallMeshes;
        [Space]
        [SerializeField] private Vector3Int[] cells = new Vector3Int[] { Vector3Int.zero };
        [SerializeField] private Vector3Int outDirection = Vector3Int.forward;
        [Space]
        [SerializeField] private Color cellColor = Color.white;
        [SerializeField] private Color outDirectionColor = Color.blue;
        [SerializeField] private bool showCells;

        public MeshFilter[] GroundMeshFilters => groundMeshes.Length > 0 ? groundMeshes : throw new NullReferenceException($"{nameof(groundMeshes)} is empty");
        public MeshFilter[] WallMeshFilters => wallMeshes.Length > 0 ? wallMeshes : throw new NullReferenceException($"{nameof(wallMeshes)} is empty");
        public Vector3Int OutDirection => outDirection;
        public Vector3Int[] LocalCells => cells;

        [ContextMenu("Show Wall")]
        public void ActivateWall() => SetWall(true);

        [ContextMenu("Hide Wall")]
        public void DeactivateWall() => SetWall(false);

        public void SetWall(bool active)
        {
            foreach (var wall in wallMeshes) wall.gameObject.SetActive(active);
        }

        private void OnDrawGizmosSelected()
        {
            if (!showCells) return;

            Gizmos.color = cellColor;
            foreach (var cell in cells)
            {
                Gizmos.DrawWireCube(transform.position + transform.rotation * Vector3.Scale(SCALE, cell), SCALE);
            }

            Gizmos.color = outDirectionColor;
            var offset = transform.position + transform.rotation * Vector3.Scale(SCALE, cells.Last());
            Gizmos.DrawWireCube(offset + transform.rotation * Vector3.Scale(SCALE, outDirection), 0.95f * SCALE);
        }
    }
}