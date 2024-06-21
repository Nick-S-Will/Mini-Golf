using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System.Linq;

namespace MiniGolf.Network
{
    [RequireComponent(typeof(NetworkRigidbodyUnreliable))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class GolfRoomPlayer : NetworkRoomPlayer
    {
        public const string PLAYER_NAME_KEY = "Player Name";

        [SyncVar]
        [SerializeField] private string playerName = "Loading...";
        [SyncVar(hook = nameof(OnVisibilityChanged))]
        [SerializeField] private bool isVisible = true;

        [HideInInspector] public UnityEvent OnReadyChanged;

        private NetworkRigidbodyUnreliable networkRigidbody;
        private Rigidbody displayRigidbody;
        private SphereCollider sphereCollider;
        private MeshRenderer meshRenderer;

        public string Name => playerName;
        public bool IsLeader => index == 0;

        private void Awake()
        {
            networkRigidbody = GetComponent<NetworkRigidbodyUnreliable>();
            displayRigidbody = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public override void OnStartAuthority()
        {
            SetName(PlayerPrefs.GetString(PLAYER_NAME_KEY, $"Player {index + 1}"));
        }

        public override void IndexChanged(int oldIndex, int newIndex)
        {
            if (!Utils.IsSceneActive(GolfRoomManager.singleton.GameplayScene)) return;

            GetGamePlayer(oldIndex).index = newIndex;
        }

        private GolfGamePlayer GetGamePlayer(int index)
        {
            var gamePlayers = FindObjectsOfType<GolfGamePlayer>();
            var correctIndexGamePlayers = gamePlayers.Where(player => player.index == index);
            if (correctIndexGamePlayers.Count() == 1) return correctIndexGamePlayers.First();
            else Debug.LogError($"Found {correctIndexGamePlayers.Count()} {nameof(GolfGamePlayer)}s with index '{index}'.");

            return null;
        }

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            OnReadyChanged.Invoke();
        }

        [Command]
        private void SetName(string name)
        {
            playerName = name;
        }

        [Server]
        public void SetVisible(bool visible)
        {
            isVisible = visible;
        }

        public void OnVisibilityChanged(bool oldValue, bool newValue)
        {
            if (oldValue == newValue) return;

            networkRigidbody.enabled = newValue;
            displayRigidbody.isKinematic = !newValue;
            sphereCollider.enabled = newValue;
            meshRenderer.enabled = newValue;
        }
    }
}