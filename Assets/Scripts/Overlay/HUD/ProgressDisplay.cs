using MiniGolf.Progress;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public class ProgressDisplay : Display<ProgressData>
    {
        [SerializeField] private TMP_Text playerNameText, scoreTextPrefab;

        private TMP_Text[] scoreTexts;

        protected override void Awake() => base.Awake();
        
        public override void SetObject(ProgressData progessData)
        {
            if (scoreTexts != null) foreach (var scoreText in scoreTexts) Destroy(scoreText.gameObject);

            scoreTexts = new TMP_Text[progessData.Scores.Length + 1];
            for (int i = 0; i < scoreTexts.Length; i++)
            {
                scoreTexts[i] = Instantiate(scoreTextPrefab, transform);
            }

            base.SetObject(progessData);
        }    

        public override void UpdateText()
        {
            playerNameText.text = displayObject.Name;

            var scores = displayObject.Scores;
            for (int i = 0; i < scores.Length; i++)
            {
                scoreTexts[i].text = scores[i].ToString();
            }
            scoreTexts[^1].text = displayObject.Total.ToString();
        }
    }
}