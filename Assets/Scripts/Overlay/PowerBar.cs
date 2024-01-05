using MiniGolf.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay
{
    [RequireComponent(typeof(Slider))]
    public class PowerBar : MonoBehaviour
    {
        [SerializeField] private SwingController swingController;
        private Slider slider;

        private void Start()
        {
            if (swingController == null) Debug.LogError($"{nameof(swingController)} not assigned");
            else swingController.OnBackswingChange.AddListener(UpdateSliderValue);
            
            slider = GetComponent<Slider>();
        }

        public void UpdateSliderValue()
        {
            slider.value = swingController.BackswingScaler;
        }

        public void ClearSliderValue()
        {
            slider.value = 0f;
        }

        private void OnDestroy()
        {
            if (swingController == null) return;

            swingController.OnBackswingChange.RemoveListener(UpdateSliderValue);
        }
    }
}