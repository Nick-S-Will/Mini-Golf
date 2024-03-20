using MiniGolf.Managers;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerHandler : Singleton<PlayerHandler>
    {
        public static BallController Player => instance ? instance.player : null;
        public static PlayerInput Input => instance ? instance.playerInput : null;

        [Space]
        [SerializeField] private BallController playerPrefab;
        [SerializeField] private Transform camTransform;

        /// <summary>
        /// Passes old player and new player as <see cref="BallController"/>s
        /// </summary>
        [HideInInspector]
        public UnityEvent<BallController, BallController> OnPlayerUpdate;
        private BallController player;
        private PlayerInput playerInput;

        protected override void Awake()
        {
            base.Awake();

            if (camTransform == null) Debug.LogError($"{nameof(camTransform)} not assigned");

            bool isMultiplayer = NetworkManager.singleton;
            UpdatePlayer(isMultiplayer ? NetworkClient.localPlayer.GetComponent<BallController>() : Instantiate(playerPrefab));
            playerInput = GetComponent<PlayerInput>();
        }

        private void UpdatePlayer(BallController newPlayer)
        {
            OnPlayerUpdate.Invoke(Player, newPlayer);

            player = newPlayer;
            if (Player) Player.camTransform = camTransform;
        }

        public void ToggleBackSwing(InputAction.CallbackContext context) => Player.ToggleBackswing(context);
        public void BackSwinging(InputAction.CallbackContext context) => Player.Backswinging(context);

        protected override void OnDestroy() => base.OnDestroy();
    }
}