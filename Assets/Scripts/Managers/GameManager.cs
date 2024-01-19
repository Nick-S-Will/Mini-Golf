using MiniGolf.Controls;
using MiniGolf.Terrain;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGolf.Managers
{
    public class GameManager : MonoBehaviour
    {
        private Dictionary<BallController, List<int>> playerScores;
        private HoleTile currentHole;
        private int holeNumber;

        private void Start()
        {
            playerScores = new Dictionary<BallController, List<int>>();
            var ballControllers = FindObjectsOfType<BallController>();
            foreach (var ballController in ballControllers)
            {
                ballController.OnSwing.AddListener(() => AddStroke(ballController));
                playerScores.Add(ballController, new List<int>());
            }
        }

        public void UpdateHole(HoleTile holeTile)
        {
            if (currentHole) currentHole.OnBallEnter.RemoveListener(BallFinish);

            currentHole = holeTile;
            currentHole.OnBallEnter.AddListener(BallFinish);
            holeNumber++;
        }

        private void AddStroke(BallController ballController)
        {
            if (ballController.BackswingScaler == 0f) return;
            if (!playerScores.TryGetValue(ballController, out var scores)) return;

            while (scores.Count < holeNumber) scores.Add(0);
            scores[holeNumber - 1]++;
        }

        private void BallFinish(BallController ballController)
        {
            var strokes = playerScores[ballController][holeNumber - 1];
            Debug.Log($"Ball reached hole with {strokes} strokes");
        }
    }
}