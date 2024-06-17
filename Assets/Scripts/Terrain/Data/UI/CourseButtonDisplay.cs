using MiniGolf.UI;
using TMPro;
using UnityEngine;

namespace MiniGolf.Terrain.Data.UI
{
    public class CourseButtonDisplay : ButtonDisplay<Course>
    {
        [SerializeField] private TMP_Text courseName, par, holeCount;

        public override void UpdateText()
        {
            bool isCourse = displayObject != null;
            if (courseName) courseName.text = isCourse ? displayObject.Name : string.Empty;
            if (par) par.text = isCourse ? displayObject.Par.ToString() : string.Empty;
            if (holeCount) holeCount.text = isCourse ? displayObject.Length.ToString() : string.Empty;
        }
    }
}