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

        [SyncVar]
        public int index;
        [SyncVar(hook = nameof(OnPhysicsChanged))]
        public bool physicsEnabled = true;

        public GolfRoomPlayer RoomPlayer
        {
            get
            {
                var roomPlayers = FindObjectsOfType<GolfRoomPlayer>();
                var correctIndexRoomPlayers = roomPlayers.Where(player => index == player.index);
                if (correctIndexRoomPlayers.Count() == 1) return correctIndexRoomPlayers.First();
                else Debug.LogError($"Found {correctIndexRoomPlayers.Count()} {nameof(GolfRoomPlayer)}s with index '{index}'.");

                return null;
            }
        }
        public SwingController Player { get; private set; }

        private void Awake()
        {
            Player = GetComponent<SwingController>();
        }

        private void Start()
        {
            OnStartPlayer.Invoke(this);
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