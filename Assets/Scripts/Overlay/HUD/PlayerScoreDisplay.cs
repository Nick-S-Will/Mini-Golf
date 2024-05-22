using TMPro;
using UnityEngine;
using MiniGolf.Network;

namespace MiniGolf.Overlay.HUD
{
    public class PlayerScoreDisplay : ArrayDisplay<PlayerScore, int>
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text totalScoreText;

        protected override void Awake()
        {
            base.Awake();

            if (playerNameText == null) Debug.LogError($"{nameof(playerNameText)} not assigned");
        }

        public override void SetObject(PlayerScore playerScore)
        {
            if (displayObject) displayObject.OnScoreChange.RemoveListener(UpdateText);

            base.SetObject(playerScore);
            totalScoreText.transform.SetAsLastSibling();

            if (displayObject) displayObject.OnScoreChange.AddListener(UpdateText);
        }

        public override void UpdateText()
        {
            playerNameText.text = displayObject.Name;

            base.UpdateText();

            totalScoreText.text = displayObject.Total.ToString();
        }
    }
}