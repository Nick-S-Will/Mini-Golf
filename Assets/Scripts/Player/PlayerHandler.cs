using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MiniGolf.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerHandler : Singleton<PlayerHandler>
    {
        /// <summary>
        /// Passes old player and new player as <see cref="SwingController"/>s. Listen to update observers etc.
        /// </summary>
        public static UnityEvent<SwingController, SwingController> OnSetPlayer = new();
        /// <summary>
        /// Happens after <see cref="OnSetPlayer"/> to be sure all oberservers are updated. Listen to change game state for new player
        /// </summary>
        public static UnityEvent OnPlayerReady = new();
        public static SwingController Player
        {
            get => singleton ? singleton.player : null;
        }
        
        [Space]
        [SerializeField] private BallController playerPrefab;

        private PlayerInput playerInput;
        private SwingController player;

        protected override void Awake()
        {
            base.Awake();

            if (playerPrefab == null) Debug.LogError($"{nameof(playerPrefab)} not assigned");

            playerInput = GetComponent<PlayerInput>();

            if (NetworkManager.singleton) SwingController.OnSetLocalPlayer.AddListener(SetPlayer);
            else SetPlayer(Instantiate(playerPrefab, Vector3.up, Quaternion.identity));
        }

        private void SetPlayer(SwingController player)
        {
            singleton.playerInput.enabled = player;
            singleton.player = player;

            OnSetPlayer.Invoke(Player, player);
            OnPlayerReady.Invoke();
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