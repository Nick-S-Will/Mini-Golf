using UnityEngine;
using UnityEngine.InputSystem;

using MiniGolf.Managers.Game;
using MiniGolf.Terrain.Data;
using MiniGolf.Player;
using MiniGolf.Progress;
using MiniGolf.Terrain.Data.UI;
using MiniGolf.UI;

namespace MiniGolf.Network.UI
{
    public class Scoreboard : DisplayMaker<Display<PlayerScore>, PlayerScore>
    {
        [Space]
        [SerializeField] private CourseParDisplay courseDisplay;
        [SerializeField] private GameObject graphicParent;

        private bool visibilityLocked;

        protected override void Awake()
        {
            base.Awake();

            if (courseDisplay == null) Debug.LogError($"{nameof(courseDisplay)} not assigned");
            if (graphicParent == null) Debug.LogError($"{nameof(graphicParent)} not assigned");

            SwingController.OnStartPlayer.AddListener(AddScoreLine);
        }

        private void Start()
        {
            var managerExists = GameManager.singleton != null;
            if (!managerExists) Debug.LogWarning($"No {nameof(GameManager)} loaded");

            var course = managerExists ? GameManager.singleton.SelectedCourse : new Course();
            courseDisplay.SetObject(course);

            ProgressHandler.singleton.OnCompleteCourse.AddListener(LockVisible);
        }

        private void OnDestroy()
        {
            SwingController.OnStartPlayer.RemoveListener(AddScoreLine);
            if (ProgressHandler.singleton) ProgressHandler.singleton.OnCompleteCourse.RemoveListener(LockVisible);
        }

        private void AddScoreLine(SwingController player)
        {
            var playerScore = player.GetComponent<PlayerScore>();
            MakeDisplay(playerScore);
        }

        public void LockVisible()
        {
            SetVisible(true);
            visibilityLocked = true;
        }

        public void Toggle(InputAction.CallbackContext context)
        {
            if (context.started) SetVisible(true);
            else if (context.canceled) SetVisible(false);
        }

        public void SetVisible(bool visible)
        {
            if (visibilityLocked) return;

            graphicParent.SetActive(visible);

            if (!visible) return;

            UpdateDisplays();
        }
    }
}