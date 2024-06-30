using MiniGolf.Managers.Game;
using MiniGolf.Terrain;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGolf.Player.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject hudParent, graphicsParent;
        [SerializeField] private HoleGenerator holeGenerator;

        public bool IsPaused => graphicsParent.activeSelf;

        private void Awake()
        {
            SetCursor(false);
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
            hudParent.SetActive(!active);
            graphicsParent.SetActive(active);

            var canSwing = !active && !holeGenerator.CurrentHoleTile.Contains(PlayerHandler.Player);
            PlayerHandler.SetControls(canSwing, !active, true);

            SetCursor(active);
        }

        public void SetCursor(bool visible)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }

        public void Quit() => GameManager.Quit();
    }
}