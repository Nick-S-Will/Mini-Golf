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
        /// Passes old player and new player as <see cref="SwingController"/>s
        /// </summary>
        public static UnityEvent<SwingController, SwingController> OnSetPlayer = new();
        public static SwingController Player => singleton ? singleton.player : null;
        
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
            playerInput.enabled = player;

            OnSetPlayer.Invoke(Player, player);
            singleton.player = player;
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