using Displayable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Progress
{
    public class PlayerScore : MonoBehaviour, IListDisplayable<int>
    {
        [SerializeField] private ProgressHandler progressHandler;
        public UnityEvent OnScoreChanged;

        private int[] scores = new int[0];

        public string Name { get; set; } = "Player 1";
        public int[] Scores => scores;
        public int Total => scores.Sum();

        IList<int> IListDisplayable<int>.Values => scores;

        private void Awake()
        {
            if (progressHandler) AddProgressListeners();
        }

        private void OnDestroy()
        {
            RemoveProgressListeners();
        }

        public void ListenToProgressHandler(ProgressHandler progressHandler, bool initializeOnly = false)
        {
            if (progressHandler == null)
            {
                Debug.LogError($"Can't listen to null {nameof(ProgressHandler)}");
                return;
            }

            RemoveProgressListeners();

            this.progressHandler = progressHandler;
            AddProgressListeners(initializeOnly);
        }

        private void AddProgressListeners(bool initializeOnly = false)
        {
            if (progressHandler.Scores.Length > 0) InitializeScores();
            else progressHandler.OnStartCourse.AddListener(InitializeScores);

            if (initializeOnly) return;

            progressHandler.OnStrokeAdded.AddListener(UpdateScores);
        }

        private void RemoveProgressListeners()
        {
            if (progressHandler == null) return;
            
            progressHandler.OnStartCourse.RemoveListener(InitializeScores);
            progressHandler.OnStrokeAdded.RemoveListener(UpdateScores);
        }

        private void InitializeScores()
        {
            scores = new int[progressHandler.Scores.Length];

            OnScoreChanged.Invoke();
        }

        private void UpdateScores()
        {
            var progressScores = progressHandler.Scores;
            var scoresIsDirty = false;
            for (int i = 0; i < progressScores.Length; i++)
            {
                if (scores[i] == progressScores[i]) continue;

                scores[i] = progressScores[i];
                scoresIsDirty = true;
            }

            if (scoresIsDirty) OnScoreChanged.Invoke();
        }
    }
}