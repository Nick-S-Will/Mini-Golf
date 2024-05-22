using MiniGolf.Progress;
using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Network
{
    public class PlayerScore : NetworkBehaviour, IContainer<int>
    {
        [SyncVar]
        public int index;
        [Space]
        public UnityEvent OnScoreChange;

        private readonly SyncList<int> scores = new();

        public string Name => $"Player {index + 1}";
        public int[] Scores => scores.ToArray();
        public int Total => scores.Sum();

        int[] IContainer<int>.Values => Scores;

        public override void OnStartAuthority()
        {
            ProgressHandler.singleton.OnStroke.AddListener(UpdateScores);

            GetScores();
        }

        public override void OnStopAuthority()
        {
            ProgressHandler.singleton.OnStroke.RemoveListener(UpdateScores);
        }

        [Command]
        private void GetScores()
        {
            scores.Clear();
            var progressScores = ProgressHandler.singleton.Scores;
            foreach (var score in progressScores) scores.Add(score);
        }

        [Command]
        private void UpdateScores()
        {
            var progressScores = ProgressHandler.singleton.Scores;
            for (int i = 0; i < scores.Count; i++) scores[i] = progressScores[i];

            OnScoreChange.Invoke();
        }
    }
}