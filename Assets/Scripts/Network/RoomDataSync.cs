using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Network
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class RoomDataSync : NetworkBehaviour
    {
        public static RoomDataSync singleton;

        public static UnityEvent OnPlayerJoined = new();

        private void Awake()
        {
            if (singleton)
            {
                Debug.LogError($"Multiple {nameof(RoomDataSync)}s loaded");
                Destroy(gameObject);
                return;
            }

            singleton = this;

            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }

        [ClientRpc]
        public void NotifyPlayerJoined() => OnPlayerJoined.Invoke();
    }
}