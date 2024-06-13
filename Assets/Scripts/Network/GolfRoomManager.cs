using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGolf.Network
{
    public enum PlayMode { None, Singleplayer, Multiplayer }

    public class GolfRoomManager : NetworkRoomManager
    {
        public static new GolfRoomManager singleton { get; private set; }

        [SerializeField] private RoomDataSync roomDataSyncPrefab;

        private PlayMode playMode;

        public PlayMode PlayMode
        {
            get => playMode;
            set
            {
                playMode = value;
                NetworkServer.dontListen = playMode == PlayMode.Singleplayer;
            }
        }
        public bool ReadyToStart { get; private set; }

        public override void Awake()
        {
            base.Awake();
            singleton = this;
        }

        #region Scene Change
        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);

            if (!Utils.IsSceneActive(RoomScene) || conn.identity == null) return;

            var roomPlayer = conn.identity.gameObject;
            SetGolfRoomPlayerVisible(roomPlayer, true);
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            SetGolfRoomPlayerVisible(roomPlayer, false);

            var playerScore = gamePlayer.GetComponent<PlayerScore>();
            if (playerScore) playerScore.index = roomPlayer.GetComponent<NetworkRoomPlayer>().index;
            else Debug.LogError($"{nameof(gamePlayer)} object must have {nameof(PlayerScore)} component");

            return true;
        }

        private void SetGolfRoomPlayerVisible(GameObject roomPlayer, bool visible)
        {
            var golfRoomPlayer = roomPlayer.GetComponent<GolfRoomPlayer>();
            if (golfRoomPlayer) golfRoomPlayer.SetVisible(visible);
            else Debug.LogError($"{nameof(roomPlayer)} object's {nameof(NetworkRoomPlayer)} must descend from {nameof(GolfRoomPlayer)}");
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            if (sceneName != RoomScene) return;

            NetworkServer.SendToAll(new UpdatePlayerListMessage(true));
        }
        #endregion

        #region Room
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            if (!Utils.IsSceneActive(RoomScene)) return;

            NetworkServer.SendToAll(new UpdatePlayerListMessage(true));

            if (RoomDataSync.singleton == null)
            {
                var roomDataSync = Instantiate(roomDataSyncPrefab, transform);
                NetworkServer.Spawn(roomDataSync.gameObject);
            }
            if (!RoomDataSync.singleton.netIdentity.isOwned) RoomDataSync.singleton.netIdentity.AssignClientAuthority(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

            if (RoomDataSync.singleton == null || roomSlots.Count == 0) return;

            RoomDataSync.singleton.netIdentity.RemoveClientAuthority();
            RoomDataSync.singleton.netIdentity.AssignClientAuthority(roomSlots[0].connectionToClient);
        }

        public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
        {
            if (!Utils.IsSceneActive(RoomScene)) return;

            NetworkServer.SendToAll(new UpdatePlayerListMessage(false));
        }

        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
        {
            var startTransform = GetStartPosition();
            var position = startTransform ? startTransform.position : Vector3.zero;
            var roomPlayer = Instantiate(roomPlayerPrefab, position, Quaternion.identity);

            return roomPlayer.gameObject;
        }

        public override void OnRoomServerPlayersReady()
        {
            if (PlayMode == PlayMode.Singleplayer)
            {
                StartGame();
                return;
            }

            if (Utils.IsHeadless()) base.OnRoomServerPlayersReady();
            else ReadyToStart = true;
        }

        public override void OnRoomServerPlayersNotReady() => ReadyToStart = false;

        public void StartGame()
        {
            ReadyToStart = false;

            ServerChangeScene(GameplayScene);
        }
        #endregion

        #region Game
        public Vector3 GetHoleStartPosition()
        {
            startPositions.RemoveAll(t => t == null);

            if (startPositions.Count < maxConnections)
            {
                Debug.LogWarning($"Number of {nameof(startPositions)} ({startPositions.Count}) should be at least {nameof(maxConnections)} ({maxConnections})");
                return Vector3.zero;
            }

            var playerScore = NetworkClient.localPlayer.GetComponent<PlayerScore>();
            var playerIndex = playerScore.index;
            var position = startPositions[playerIndex].position;

            return position;
        }

        public void QuitGame()
        {
            switch (singleton.mode)
            {
                case NetworkManagerMode.ServerOnly: singleton.StopServer(); break;
                case NetworkManagerMode.ClientOnly: singleton.StopClient(); break;
                case NetworkManagerMode.Host: EndGame(); break;
            }
        }

        public void EndGame() // TODO: Complete functionality for room scene to not crash after return
        {
            if (singleton.mode == NetworkManagerMode.ClientOnly) return;

            if (PlayMode == PlayMode.Singleplayer) singleton.StopHost();
            else ServerChangeScene(RoomScene);
        }
        #endregion
    }
}