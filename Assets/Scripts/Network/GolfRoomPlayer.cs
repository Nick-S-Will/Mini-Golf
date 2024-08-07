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
        [SyncVar(hook = nameof(OnColorChanged))]
        [SerializeField] private Color color = Color.white;
        [SyncVar(hook = nameof(OnVisibilityChanged))]
        [SerializeField] private bool isVisible = true;

        [HideInInspector] public UnityEvent OnReadyChanged;

        private NetworkRigidbodyUnreliable networkRigidbody;
        private Rigidbody displayRigidbody;
        private SphereCollider sphereCollider;
        private MeshRenderer meshRenderer;

        public string Name => playerName;
        public Color Color => color;
        public bool IsLeader => index == 0;

        private void Awake()
        {
            networkRigidbody = GetComponent<NetworkRigidbodyUnreliable>();
            displayRigidbody = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
            meshRenderer = GetComponent<MeshRenderer>();

            GolfRoomManager.singleton.OnPlayerListChanged.AddListener(GoToStartPosition);
        }

        private void OnDestroy()
        {
            if (GolfRoomManager.singleton == null) return;

            GolfRoomManager.singleton.OnPlayerListChanged.RemoveListener(GoToStartPosition);
        }

        public override void OnStartAuthority()
        {
            SetName(PlayerPrefs.GetString(PLAYER_NAME_KEY, $"Player {index + 1}"));
        }

        private void GoToStartPosition()
        {
            transform.position = NetworkManager.startPositions[index].position;
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

        [Command]
        public void SetColor(Color color)
        {
            this.color = color;
        }

        private void OnColorChanged(Color oldColor, Color newColor)
        {
            if (oldColor == newColor) return;

            meshRenderer.material.color = newColor;
        }

        [Server]
        public void SetVisible(bool visible)
        {
            isVisible = visible;
        }

        private void OnVisibilityChanged(bool oldValue, bool newValue)
        {
            if (oldValue == newValue) return;

            networkRigidbody.enabled = newValue;
            displayRigidbody.isKinematic = !newValue;
            sphereCollider.enabled = newValue;
            meshRenderer.enabled = newValue;
        }
    }
}