using MiniGolf.Player;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay.HUD
{
    [RequireComponent(typeof(Slider))]
    public class PowerBar : MonoBehaviour
    {
        private Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();

            PlayerHandler.OnChangePlayer.AddListener(PlayerChanged);
        }

        private void PlayerChanged(BallController oldPlayer, BallController newPlayer)
        {
            if (oldPlayer) oldPlayer.OnBackswingChange.RemoveListener(UpdateSliderValue);
            if (newPlayer) newPlayer.OnBackswingChange.AddListener(UpdateSliderValue);
        }

        public void UpdateSliderValue()
        {
            slider.value = PlayerHandler.Player.BackswingScaler;
        }

        private void OnDestroy()
        {
            PlayerHandler.OnChangePlayer.RemoveListener(PlayerChanged);
            if (PlayerHandler.Player) PlayerHandler.Player.OnBackswingChange.RemoveListener(UpdateSliderValue);
        }
    }
}