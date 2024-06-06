using UnityEngine;
using MiniGolf.Network;
using MiniGolf.Overlay;
using MiniGolf.Overlay.HUD;
using Mirror;
using UnityEngine.UI;
using MiniGolf.Managers.Game;

public class RoomUI : DisplayMaker<GolfRoomPlayerDisplay, GolfRoomPlayer>
{
    [SerializeField] private Button startButton;

    private GolfRoomPlayer localPlayer;

    protected override void Awake()
    {
        base.Awake();

        if (startButton == null) Debug.LogError($"{nameof(startButton)} not assigned");

        NetworkClient.RegisterHandler<UpdatePlayerListMessage>(UpdatePlayerList);
    }

    private void OnDestroy()
    {
        NetworkClient.UnregisterHandler<UpdatePlayerListMessage>();
    }

    private void Update()
    {
        if (!GameManager.singleton.IsMultiplayer)
        {
            if (localPlayer) SetReady(true);
            return;
        }

        UpdateDisplays();
        if (localPlayer) UpdateStartButton();
    }

    private void UpdatePlayerList(UpdatePlayerListMessage _) => UpdatePlayerList();

    private void UpdatePlayerList() 
    {
        SetObjects(FindObjectsOfType<GolfRoomPlayer>());

        localPlayer = NetworkClient.localPlayer.GetComponent<GolfRoomPlayer>();
    }

    private void UpdateStartButton()
    {
        startButton.gameObject.SetActive(localPlayer.index == 0);
        startButton.interactable = GolfRoomManager.singleton.ReadyToStart;
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