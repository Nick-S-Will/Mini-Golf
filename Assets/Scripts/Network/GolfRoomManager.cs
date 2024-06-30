using MiniGolf.Managers.Game;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MiniGolf.Network
{
    public enum NetScene { Offline, Room, Game }

    public class GolfRoomManager : NetworkRoomManager
    {
        public static new GolfRoomManager singleton { get; private set; }
        public static Dictionary<NetScene, string> NetSceneToName { get; private set; }

        [SerializeField] private RoomDataSync roomDataSyncPrefab;

        public UnityEvent OnPlayerListChanged { get; private set; } = new();
        public bool ReadyToStart { get; private set; }

        public override void Awake()
        {
            base.Awake();

            if (NetworkManager.singleton != this) return;

            singleton = this;

            NetSceneToName ??= new() { { NetScene.Offline, offlineScene }, { NetScene.Room, RoomScene }, { NetScene.Game, GameplayScene } };
        }

        public override void OnRoomStartClient()
        {
            NetworkClient.RegisterHandler<PlayerListChangedMessage>(NotifyPlayerListChanged);
        }

        public override void OnRoomStopClient()
        {
            NetworkClient.UnregisterHandler<PlayerListChangedMessage>();
        }

        private void NotifyPlayerListChanged(PlayerListChangedMessage playerListChangedMessage) => OnPlayerListChanged.Invoke();
        
        public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
        {
            if (RoomDataSync.singleton && roomSlots.Count > 0)
            {
                RoomDataSync.singleton.netIdentity.RemoveClientAuthority();
                RoomDataSync.singleton.netIdentity.AssignClientAuthority(roomSlots[0].connectionToClient);
            }

            if (Utils.IsSceneActive(RoomScene) || Utils.IsSceneActive(GameplayScene))
            {
                NetworkServer.SendToAll(new PlayerListChangedMessage(false));
            }
        }

        #region Room
        public override void OnRoomServerSceneChanged(string sceneName)
        {
            if (sceneName != RoomScene) return;

            NetworkServer.SendToAll(new PlayerListChangedMessage(true));
        }

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);

            if (!Utils.IsSceneActive(RoomScene) || conn.identity == null) return;

            var roomPlayer = conn.identity.gameObject;
            SetGolfRoomPlayerVisible(roomPlayer, true);
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            if (!Utils.IsSceneActive(RoomScene)) return;

            NetworkServer.SendToAll(new PlayerListChangedMessage(true));

            if (RoomDataSync.singleton == null)
            {
                var roomDataSync = Instantiate(roomDataSyncPrefab, transform);
                NetworkServer.Spawn(roomDataSync.gameObject);
            }
            if (!RoomDataSync.singleton.netIdentity.isOwned) RoomDataSync.singleton.netIdentity.AssignClientAuthority(conn);
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
        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            SetGolfRoomPlayerVisible(roomPlayer, false);

            if (!roomPlayer.TryGetComponent(out GolfRoomPlayer golfRoomPlayer)) Debug.LogError($"{nameof(roomPlayer)} object's {nameof(NetworkRoomPlayer)} must descend from {nameof(GolfRoomPlayer)}");
            if (!gamePlayer.TryGetComponent(out GolfGamePlayer golfGamePlayer)) Debug.LogError($"{nameof(gamePlayer)} must have {nameof(GolfGamePlayer)} component");
            golfGamePlayer.index = golfRoomPlayer.index;

            return true;
        }

        public Vector3 GetHoleStartPosition()
        {
            startPositions.RemoveAll(t => t == null);

            if (startPositions.Count < maxConnections)
            {
                Debug.LogWarning($"Number of {nameof(startPositions)} ({startPositions.Count}) should be at least {nameof(maxConnections)} ({maxConnections})");
                return Vector3.zero;
            }

            var playerIndex = NetworkClient.localPlayer.GetComponent<GolfGamePlayer>().index;
            var position = startPositions[playerIndex].position;

            return position;
        }

        public void Quit()
        {
            switch (mode)
            {
                case NetworkManagerMode.ServerOnly: singleton.StopServer(); break;
                case NetworkManagerMode.ClientOnly: singleton.StopClient(); break;
                case NetworkManagerMode.Host: ServerChangeScene(RoomScene); break;
            }
        }

        public void EndRound()
        {
            if (mode == NetworkManagerMode.ClientOnly) return;

            ServerChangeScene(RoomScene);
        }
        #endregion

        private void SetGolfRoomPlayerVisible(GameObject roomPlayer, bool visible)
        {
            var golfRoomPlayer = roomPlayer.GetComponent<GolfRoomPlayer>();
            if (golfRoomPlayer) golfRoomPlayer.SetVisible(visible);
            else Debug.LogError($"{nameof(roomPlayer)} object's {nameof(NetworkRoomPlayer)} must descend from {nameof(GolfRoomPlayer)}");
        }
    }
}