using System;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Terrain.Data
{
    [Serializable]
    public class Course
    {
        [SerializeField] private string name;
        [SerializeField] private HoleData[] holeData;

        public string Name => name;
        public HoleData[] HoleData => holeData;
        public int Length => holeData.Length;
        public int Par => holeData.Sum(hole => hole.TileCount);
        public int[] Pars => holeData.Select(hole => hole.TileCount).ToArray();

        public Course(string name, HoleData[] holeData)
        {
            this.name = name;
            this.holeData = holeData;
        }
    }
}