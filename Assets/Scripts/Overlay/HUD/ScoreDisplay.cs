using MiniGolf.Progress;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private ProgressHandler progressHandler;
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            progressHandler.OnStroke.AddListener(UpdateTextValue);
            progressHandler.OnCompleteHole.AddListener(UpdateTextValue);
        }

        private void UpdateTextValue()
        {
            scoreText.text = progressHandler.CurrentScore.ToString();
        }
    }
}