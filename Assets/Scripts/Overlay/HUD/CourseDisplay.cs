using MiniGolf.Terrain.Data;
using TMPro;
using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public class CourseDisplay : Display<Course>
    {
        [SerializeField] private TMP_Text courseName, par, holeCount;

        protected override void Awake() => base.Awake();
        
        public override void UpdateText()
        {
            bool isCourse = displayObject != null;
            if (courseName) courseName.text = isCourse ? displayObject.Name : string.Empty;
            if (par) par.text = isCourse ? displayObject.Par.ToString() : string.Empty;
            if (holeCount) holeCount.text = isCourse ? displayObject.Length.ToString() : string.Empty;
        }
    }
}