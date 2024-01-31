using System;
using UnityEngine;

namespace MiniGolf.Terrain
{
    [Serializable]
    public struct GenerationSettings
    {
        public static GenerationSettings Default = new(0, 3);

        [SerializeField] private int seed;
        [SerializeField][Min(2f)] private int courseLength;

        public int Seed => seed;
        public int CourseLength => courseLength;

        public GenerationSettings(int seed, int courseLength)
        {
            this.seed = seed;
            this.courseLength = courseLength;
        }
    }
}