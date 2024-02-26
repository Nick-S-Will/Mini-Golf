using MiniGolf.Managers.Game;
using MiniGolf.Overlay.UI;
using MiniGolf.Terrain.Data;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public class CourseDisplayMaker : DisplayMaker<Course>
    {
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
                _ = display.TryAddOnClick(() => UpdateSelected(course));
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