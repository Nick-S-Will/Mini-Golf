using MiniGolf.Network;
using Mirror;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay.UI
{
    public class MultiplayerUI : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private Button courseSelectButton;
        [SerializeField] private Button leaveButton, backButton;
        [Header("Setup")]
        [SerializeField] private GameObject setupPanel;
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private Button hostButton, joinButton;
        [Header("Room")]
        [SerializeField] private GameObject roomPanel;
        [SerializeField] private Button startButton;

        private bool hasValidPlayerName, hasValidIP;

        private void Awake()
        {
            if (backButton == null) Debug.LogError($"{nameof(backButton)} not assigned");
            if (setupPanel == null) Debug.LogError($"{nameof(setupPanel)} not assigned");
            if (nameInput == null) Debug.LogError($"{nameof(nameInput)} not assigned");
            if (hostButton == null) Debug.LogError($"{nameof(hostButton)} not assigned");
            if (joinButton == null) Debug.LogError($"{nameof(joinButton)} not assigned");
            if (roomPanel == null) Debug.LogError($"{nameof(roomPanel)} not assigned");
            if (leaveButton == null) Debug.LogError($"{nameof(leaveButton)} not assigned");
            if (startButton == null) Debug.LogError($"{nameof(startButton)} not assigned");

            var playerName = PlayerPrefs.GetString(GolfRoomPlayer.PLAYER_NAME_KEY, null);
            nameInput.text = playerName;
            hasValidPlayerName = playerName != null;
            UpdateSetupButtons();
        }

        private void Start()
        {
            GolfRoomManager.singleton.OnClientExit.AddListener(ShowSetupDisplay);
            GolfRoomManager.singleton.OnClientEnter.AddListener(ShowRoomDisplay);
            GolfRoomManager.singleton.OnPlayerIndexChanged.AddListener(UpdateRoomButtons);
            GolfRoomManager.singleton.OnPlayerReadyChanged.AddListener(UpdateRoomButtons);
        }

        #region Input Handling
        public void UpdatePlayerName(string playerName)
        {
            hasValidPlayerName = !string.IsNullOrWhiteSpace(playerName);
            
            if (hasValidPlayerName) SavePlayerName(playerName);

            UpdateSetupButtons();
        }

        private void SavePlayerName(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                Debug.LogError($"{nameof(playerName)} should only be saved when it isn't null or empty");
                return;
            }

            PlayerPrefs.SetString(GolfRoomPlayer.PLAYER_NAME_KEY, playerName);
        }

        public void UpdateIP(string ip)
        {
            hasValidIP = Regex.Match(ip, @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$").Success; // From https://stackoverflow.com/questions/5284147/validating-ipv4-addresses-with-regexp

            if (hasValidIP) GolfRoomManager.singleton.networkAddress = ip;

            UpdateSetupButtons();
        }
        #endregion

        #region UI Updates
        public void ShowSetupDisplay() => SetPanelDisplay(true);
        
        public void ShowRoomDisplay() => SetPanelDisplay(false);

        private void SetPanelDisplay(bool inSetup)
        {
            leaveButton.gameObject.SetActive(!inSetup);
            backButton.gameObject.SetActive(inSetup);

            setupPanel.SetActive(inSetup);
            roomPanel.SetActive(!inSetup);

            if (inSetup) UpdateSetupButtons();
            else UpdateRoomButtons();
        }

        private void UpdateSetupButtons()
        {
            hostButton.interactable = hasValidPlayerName;
            joinButton.interactable = hasValidPlayerName && hasValidIP;
        }

        private void UpdateRoomButtons()
        {
            bool localPlayerExists = GolfRoomManager.singleton.LocalPlayer;
            if (!localPlayerExists)
            {
                Debug.LogWarning($"{nameof(GolfRoomManager.singleton.LocalPlayer)} doesn't exist");
                return;
            }

            bool localPlayerIsLeader = GolfRoomManager.singleton.LocalPlayer.IsLeader;
            courseSelectButton.gameObject.SetActive(localPlayerIsLeader);
            startButton.gameObject.SetActive(localPlayerIsLeader);
            startButton.interactable = GolfRoomManager.singleton.allPlayersReady;
        }
        #endregion

        #region Golf Room Manager Wrappers
        public void HostRoom()
        {
            try
            {
                hostButton.interactable = false;
                GolfRoomManager.singleton.StartHost();
            }
            catch (SocketException e)
            {
                Debug.Log($"{nameof(SocketException)} occured. Probably trying to host 2 games at once.\n\n{e.StackTrace}");
                hostButton.interactable = true;
            }
        }

        public void JoinRoom()
        {
            joinButton.interactable = false;
            GolfRoomManager.singleton.StartClient();
        }

        public void LeaveRoom()
        {
            switch (GolfRoomManager.singleton.mode)
            {
                case NetworkManagerMode.Host:
                    GolfRoomManager.singleton.StopHost();
                    break;
                case NetworkManagerMode.ClientOnly:
                    GolfRoomManager.singleton.StopClient();
                    break;
            }
        }

        public void SetReady(bool ready) => GolfRoomManager.singleton.LocalPlayer.CmdChangeReadyState(ready);

        public void StartGame() => GolfRoomManager.singleton.ServerChangeScene(GolfRoomManager.singleton.GameplayScene);
        #endregion

        private void OnDestroy()
        {
            if (GolfRoomManager.singleton == null) return;

            GolfRoomManager.singleton.OnClientExit.RemoveListener(ShowSetupDisplay);
            GolfRoomManager.singleton.OnClientEnter.RemoveListener(ShowRoomDisplay);
            GolfRoomManager.singleton.OnPlayerIndexChanged.RemoveListener(UpdateRoomButtons);
            GolfRoomManager.singleton.OnPlayerReadyChanged.RemoveListener(UpdateRoomButtons);
        }
    }
}