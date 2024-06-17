using TMPro;
using UnityEngine;

namespace MiniGolf.Progress.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            ProgressHandler.singleton.OnStartHole.AddListener(UpdateTextValue);
            ProgressHandler.singleton.OnStroke.AddListener(UpdateTextValue);
        }

        private void OnDestroy()
        {
            if (ProgressHandler.singleton == null) return;

            ProgressHandler.singleton.OnStartHole.RemoveListener(UpdateTextValue);
            ProgressHandler.singleton.OnStroke.RemoveListener(UpdateTextValue);
        }

        private void UpdateTextValue()
        {
            scoreText.text = ProgressHandler.singleton.CurrentScore.ToString();
        }
    }
}