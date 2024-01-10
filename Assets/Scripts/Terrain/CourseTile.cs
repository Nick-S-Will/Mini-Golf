using UnityEngine;

namespace MiniGolf.Terrain
{
    public class CourseTile : MonoBehaviour
    {
        [SerializeField] private Vector3Int[] availableDirections;
        [SerializeField] private int height = 0;

        public Vector3Int[] AvailableDirections => availableDirections;
        public int Height => height;
    }
}