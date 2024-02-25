using System;
using UnityEngine;

namespace MiniGolf.Terrain.Data
{
    [Serializable]
    public class Course
    {
        [SerializeField] private string name;
        [SerializeField] private HoleData[] holeData;

        public string Name => name;
        public int Par
        { 
            get
            {
                int totalPar = 0;
                foreach (var hole in holeData) totalPar += hole.TileCount;
                return totalPar;
            }
        }
        public HoleData[] HoleData => holeData;
        public int Length => holeData.Length;
    }
}