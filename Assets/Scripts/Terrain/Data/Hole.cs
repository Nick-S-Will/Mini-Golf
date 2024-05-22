using System;
using UnityEngine;

namespace MiniGolf.Terrain.Data
{
    [Serializable]
    public class Hole
    {
        [SerializeField] private int seed;
        [SerializeField][Min(2f)] private int tileCount;

        public int Seed => seed;
        public int TileCount => tileCount;

        public Hole() : this(0, 3) { }

        public Hole(int seed, int tileCount)
        {
            this.seed = seed;
            this.tileCount = tileCount;
        }
    }
}