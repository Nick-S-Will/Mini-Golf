using MiniGolf.Managers.Game;
using MiniGolf.Overlay.UI;
using MiniGolf.Terrain.Data;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public class CourseDisplayMaker : DisplayMaker<Course>
    {
        private void Start()
        {
            if (GameManager.instance == null)
            {
                Debug.LogError($"No {nameof(GameManager)} loaded");
                return;
            }

            foreach (var course in GameManager.instance.Courses) MakeDisplay(course);
        }
    }
}