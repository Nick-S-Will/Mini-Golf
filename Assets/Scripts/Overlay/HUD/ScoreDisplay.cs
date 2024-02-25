using MiniGolf.Managers.Progress;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private ProgressManager progressManager;
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            progressManager.OnStroke.AddListener(UpdateTextValue);
            progressManager.OnCompleteHole.AddListener(UpdateTextValue);
        }

        private void UpdateTextValue()
        {
            scoreText.text = progressManager.CurrentScore.ToString();
        }
    }
}