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

        private void Start()
        {
            if (swingIndicator == null)
            {
                Debug.LogError($"{nameof(swingIndicator)} not assigned");
                return;
            }

            startColor = swingIndicator.color;

            PlayerHandler.Player.OnStopMoving.AddListener(ShowCanSwing);
            PlayerHandler.Player.OnSwing.AddListener(ShowCannotSwing);

            if (!PlayerHandler.Player.IsMoving) ShowCanSwing();
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
            if (PlayerHandler.Player == null) return;
            
            PlayerHandler.Player.OnStopMoving.RemoveListener(ShowCanSwing);
            PlayerHandler.Player.OnSwing.RemoveListener(ShowCannotSwing);
        }
    }
}