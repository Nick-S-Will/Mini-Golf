using MiniGolf.UI;
using TMPro;
using UnityEngine;

namespace MiniGolf.Network.UI
{
    public class PlayerScoreDisplay : ArrayDisplay<PlayerScore, int>
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text totalScoreText;

        protected override void Awake()
        {
            base.Awake();

            if (playerNameText == null) Debug.LogError($"{nameof(playerNameText)} not assigned");
            if (totalScoreText == null) Debug.LogError($"{nameof(totalScoreText)} not assigned");
        }

        public override void SetObject(PlayerScore playerScore)
        {
            base.SetObject(playerScore);
        }

        public override void UpdateText()
        {
            playerNameText.text = displayObject.Name;

            base.UpdateText();

            totalScoreText.transform.SetAsLastSibling();
            totalScoreText.text = displayObject.Total.ToString();
        }
    }
}