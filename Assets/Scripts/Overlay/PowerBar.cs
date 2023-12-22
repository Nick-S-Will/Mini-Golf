using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay
{
    [RequireComponent(typeof(Slider))]
    public class PowerBar : MonoBehaviour
    {
        private Slider slider;

        private void Start()
        {
            slider = GetComponent<Slider>();
        }

        public void UpdateSliderValue(float value)
        {
            slider.value = value;
        }
    }
}