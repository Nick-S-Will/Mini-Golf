using MiniGolf.Score;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            ScoreManager.instance.OnStroke.AddListener(UpdateTextValue);
        }

        private void UpdateTextValue()
        {
            scoreText.text = ScoreManager.instance.CurrentScore.ToString();
        }
    }
}