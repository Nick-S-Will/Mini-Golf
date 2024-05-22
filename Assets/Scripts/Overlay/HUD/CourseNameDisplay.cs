using MiniGolf.Terrain.Data;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public class CourseNameDisplay : Display<Course>
    {
        [SerializeField] private TMP_Text courseName;

        public override void UpdateText()
        {
            bool isCourse = displayObject != null;
            if (courseName) courseName.text = isCourse ? displayObject.Name : string.Empty;
        }
    }
}