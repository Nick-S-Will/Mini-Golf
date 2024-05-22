using MiniGolf.Progress;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            ProgressHandler.singleton.OnStroke.AddListener(UpdateTextValue);
        }

        private void UpdateTextValue()
        {
            scoreText.text = ProgressHandler.singleton.CurrentScore.ToString();
        }
    }
}