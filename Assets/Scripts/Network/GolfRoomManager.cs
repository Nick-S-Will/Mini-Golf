using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Network
{
    public class GolfRoomManager : NetworkRoomManager
    {
        public static new GolfRoomManager singleton { get; private set; }

        [Space]
        [SerializeField] private RoomDataSync roomDataSyncPrefab;
        [Space]
        [HideInInspector] public UnityEvent OnClientEnter, OnClientExit, OnPlayerStarted, OnPlayerExitedRoom, OnPlayerIndexChanged, OnPlayerNameChanged, OnPlayerReadyChanged;

        public GolfRoomPlayer LocalPlayer => NetworkClient.localPlayer ? NetworkClient.localPlayer.GetComponent<GolfRoomPlayer>() : null;
       
        public override void Awake()
        {
            base.Awake();
            singleton = this;
        }

        public override void Start() => base.Start();

        public override void OnStartServer()
        {
            base.OnStartServer();

            var roomDataSync = Instantiate(roomDataSyncPrefab);
            NetworkServer.Spawn(roomDataSync.gameObject);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            if (RoomDataSync.instance) Destroy(RoomDataSync.instance);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);

            if (roomSlots.Count > 0) return;
            _ = RoomDataSync.instance.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            var roomPlayer = (GolfRoomPlayer)conn.identity.GetComponent<NetworkRoomPlayer>();
            var requiresReassign = roomPlayer.IsLeader && roomSlots.Count > 1;

            base.OnServerDisconnect(conn);

            if (!requiresReassign) return;

            var roomDataSyncIdentity = RoomDataSync.instance.GetComponent<NetworkIdentity>();
            roomDataSyncIdentity.RemoveClientAuthority();
            _ = roomDataSyncIdentity.AssignClientAuthority(roomSlots[0].connectionToClient);
        }

        public override void OnRoomServerPlayersReady() {}

        public override void OnRoomClientEnter()
        {
            base.OnRoomClientEnter();

            OnClientEnter.Invoke();
        }

        public override void OnRoomClientDisconnect()
        {
            base.OnRoomClientDisconnect();

            OnClientExit.Invoke();
        }

        public override void OnDestroy() => base.OnDestroy();
    }
}