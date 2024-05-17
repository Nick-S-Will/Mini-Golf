using MiniGolf.Managers.Game;
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
        public const string PLAYER_NAME_KEY = "Player Name";

        [Header("Main")]
        [SerializeField] private Button courseSelectButton;
        [SerializeField] private Button leaveButton, backButton;
        [Header("Setup")]
        [SerializeField] private GameObject setupPanel;
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private Button hostButton, joinButton;
        [Header("Room")]
        [SerializeField] private GameObject roomPanel;
        [SerializeField] private Toggle readyToggle;
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
            if (readyToggle == null) Debug.LogError($"{nameof(readyToggle)} not assigned");
            if (startButton == null) Debug.LogError($"{nameof(startButton)} not assigned");

            var playerName = PlayerPrefs.GetString(PLAYER_NAME_KEY, null);
            nameInput.text = playerName;
            hasValidPlayerName = playerName != null;
            UpdateSetupButtons();
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

            PlayerPrefs.SetString(PLAYER_NAME_KEY, playerName);
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
        }

        private void UpdateSetupButtons()
        {
            hostButton.interactable = hasValidPlayerName;
            joinButton.interactable = hasValidPlayerName && hasValidIP;
        }
        #endregion

        #region Manager Wrappers (for UI events to access static manager singleton)
        public void SetMultiplayerState(bool isMultiplayer) => GameManager.singleton.isMultiplayer = isMultiplayer;

        public void HostRoom()
        {
            try
            {
                hostButton.interactable = false;
                GolfRoomManager.singleton.StartHost();
            }
            catch (SocketException e)
            {
                Debug.Log($"{nameof(SocketException)} occured.\n\n{e.StackTrace}");
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
                case NetworkManagerMode.Host: GolfRoomManager.singleton.StopHost(); break;
                case NetworkManagerMode.ClientOnly: GolfRoomManager.singleton.StopClient(); break;
            }
        }
        #endregion
    }
}