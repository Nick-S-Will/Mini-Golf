using MiniGolf.Player;
using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Network
{
    [RequireComponent(typeof(SwingController))]
    public class GolfGamePlayer : NetworkBehaviour
    {
        public static UnityEvent<GolfGamePlayer> OnStartPlayer = new();
        public static UnityEvent<GolfGamePlayer> OnSetLocalPlayer = new();

        [SerializeField] private MeshRenderer ballRenderer;
        [Space]
        [SyncVar]
        public int index;
        [SyncVar(hook = nameof(OnPhysicsChanged))]
        public bool physicsEnabled = true;

        private GolfRoomPlayer roomPlayer;

        public GolfRoomPlayer RoomPlayer
        {
            get
            {
                if (roomPlayer) return roomPlayer;

                var roomPlayers = FindObjectsOfType<GolfRoomPlayer>();
                var correctIndexRoomPlayers = roomPlayers.Where(player => index == player.index);
                if (correctIndexRoomPlayers.Count() == 1) roomPlayer = correctIndexRoomPlayers.First();
                else Debug.LogError($"Found {correctIndexRoomPlayers.Count()} {nameof(GolfRoomPlayer)}s with index '{index}'.");

                return roomPlayer;
            }
        }
        public SwingController Player { get; private set; }

        private void Awake()
        {
            if (ballRenderer == null) Debug.LogError($"{nameof(ballRenderer)} not assigned");

            Player = GetComponent<SwingController>();
        }

        private void Start()
        {
            OnStartPlayer.Invoke(this);

            ballRenderer.material.color = RoomPlayer.Color;
        }

        public override void OnStartLocalPlayer()
        {
            PlayerHandler.singleton.ProgressHandler.OnStartHole.AddListener(EnablePhysics);
            PlayerHandler.singleton.ProgressHandler.OnPlayerWaiting.AddListener(DisablePhysics);

            OnSetLocalPlayer.Invoke(this);
        }

        private void EnablePhysics() => SetPhysics(true);

        private void DisablePhysics() => SetPhysics(false);

        [Command]
        private void SetPhysics(bool enabled)
        {
            physicsEnabled = enabled;
        }

        private void OnPhysicsChanged(bool oldEnabled, bool newEnabled)
        {
            if (oldEnabled == newEnabled) return;

            Player.Rigidbody.isKinematic = !newEnabled;
        }
    }
}