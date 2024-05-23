using MiniGolf.Progress;
using Mirror;
using System.Linq;

namespace MiniGolf.Network
{
    public class PlayerScore : NetworkBehaviour, IContainer<int>
    {
        [SyncVar]
        public int index;

        private readonly SyncList<int> scores = new();

        public string Name => $"Player {index + 1}";
        public int[] Scores => scores.ToArray();
        public int Total => scores.Sum();

        int[] IContainer<int>.Values => Scores;

        public override void OnStartAuthority()
        {
            ProgressHandler.singleton.OnStroke.AddListener(UpdateScores);

            InitializeScores();
        }

        public override void OnStopAuthority()
        {
            ProgressHandler.singleton.OnStroke.RemoveListener(UpdateScores);
        }

        private void InitializeScores()
        {
            scores.Clear();
            var scoreCount = ProgressHandler.singleton.Scores.Length;
            for (int i = 0; i < scoreCount; i++) scores.Add(0);
        }

        private void UpdateScores()
        {
            var progressScores = ProgressHandler.singleton.Scores;
            for (int i = 0; i < scores.Count; i++) scores[i] = progressScores[i];
        }
    }
}