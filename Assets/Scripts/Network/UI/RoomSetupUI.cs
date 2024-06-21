using MiniGolf.Managers.Game;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Network.UI
{
    public class RoomSetupUI : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private Button hostButton, joinButton;

        private bool hasValidPlayerName, hasValidIP;

        private void Awake()
        {
            if (nameInput == null) Debug.LogError($"{nameof(nameInput)} not assigned");
            if (hostButton == null) Debug.LogError($"{nameof(hostButton)} not assigned");
            if (joinButton == null) Debug.LogError($"{nameof(joinButton)} not assigned");

            var playerName = PlayerPrefs.GetString(GolfRoomPlayer.PLAYER_NAME_KEY, null);
            nameInput.text = playerName;
            hasValidPlayerName = playerName != null;
            UpdateButtons();
        }

        #region Input Handling
        public void UpdatePlayerName(string playerName)
        {
            hasValidPlayerName = !string.IsNullOrWhiteSpace(playerName);
            
            if (hasValidPlayerName) SavePlayerName(playerName);

            UpdateButtons();
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

            UpdateButtons();
        }
        #endregion

        private void UpdateButtons()
        {
            hostButton.interactable = hasValidPlayerName;
            joinButton.interactable = hasValidPlayerName && hasValidIP;
        }

        #region Manager Wrappers (for UI events to access static manager singleton)
        public void SetSingleplayer(bool isSingleplayer)
        {
            GameManager.singleton.NetPlayMode = isSingleplayer ? NetPlayMode.Singleplayer : NetPlayMode.Multiplayer;
        }

        public void ResetPlayMode() => GameManager.singleton.NetPlayMode = NetPlayMode.None;

        public void HostRoom()
        {
            try
            {
                hostButton.interactable = false;
                GolfRoomManager.singleton.StartHost();
            }
            catch (Exception e)
            {
                Debug.Log($"{e.GetType().Name} occured.\n\n{e.StackTrace}");
                hostButton.interactable = true;
            }
        }

        public void JoinRoom()
        {
            joinButton.interactable = false;
            GolfRoomManager.singleton.StartClient();
        }
        #endregion
    }
}