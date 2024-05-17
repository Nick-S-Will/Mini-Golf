using MiniGolf.Player;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay.HUD
{
    public class SwingIndicator : MonoBehaviour
    {
        [SerializeField] private Image swingIndicator;
        [SerializeField] private Color canSwingColor = Color.green;

        private Color startColor;

        private void Awake()
        {
            if (swingIndicator == null)
            {
                Debug.LogError($"{nameof(swingIndicator)} not assigned");
                return;
            }

            startColor = swingIndicator.color;

            PlayerHandler.OnChangePlayer.AddListener(PlayerChanged);
        }

        private void PlayerChanged(BallController oldPlayer, BallController newPlayer)
        {
            if (oldPlayer)
            {
                oldPlayer.OnSwing.RemoveListener(ShowCannotSwing);
                oldPlayer.OnStopMoving.RemoveListener(ShowCanSwing);
            }
            if (newPlayer)
            {
                newPlayer.OnSwing.AddListener(ShowCannotSwing);
                newPlayer.OnStopMoving.AddListener(ShowCanSwing);

                if (!newPlayer.IsMoving) ShowCanSwing();
                else ShowCannotSwing();
            }
        }

        private void ShowCanSwing()
        {
            swingIndicator.color = canSwingColor;
        }

        private void ShowCannotSwing()
        {
            swingIndicator.color = startColor;
        }

        private void OnDestroy()
        {
            PlayerHandler.OnChangePlayer.RemoveListener(PlayerChanged);

            if (PlayerHandler.Player == null) return;
            
            PlayerHandler.Player.OnStopMoving.RemoveListener(ShowCanSwing);
            PlayerHandler.Player.OnSwing.RemoveListener(ShowCannotSwing);
        }
    }
}