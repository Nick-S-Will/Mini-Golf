using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Network.UI
{
    public class PlayerColorPicker : MonoBehaviour
    {
        [SerializeField] private Image colorDisplayImage;

        private Color color = Color.white;

        private void Awake()
        {
            if (colorDisplayImage == null) Debug.LogError($"{nameof(colorDisplayImage)} not assigned");
        }

        public void SetRed(float red)
        {
            color.r = red;
            UpdatePlayerColor();
        }
        public void SetGreen(float green)
        {
            color.g = green;
            UpdatePlayerColor();
        }
        public void SetBlue(float blue)
        {
            color.b = blue;
            UpdatePlayerColor();
        }

        private void UpdatePlayerColor()
        {
            colorDisplayImage.color = color;
            NetworkClient.localPlayer.GetComponent<GolfRoomPlayer>().SetColor(color);
        }
    }
}