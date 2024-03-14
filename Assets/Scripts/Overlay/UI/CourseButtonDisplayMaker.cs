using MiniGolf.Managers.Game;
using MiniGolf.Overlay.HUD;
using MiniGolf.Terrain.Data;
using UnityEngine;

namespace MiniGolf.Overlay.UI
{
    public class CourseButtonDisplayMaker : DisplayMaker<ButtonDisplay<Course>, Course>
    {
        [Space]
        [SerializeField] private CourseDisplay selectedCourseDisplay;

        private void Start()
        {
            if (GameManager.instance == null)
            {
                Debug.LogError($"No {nameof(GameManager)} loaded");
                return;
            }

            foreach (var course in GameManager.instance.Courses)
            {
                var display = MakeDisplay(course);
                display.Button.onClick.AddListener(() => UpdateSelected(course));
            }
            UpdateSelected(displayInstances[0].DisplayObject);
        }

        private void UpdateSelected(Course course)
        {
            selectedCourseDisplay.SetObject(course);
            GameManager.instance.SelectedCourse = course;
        }
    }
}