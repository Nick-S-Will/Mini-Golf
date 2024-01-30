using MiniGolf.Progress;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private ProgressManager progressManager;
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            progressManager.OnStroke.AddListener(UpdateTextValue);
            progressManager.OnCompleteCourse.AddListener(UpdateTextValue);
        }

        private void UpdateTextValue()
        {
            scoreText.text = progressManager.CurrentScore.ToString();
        }
    }
}