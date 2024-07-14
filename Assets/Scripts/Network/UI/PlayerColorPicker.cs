using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Network.UI
{
    public class PlayerColorPicker : MonoBehaviour
    {
        private const string RED_KEY = "Player Color R", GREEN_KEY = "Player Color G", BLUE_KEY = "Player Color B";

        [SerializeField] private Slider redSlider, greenSlider, blueSlider;
        [SerializeField] private Image colorDisplayImage;

        private Color color;

        private void Awake()
        {
            if (redSlider == null) Debug.LogError($"{nameof(redSlider)} not assigned");
            if (greenSlider == null) Debug.LogError($"{nameof(greenSlider)} not assigned");
            if (blueSlider == null) Debug.LogError($"{nameof(blueSlider)} not assigned");

            ReadColor();

            redSlider.onValueChanged.AddListener(SetRed);
            greenSlider.onValueChanged.AddListener(SetGreen);
            blueSlider.onValueChanged.AddListener(SetBlue);

            GolfRoomManager.singleton.OnPlayerListChanged.AddListener(UpdatePlayerColor);
        }

        private void ReadColor()
        {
            var r = PlayerPrefs.GetFloat(RED_KEY, 1f);
            redSlider.value = r;
            var g = PlayerPrefs.GetFloat(GREEN_KEY, 1f);
            greenSlider.value = g;
            var b = PlayerPrefs.GetFloat(BLUE_KEY, 1f);
            blueSlider.value = b;
            color = new Color(r, g, b);
        }

        public void SetRed(float red)
        {
            color.r = red;
            PlayerPrefs.SetFloat(RED_KEY, red);
            UpdatePlayerColor();
        }
        public void SetGreen(float green)
        {
            color.g = green;
            PlayerPrefs.SetFloat(GREEN_KEY, green);
            UpdatePlayerColor();
        }
        public void SetBlue(float blue)
        {
            color.b = blue;
            PlayerPrefs.SetFloat(BLUE_KEY, blue);
            UpdatePlayerColor();
        }

        private void UpdatePlayerColor()
        {
            if (colorDisplayImage) colorDisplayImage.color = color;
            if (NetworkClient.localPlayer) NetworkClient.localPlayer.GetComponent<GolfRoomPlayer>().SetColor(color);
        }
    }
}