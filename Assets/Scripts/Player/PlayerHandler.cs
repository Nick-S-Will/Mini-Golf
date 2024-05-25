using Cinemachine;
using Mirror;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Player
{
    [RequireComponent(typeof(PlayerInput))]
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

        [Space]
        [SerializeField] private BallController playerPrefab;

        private PlayerInput playerInput;
        private CinemachineInputProvider[] cameraInputs;
        private SwingController player;

        protected override void Awake()
        {
            base.Awake();

            if (playerPrefab == null) Debug.LogError($"{nameof(playerPrefab)} not assigned");

            playerInput = GetComponent<PlayerInput>();
            cameraInputs = FindObjectsOfType<CinemachineInputProvider>();

            if (NetworkManager.singleton) SwingController.OnSetLocalPlayer.AddListener(SetPlayer);
            else SetPlayer(Instantiate(playerPrefab, Vector3.up, Quaternion.identity));
        }

        private void SetPlayer(SwingController player)
        {
            this.player = player;
            SetControls(player);

            OnSetPlayer.Invoke(Player, player);
            if (player) OnPlayerReady.Invoke();
        }

        public static void SetControls(bool enabled) => SetControls(enabled, enabled);

        public static void SetControls(bool playerEnabled, bool cameraEnabled)
        {
            singleton.playerInput.enabled = playerEnabled;
            foreach (var cameraInput in singleton.cameraInputs) cameraInput.enabled = cameraEnabled;
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