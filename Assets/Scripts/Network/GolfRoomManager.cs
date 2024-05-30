using Mirror;
using UnityEngine;

namespace MiniGolf.Network
{
    public class GolfRoomManager : NetworkRoomManager
    {
        public static new GolfRoomManager singleton { get; private set; }

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
        #endregion

        #region Room
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            NetworkServer.SendToAll(new NewPlayerMessage());
        }

        public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
        {
            NetworkServer.SendToAll(new PlayerLeaveMessage());
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
        #endregion
    }
}