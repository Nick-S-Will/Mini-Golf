using MiniGolf.Managers.Game;
using MiniGolf.Network;
using Displayable;
using System;
using UnityEngine;

namespace MiniGolf.Terrain.Data.UI
{
    public class CourseButtonDisplayMaker : DisplayMaker<ButtonDisplay<Course>, Course>
    {
        [Space]
        [SerializeField] private CourseNameDisplay selectedCourseDisplay;

        private void Start()
        {
            if (GameManager.singleton == null)
            {
                Debug.LogWarning($"No {nameof(GameManager)} loaded");
                return;
            }

            GameManager.singleton.OnSelectedCourseChange.AddListener(UpdateSelectedCourseDisplay);

            var courses = GameManager.singleton.Courses;
            foreach (var course in courses)
            {
                var display = MakeDisplay(course);
                var courseIndex = Array.IndexOf(GameManager.singleton.Courses, course);
                display.Button.onClick.AddListener(() => SetSelectedCourse(courseIndex));
            }

            UpdateSelectedCourseDisplay();
        }

        private void SetSelectedCourse(int courseIndex)
        {
            if (RoomDataSync.singleton) RoomDataSync.singleton.SetSelectedCourse(courseIndex);
            else GameManager.singleton.SelectedCourseIndex = courseIndex;
        }

        private void UpdateSelectedCourseDisplay()
        {
            selectedCourseDisplay.SetObject(GameManager.singleton.SelectedCourse);
        }
    }
}