using System;
using UnityEngine;

namespace MiniGolf.Terrain.Data
{
    [Serializable]
    public struct HoleData
    {
        [SerializeField] private int seed;
        [SerializeField][Min(2f)] private int tileCount;

        public readonly int Seed => seed;
        public readonly int TileCount => tileCount;
    }
}