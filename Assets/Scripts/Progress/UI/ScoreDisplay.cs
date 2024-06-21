using TMPro;
using UnityEngine;

namespace MiniGolf.Progress.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private ProgressHandler progressHandler;
        [Space]
        [SerializeField] private TMP_Text scoreText;

        private void Awake()
        {
            if (progressHandler == null) Debug.LogError($"{nameof(progressHandler)} not assigned");

            progressHandler.OnStartHole.AddListener(UpdateTextValue);
            progressHandler.OnStrokeAdded.AddListener(UpdateTextValue);
        }

        private void OnDestroy()
        {
            if (progressHandler == null) return;

            progressHandler.OnStartHole.RemoveListener(UpdateTextValue);
            progressHandler.OnStrokeAdded.RemoveListener(UpdateTextValue);
        }

        private void UpdateTextValue()
        {
            scoreText.text = progressHandler.CurrentScore.ToString();
        }
    }
}