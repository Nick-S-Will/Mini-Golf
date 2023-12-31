using MiniGolf.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay
{
    [RequireComponent(typeof(Slider))]
    public class PowerBar : MonoBehaviour
    {
        [SerializeField] private BallController ball;
        private Slider slider;

        private void Start()
        {
            if (ball == null) Debug.LogError($"{nameof(ball)} not assigned");
            else
            {
                ball.OnBackswingChange.AddListener(UpdateSliderValue);
                ball.OnSwing.AddListener(ClearSliderValue);
            }

            slider = GetComponent<Slider>();
        }

        public void UpdateSliderValue()
        {
            slider.value = ball.BackswingScaler;
        }

        public void ClearSliderValue()
        {
            slider.value = 0f;
        }

        private void OnDestroy()
        {
            if (ball == null) return;

            ball.OnBackswingChange.RemoveListener(UpdateSliderValue);
            ball.OnSwing.RemoveListener(ClearSliderValue);
        }
    }
}