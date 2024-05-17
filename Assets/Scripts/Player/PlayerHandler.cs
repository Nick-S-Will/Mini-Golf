using MiniGolf.Managers.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerHandler : Singleton<PlayerHandler>
    {
        public static BallController Player
        {
            get => singleton ? singleton.player : null;
            set
            {
                singleton.SetInput(value);

                OnChangePlayer.Invoke(Player, value);
                singleton.player = value;
            }
        }

        [Space]
        [SerializeField] private BallController offlinePlayerPrefab;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private MonoBehaviour cameraBehaviour;

        /// <summary>
        /// Passes old player and new player as <see cref="BallController"/>s
        /// </summary>
        public static UnityEvent<BallController, BallController> OnChangePlayer = new();
        private BallController player;
        private PlayerInput playerInput;

        protected override void Awake()
        {
            base.Awake();

            if (offlinePlayerPrefab == null) Debug.LogError($"{nameof(offlinePlayerPrefab)} not assigned");
            if (cameraTransform == null) Debug.LogError($"{nameof(cameraTransform)} not assigned");
            if (cameraBehaviour == null) Debug.LogError($"{nameof(cameraBehaviour)} not assigned");

            playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            if (!GameManager.IsMultiplayer) Player = Instantiate(offlinePlayerPrefab);
        }

        public void ToggleBackSwing(InputAction.CallbackContext context) => Player.ToggleBackswing(context);
        public void BackSwinging(InputAction.CallbackContext context) => Player.Backswinging(context);

        public void SetInput(bool enabled)
        {
            playerInput.enabled = enabled;
            cameraBehaviour.enabled = enabled;
        }

        protected override void OnDestroy() => base.OnDestroy();
    }
}