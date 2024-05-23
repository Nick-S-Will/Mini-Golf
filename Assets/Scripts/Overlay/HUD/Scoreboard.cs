using UnityEngine;
using UnityEngine.InputSystem;

using MiniGolf.Managers.Game;
using MiniGolf.Network;
using MiniGolf.Terrain.Data;
using MiniGolf.Player;

namespace MiniGolf.Overlay.HUD
{
    public class Scoreboard : DisplayMaker<Display<PlayerScore>, PlayerScore>
    {
        [Space]
        [SerializeField] private CourseParDisplay courseDisplay;
        [SerializeField] private GameObject graphicParent;

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
        }

        private void OnDestroy()
        {
            SwingController.OnStartPlayer.RemoveListener(AddScoreLine);
        }

        private void AddScoreLine(SwingController player)
        {
            var playerScore = player.GetComponent<PlayerScore>();
            MakeDisplay(playerScore);
        }

        public void Toggle(InputAction.CallbackContext context)
        {
            if (context.started) SetVisible(true);
            else if (context.canceled) SetVisible(false);
        }

        public void SetVisible(bool visible)
        {
            graphicParent.SetActive(visible);

            if (!visible) return;

            // TODO: Sort by score
            foreach (var playerDisplay in displayInstances) playerDisplay.UpdateText();
        }
    }
}