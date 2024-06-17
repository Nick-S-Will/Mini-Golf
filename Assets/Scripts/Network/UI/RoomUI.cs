using UnityEngine;
using Displayable;
using Mirror;
using UnityEngine.UI;

namespace MiniGolf.Network.UI
{
    public class RoomUI : DisplayMaker<GolfRoomPlayerDisplay, GolfRoomPlayer>
    {
        [Space]
        [SerializeField] private Toggle readyToggle;
        [SerializeField] private Button startButton;
        [Space]
        [SerializeField] private GameObject[] leaderUIObjects;

        private GolfRoomPlayer localPlayer;

        protected override void Awake()
        {
            base.Awake();

            if (readyToggle == null) Debug.LogError($"{nameof(readyToggle)} not assigned");
            if (startButton == null) Debug.LogError($"{nameof(startButton)} not assigned");

            NetworkClient.RegisterHandler<PlayerListChangedMessage>(UpdatePlayerList);
        }

        private void OnDestroy()
        {
            NetworkClient.UnregisterHandler<PlayerListChangedMessage>();
        }

        private void Update()
        {
            if (NetworkClient.ready && GolfRoomManager.singleton.PlayMode == MiniGolf.Network.PlayMode.Singleplayer)
            {
                if (localPlayer) SetReady(true);
                return;
            }

            UpdateDisplays();
            UpdateLeaderUI();
        }

        private void UpdatePlayerList(PlayerListChangedMessage _) => UpdatePlayerList();

        private void UpdatePlayerList()
        {
            if (NetworkClient.localPlayer) localPlayer = NetworkClient.localPlayer.GetComponent<GolfRoomPlayer>();
            if (localPlayer) readyToggle.isOn = localPlayer.readyToBegin;

            SetObjects(FindObjectsOfType<GolfRoomPlayer>());
        }

        private void UpdateLeaderUI()
        {
            var active = localPlayer && localPlayer.IsLeader;

            startButton.gameObject.SetActive(active);
            if (active) startButton.interactable = GolfRoomManager.singleton.ReadyToStart;

            foreach (var obj in leaderUIObjects) obj.SetActive(active);
        }

        public void SetReady(bool ready) => localPlayer.CmdChangeReadyState(ready);

        #region Manager Wrappers (for UI events to access static manager singleton)
        public void LeaveRoom()
        {
            switch (GolfRoomManager.singleton.mode)
            {
                case NetworkManagerMode.Host: GolfRoomManager.singleton.StopHost(); break;
                case NetworkManagerMode.ClientOnly: GolfRoomManager.singleton.StopClient(); break;
                case NetworkManagerMode.ServerOnly: GolfRoomManager.singleton.StopServer(); break;
            }
        }

        public void StartGame() => GolfRoomManager.singleton.StartGame();
        #endregion
    }
}