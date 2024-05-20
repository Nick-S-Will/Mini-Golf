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

            PlayerHandler.OnSetPlayer.AddListener(ChangePlayer);
        }

        private void ChangePlayer(SwingController oldPlayer, SwingController newPlayer)
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
            PlayerHandler.OnSetPlayer.RemoveListener(ChangePlayer);

            if (PlayerHandler.Player == null) return;
            
            PlayerHandler.Player.OnStopMoving.RemoveListener(ShowCanSwing);
            PlayerHandler.Player.OnSwing.RemoveListener(ShowCannotSwing);
        }
    }
}