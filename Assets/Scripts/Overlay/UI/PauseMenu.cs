using MiniGolf.Network;
using MiniGolf.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGolf.Overlay.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject hudParent, graphicsParent;

        private bool swingEnabled, cameraEnabled, uiEnabled;

        public bool IsPaused => graphicsParent.activeSelf;

        private void Awake()
        {
            SetCursor(false);
        }

        private void Start()
        {
            UpdateControlEnabledStatus();
        }
        private void OnDestroy()
        {
            SetCursor(true);
        }

        public void Toggle(InputAction.CallbackContext context)
        {
            if (context.started) SetActive(!IsPaused);
        }

        public void SetActive(bool active)
        {
            if (!IsPaused) UpdateControlEnabledStatus();

            hudParent.SetActive(!active);
            graphicsParent.SetActive(active);

            if (active) PlayerHandler.SetControls(false, false, true);
            else PlayerHandler.SetControls(swingEnabled, cameraEnabled, uiEnabled);
            
            SetCursor(active);
        }

        private void UpdateControlEnabledStatus()
        {
            swingEnabled = PlayerHandler.SwingControlsEnabled;
            cameraEnabled = PlayerHandler.CameraControlsEnabled;
            uiEnabled = PlayerHandler.UIControlsEnabled;
        }

        public void SetCursor(bool visible)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }

        public void Quit() => GolfRoomManager.singleton.QuitGame();
    }
}