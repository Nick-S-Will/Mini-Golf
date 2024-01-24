using MiniGolf.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay
{
    public class SwingIndicator : MonoBehaviour
    {
        [SerializeField] private BallController ballController;
        [SerializeField] private SwingController swingController;
        [SerializeField] private Image swingIndicator;
        [SerializeField] private Color canSwingColor = Color.green;

        private Color startColor;

        private void Start()
        {
            if (ballController == null)
            {
                Debug.LogError($"{nameof(ballController)} not assigned");
                return;
            }
            if (swingController == null)
            {
                Debug.LogError($"{nameof(swingController)} not assigned");
                return;
            }
            if (swingIndicator == null)
            {
                Debug.LogError($"{nameof(swingIndicator)} not assigned");
                return;
            }

            startColor = swingIndicator.color;

            ballController.OnStopMoving.AddListener(ShowCanSwing);
            swingController.OnSwing.AddListener(ShowCannotSwing);

            if (!ballController.IsMoving) ShowCanSwing();
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
            if (ballController != null) ballController.OnStopMoving.RemoveListener(ShowCanSwing);
            if (swingController != null) swingController.OnSwing.RemoveListener(ShowCannotSwing);
        }
    }
}