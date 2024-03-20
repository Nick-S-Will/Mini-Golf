using MiniGolf.Player;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay.HUD
{
    [RequireComponent(typeof(Slider))]
    public class PowerBar : MonoBehaviour
    {
        private Slider slider;

        private void Start()
        {
            PlayerHandler.Player.OnBackswingChange.AddListener(UpdateSliderValue);
            
            slider = GetComponent<Slider>();
        }

        public void UpdateSliderValue()
        {
            slider.value = PlayerHandler.Player.BackswingScaler;
        }

        private void OnDestroy()
        {
            if (PlayerHandler.Player == null) return;

            PlayerHandler.Player.OnBackswingChange.RemoveListener(UpdateSliderValue);
        }
    }
}