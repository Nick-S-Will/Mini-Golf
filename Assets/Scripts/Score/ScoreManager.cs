using MiniGolf.Controls;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Score
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;

        [SerializeField] private BallController ball;
        public UnityEvent OnStroke;

        private readonly List<int> scoreList = new();

        public int[] Scores => scoreList.ToArray();
        public int CurrentScore => scoreList[^1];

        private void Awake()
        {
            if (instance) Debug.LogError($"Multiple {nameof(ScoreManager)}s exist in scene");
            else instance = this;
        }

        private void Start()
        {
            ball.OnSwing.AddListener(AddStroke);
            scoreList.Add(0);
        }

        private void AddStroke()
        {
            scoreList[^1]++;
            OnStroke.Invoke();
        }
        
        private void OnDestroy()
        {
            if (instance == this) instance = null;
        }
    }
}