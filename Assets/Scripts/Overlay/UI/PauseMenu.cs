using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGolf.Overlay.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public bool Paused => gameObject.activeSelf;

        public void Toggle(InputAction.CallbackContext context)
        {
            if (context.started) SetActive(!Paused);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);

            Time.timeScale = active ? 0f : 1f;

            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = active;
        }
    }
}