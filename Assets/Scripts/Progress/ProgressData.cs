using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniGolf.Progress
{
    [Serializable]
    public class ProgressData
    {
        [SerializeField] private string name;
        [SerializeField] private ProgressHandler progressHandler;

        private int[] scores;

        public string Name => name;
        public int[] Scores => progressHandler ? progressHandler.Scores : scores;
        public int Total => Scores.Sum();

        public ProgressData(string name, ProgressHandler progressHandler)
        {
            this.name = name;
            this.progressHandler = progressHandler;
        }

        public ProgressData(string name, int[] scores)
        {
            this.name = name;
            this.scores = scores;
        }
    }
}