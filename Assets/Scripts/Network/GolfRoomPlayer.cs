using UnityEngine;
using Mirror;
using UnityEngine.Events;

namespace MiniGolf.Network
{
    [RequireComponent(typeof(NetworkRigidbodyUnreliable))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class GolfRoomPlayer : NetworkRoomPlayer
    {
        [SyncVar(hook = nameof(OnVisibilityChanged))]
        [SerializeField] private bool isVisible = true;

        [HideInInspector] public UnityEvent OnReadyChanged;

        private NetworkRigidbodyUnreliable networkRigidbody;
        private Rigidbody displayRigidbody;
        private SphereCollider sphereCollider;
        private MeshRenderer meshRenderer;

        public string Name => $"Player {index}"; // TODO: Make selected name work

        private void Awake()
        {
            networkRigidbody = GetComponent<NetworkRigidbodyUnreliable>();
            displayRigidbody = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnVisibilityChanged(bool oldValue, bool newValue)
        {
            if (oldValue == newValue) return;

            networkRigidbody.enabled = newValue;
            displayRigidbody.isKinematic = !newValue;
            sphereCollider.enabled = newValue;
            meshRenderer.enabled = newValue;
        }

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            OnReadyChanged.Invoke();
        }

        [Server]
        public void SetVisible(bool visible)
        {
            isVisible = visible;
        }

        public override void OnGUI()
        {
            base.OnGUI();
        }
    }
}