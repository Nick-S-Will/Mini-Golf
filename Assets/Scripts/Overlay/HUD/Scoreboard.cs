using MiniGolf.Managers.Game;
using MiniGolf.Progress;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGolf.Overlay.HUD
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private ProgressData[] players;
        [Space]
        [SerializeField] private Transform scoreLineParent;
        [SerializeField] private ProgressDisplay progressDisplayPrefab;

        private ProgressDisplay[] playerDisplays;

        private void Awake()
        {
            if (players.Length == 0) Debug.LogError($"{nameof(players)} array empty");
            if (scoreLineParent == null) Debug.LogError($"{nameof(scoreLineParent)} not assigned");
            if (progressDisplayPrefab == null) Debug.LogError($"{nameof(progressDisplayPrefab)} not assigned");
        }

        private void Start()
        {
            var managerExists = GameManager.singleton != null;
            if (!managerExists) Debug.LogWarning($"No {nameof(GameManager)} loaded");

            var courseName = managerExists ? GameManager.singleton.SelectedCourse.Name : "Test Name";
            var pars = managerExists ? GameManager.singleton.SelectedCourse.Pars : new int[18];
            var headerData = new ProgressData(courseName, pars);
            var boardHeaderDisplay = Instantiate(progressDisplayPrefab, scoreLineParent);
            boardHeaderDisplay.SetObject(headerData);

            playerDisplays = new ProgressDisplay[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                playerDisplays[i] = Instantiate(progressDisplayPrefab, scoreLineParent);
                playerDisplays[i].SetObject(players[i]);
            }

            gameObject.SetActive(false);
        }

        public void Toggle(InputAction.CallbackContext context)
        {
            if (context.started) SetVisible(true);
            else if (context.canceled) SetVisible(false);
        }

        public void SetVisible(bool active)
        {
            gameObject.SetActive(active);

            if (!active) return;

            foreach (var playerDisplay in playerDisplays) playerDisplay.UpdateText();
        }
    }
}