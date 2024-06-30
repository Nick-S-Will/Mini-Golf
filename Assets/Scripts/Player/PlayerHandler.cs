using MiniGolf.Managers.Game;
using MiniGolf.Network;
using MiniGolf.Progress;
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
            get => singleton.golfInput.enabled;
            set => singleton.golfInput.enabled = value;
        }

        public static bool UIControlsEnabled
        {
            get => singleton.uiInput.enabled;
            set => singleton.uiInput.enabled = value;
        }

        public static bool CameraControlsEnabled
        {
            get => singleton.cameraBehaviours.Any(input => input.enabled);
            set
            {
                foreach (var cameraInput in singleton.cameraBehaviours) cameraInput.enabled = value;
            }
        }

        [Space]
        [SerializeField] private SwingController playerPrefab;
        [SerializeField] private ProgressHandler progressHandler;
        [Space]
        [SerializeField] private PlayerInput golfInput;
        [SerializeField] private PlayerInput uiInput;
        [SerializeField] private MonoBehaviour[] cameraBehaviours;

        private SwingController player;

        public ProgressHandler ProgressHandler => progressHandler;

        protected override void Awake()
        {
            base.Awake();

            if (playerPrefab == null) Debug.LogError($"{nameof(playerPrefab)} not assigned");
            if (progressHandler == null) Debug.LogError($"{nameof(progressHandler)} not assigned");
            if (golfInput == null) Debug.LogError($"{nameof(golfInput)} not assigned");
            if (uiInput == null) Debug.LogError($"{nameof(uiInput)} not assigned");

            if (GameManager.IsMultiplayer) GolfGamePlayer.OnSetLocalPlayer.AddListener(SetLocalPlayer);
        }

        private void Start()
        {
            if (GameManager.IsSingleplayer) SpawnPlayer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            GolfGamePlayer.OnSetLocalPlayer.RemoveListener(SetLocalPlayer);
        }

        private void SetLocalPlayer(GolfGamePlayer netPlayer)
        {
            if (netPlayer == null)
            {
                Debug.LogError($"{nameof(GolfGamePlayer)} was null");
                return;
            }

            AssignPlayer(netPlayer.Player);
        }

        private void SpawnPlayer()
        {
            var player = Instantiate(playerPrefab);

            AssignPlayer(player);
        }

        private void AssignPlayer(SwingController newPlayer)
        {
            var oldPlayer = player;
            player = newPlayer;

            SetControls(newPlayer);
            newPlayer.GetComponent<PlayerScore>().ListenToProgressHandler(progressHandler);

            OnSetPlayer.Invoke(oldPlayer, newPlayer);
            OnPlayerReady.Invoke();
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
    }
}