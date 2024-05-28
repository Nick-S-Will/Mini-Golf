using MiniGolf.Managers.SceneTransition;
using MiniGolf.Network;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGolf.Overlay.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject hudParent;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private string playActionMap = "Golf", pauseActionMap = "UI";
        [SerializeField] private MonoBehaviour cameraBehaviour;

        public bool Paused => Time.timeScale == 0f;

        private void Awake()
        {
            SetCursor(false);
        }

        public void Toggle(InputAction.CallbackContext context)
        {
            if (context.started) SetActive(!Paused);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            hudParent.SetActive(!active);

            playerInput.SwitchCurrentActionMap(active ? pauseActionMap : playActionMap);
            if (cameraBehaviour) cameraBehaviour.enabled = !active;

            SetPaused(active);
            SetCursor(active);
        }

        private void SetPaused(bool paused)
        {
            Time.timeScale = paused ? 0f : 1f;
        }

        public void SetCursor(bool visible)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }

        public void Quit()
        {
            if (GolfRoomManager.singleton == null)
            {
                SceneTransitionManager.ChangeScene(Scene.Title);
                return;
            }

            switch (GolfRoomManager.singleton.mode)
            {
                case NetworkManagerMode.Host: GolfRoomManager.singleton.StopHost(); break;
                case NetworkManagerMode.ClientOnly: GolfRoomManager.singleton.StopClient(); break;
                case NetworkManagerMode.ServerOnly: GolfRoomManager.singleton.StopServer(); break;
            }
        }

        private void OnDestroy()
        {
            SetPaused(false);
            SetCursor(true);
        }
    }
}