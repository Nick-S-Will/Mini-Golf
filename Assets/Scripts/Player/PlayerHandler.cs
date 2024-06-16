using Cinemachine;
using MiniGolf.Network;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Player
{
    public class PlayerHandler : Singleton<PlayerHandler>
    {
        /// <summary>
        /// Passes old player and new player as <see cref="SwingController"/>s. For updating observers etc.
        /// </summary>
        public static UnityEvent<SwingController, SwingController> OnSetPlayer = new();
        /// <summary>
        /// Happens after <see cref="OnSetPlayer"/> to be sure all oberservers are updated
        /// </summary>
        public static UnityEvent OnPlayerReady = new();
        public static SwingController Player => singleton ? singleton.player : null;

        public static bool SwingControlsEnabled
        {
            get => singleton.playerInput.enabled;
            set => singleton.playerInput.enabled = value;
        }

        public static bool UIControlsEnabled
        {
            get => singleton.uiInput.enabled;
            set => singleton.uiInput.enabled = value;
        }

        public static bool CameraControlsEnabled
        {
            get => singleton.cameraInputs.Any(input => input.enabled);
            set
            {
                foreach (var cameraInput in singleton.cameraInputs) cameraInput.enabled = value;
            }
        }

        [SerializeField] private PlayerInput playerInput, uiInput;

        private CinemachineInputProvider[] cameraInputs;
        private SwingController player;

        protected override void Awake()
        {
            base.Awake();

            cameraInputs = FindObjectsOfType<CinemachineInputProvider>();

            if (GolfRoomManager.singleton) SwingController.OnSetLocalPlayer.AddListener(SetPlayer);
            else Debug.LogWarning($"No {nameof(GolfRoomManager)} loaded");
        }

        private void SetPlayer(SwingController player)
        {
            this.player = player;
            SetControls(player);

            OnSetPlayer.Invoke(Player, player);
            if (player) OnPlayerReady.Invoke();
        }

        public static void SetControls(bool allEnabled) => SetControls(allEnabled, allEnabled, allEnabled);

        public static void SetControls(bool swingEnabled, bool cameraEnabled, bool uiEnabled)
        {
            SwingControlsEnabled = swingEnabled;
            CameraControlsEnabled = cameraEnabled;
            UIControlsEnabled = uiEnabled;
        }

        public void ToggleBackSwing(InputAction.CallbackContext context) => Player.ToggleBackswing(context);
        public void BackSwinging(InputAction.CallbackContext context) => Player.Backswinging(context);

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SwingController.OnSetLocalPlayer.RemoveListener(SetPlayer);
        }
    }
}