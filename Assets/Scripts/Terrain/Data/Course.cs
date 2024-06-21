using Displayable;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Terrain.Data
{
    [Serializable]
    public class Course : IListDisplayable<int>
    {
        [SerializeField] private string name;
        [SerializeField] private Hole[] holeData;

        public string Name => name;
        public Hole[] HoleData => holeData;
        public int Length => holeData.Length;
        public int Par => holeData.Sum(hole => hole.TileCount);
        public int[] Pars => holeData.Select(hole => hole.TileCount).ToArray();

        IList<int> IListDisplayable<int>.Values => Pars;

        public Course()
        {
            name = "Test Course";
            holeData = new Hole[18];
            for (int i = 0; i < holeData.Length; i++) holeData[i] = new Hole(i, 3);
        }

        public Course(string name, Hole[] holeData)
        {
            this.name = name;
            this.holeData = holeData;
        }
    }
}