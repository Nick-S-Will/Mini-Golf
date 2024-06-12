using UnityEngine;
using Mirror;
using UnityEngine.Events;
using MiniGolf.Overlay.UI;
using System;

namespace MiniGolf.Network
{
    [RequireComponent(typeof(NetworkRigidbodyUnreliable))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class GolfRoomPlayer : NetworkRoomPlayer, IComparable<GolfRoomPlayer>
    {
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
            SetName(PlayerPrefs.GetString(RoomSetupUI.PLAYER_NAME_KEY, $"Player {index + 1}"));
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

        private void OnVisibilityChanged(bool oldValue, bool newValue)
        {
            if (oldValue == newValue) return;

            networkRigidbody.enabled = newValue;
            displayRigidbody.isKinematic = !newValue;
            sphereCollider.enabled = newValue;
            meshRenderer.enabled = newValue;
        }

        public int CompareTo(GolfRoomPlayer other) => playerName.CompareTo(other.playerName);

        public override void OnGUI()
        {
            base.OnGUI();
        }
    }
}