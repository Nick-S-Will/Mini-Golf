using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGolf.Overlay.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public bool Paused => Time.timeScale == 0f;

        public void Toggle(InputAction.CallbackContext context)
        {
            if (context.started) SetActive(!Paused);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);

            SetPaused(active);

            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = active;
        }

        private void SetPaused(bool paused)
        {
            Time.timeScale = paused ? 0f : 1f;
        }

        private void OnDestroy()
        {
            if (Paused) SetPaused(false);
        }
    }
}