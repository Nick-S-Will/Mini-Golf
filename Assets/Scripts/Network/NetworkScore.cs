using UnityEngine;
using MiniGolf.Progress;
using Mirror;
using System.Linq;

namespace MiniGolf.Network
{
    [RequireComponent(typeof(PlayerScore))]
    public class NetworkScore : NetworkBehaviour
    {
        private PlayerScore playerScore;
        private readonly SyncList<int> networkScores = new();

        private void Awake()
        {
            playerScore = GetComponent<PlayerScore>();

            networkScores.Callback += UpdateLocalScore;
        }

        private void Start()
        {
            playerScore.Name = GetComponent<GolfGamePlayer>().RoomPlayer.Name;
        }

        public override void OnStartClient()
        {
            if (!isOwned)
            {
                var progressHandler = FindObjectOfType<ProgressHandler>();
                if (progressHandler == null)
                {
                    Debug.LogError($"{nameof(NetworkScore)} depends on a {nameof(ProgressHandler)} existing in the scene.");
                    return;
                }

                playerScore.ListenToProgressHandler(progressHandler, true);
            }
        }

        public override void OnStartAuthority()
        {
            playerScore.OnScoreChanged.AddListener(SyncScores);
        }

        private void OnDestroy()
        {
            if (playerScore) playerScore.OnScoreChanged.RemoveListener(SyncScores);
        }

        private void SyncScores() => SyncScoresCommand(playerScore.Scores);

        [Command]
        private void SyncScoresCommand(int[] scores)
        {
            for (int i = 0; i < scores.Length; i++)
            {
                if (i == networkScores.Count)
                {
                    networkScores.AddRange(scores.Skip(i));
                    break;
                }
                
                networkScores[i] = scores[i];
            }
        }

        private void UpdateLocalScore(SyncList<int>.Operation op, int index, int oldItem, int newItem)
        {
            switch (op)
            {
                case SyncList<int>.Operation.OP_ADD:
                    if (index < playerScore.Scores.Length) playerScore.Scores[index] = newItem;
                    else Debug.LogError($"Tried {op} at {index} from {oldItem} to {newItem}.");
                    break;
                case SyncList<int>.Operation.OP_CLEAR:
                case SyncList<int>.Operation.OP_INSERT:
                case SyncList<int>.Operation.OP_REMOVEAT:
                    Debug.LogError($"Tried {op} at {index} from {oldItem} to {newItem}.");
                    break;
                case SyncList<int>.Operation.OP_SET:
                    playerScore.Scores[index] = newItem;
                    break;
            }
        }
    }
}