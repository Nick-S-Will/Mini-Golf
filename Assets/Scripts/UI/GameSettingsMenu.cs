using MiniGolf.Managers.Game;
using MiniGolf.Network;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.UI
{
    public class GameSettingsMenu : MonoBehaviour
    {
        [SerializeField] private Toggle usingWallsToggle;

        private void Awake()
        {
            if (usingWallsToggle == null) Debug.LogError($"{nameof(usingWallsToggle)} not assigned");
        }

        private void OnEnable()
        {
            usingWallsToggle.isOn = GameManager.singleton.UsingWalls;
        }

        public void SetUsingWalls(bool usingWalls)
        {
            if (RoomDataSync.singleton) RoomDataSync.singleton.SetUsingWalls(usingWalls);
            else GameManager.singleton.UsingWalls = usingWalls;
        }
    }
}