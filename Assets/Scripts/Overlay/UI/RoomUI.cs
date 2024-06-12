using UnityEngine;
using MiniGolf.Network;
using MiniGolf.Overlay;
using MiniGolf.Overlay.HUD;
using Mirror;
using UnityEngine.UI;

public class RoomUI : DisplayMaker<GolfRoomPlayerDisplay, GolfRoomPlayer>
{
    [Space]
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject[] leaderUIObjects;

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
        if (NetworkClient.ready && GolfRoomManager.singleton.PlayMode == MiniGolf.Network.PlayMode.Singleplayer)
        {
            if (localPlayer) SetReady(true);
            return;
        }

        UpdateDisplays();
        UpdateLeaderUI();
    }

    private void UpdatePlayerList(UpdatePlayerListMessage _) => UpdatePlayerList();

    private void UpdatePlayerList() 
    {
        SetObjects(FindObjectsOfType<GolfRoomPlayer>());

        localPlayer = NetworkClient.localPlayer.GetComponent<GolfRoomPlayer>();
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