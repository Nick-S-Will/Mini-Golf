using MiniGolf.Managers.SceneTransition;
using MiniGolf.Player;
using MiniGolf.Progress;
using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGolf.Network
{
    public class GolfRoomManager : NetworkRoomManager
    {
        public static new GolfRoomManager singleton { get; private set; }
        public static GolfRoomPlayer LocalRoomPlayer => NetworkClient.localPlayer ? NetworkClient.localPlayer.GetComponent<GolfRoomPlayer>() : null;
        public static BallController LocalPlayer => NetworkClient.localPlayer ? NetworkClient.localPlayer.GetComponent<BallController>() : null;

        [Space]
        [SerializeField] private RoomDataSync roomDataSyncPrefab;
        [Space]
        [HideInInspector] public UnityEvent OnClientEnter, OnClientExit, OnPlayerStarted, OnPlayerExitedRoom, OnPlayerIndexChanged, OnPlayerNameChanged, OnPlayerReadyChanged;

        public override void Awake()
        {
            base.Awake();
            singleton = this;
        }

        public override void Start() => base.Start();

        #region Room Events
        #region Server
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

            if (roomSlots.Count == 0) AssignLeader(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn.identity == null || !Utils.IsSceneActive(RoomScene))
            {
                base.OnServerDisconnect(conn);
                return;
            }

            var roomPlayer = conn.identity.GetComponent<GolfRoomPlayer>();
            var requiresLeaderReassign = roomPlayer.IsLeader && roomSlots.Count > 1;

            base.OnServerDisconnect(conn);

            if (requiresLeaderReassign) AssignLeader(roomSlots[0].connectionToClient);
        }

        private void AssignLeader(NetworkConnectionToClient conn)
        {
            var roomDataSyncIdentity = RoomDataSync.instance.GetComponent<NetworkIdentity>();
            roomDataSyncIdentity.RemoveClientAuthority();
            _ = roomDataSyncIdentity.AssignClientAuthority(conn);
        }

        // Prevents auto start on ready
        public override void OnRoomServerPlayersReady() {}

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            roomPlayer.SetActive(false);

            var golfPlayer = gamePlayer.GetComponent<GolfPlayer>();
            golfPlayer.index = roomPlayer.GetComponent<NetworkRoomPlayer>().index;

            return true;
        }
        #endregion

        #region Client
        public override void OnStartClient()
        {
            base.OnStartClient();

            NetworkClient.RegisterHandler<GameStartMessage>(StartClientGame);
        }

        public override void OnRoomClientEnter()
        {
            base.OnRoomClientEnter();

            OnClientEnter.Invoke();
        }

        public override void OnRoomClientDisconnect()
        {
            base.OnRoomClientDisconnect();

            SceneTransitionManager.ChangeScene(Scene.Title);

            OnClientExit.Invoke();
        }

        public override void ReadyStatusChanged()
        {
            base.ReadyStatusChanged();

            OnPlayerReadyChanged.Invoke();
        }
        #endregion
        #endregion

        #region Game Events
        private void StartClientGame(GameStartMessage message)
        {
            var players = FindObjectsOfType<GolfPlayer>();
            var localPlayer = players.First(player => player.index == message.playerIndex);
            PlayerHandler.Player = localPlayer.GetComponent<BallController>();

            ProgressHandler.singleton.CanChangeHoles = CanChangeHoles;
        }

        public bool CanChangeHoles(int ballsInHole) => ballsInHole == numPlayers;
        #endregion
    }
}