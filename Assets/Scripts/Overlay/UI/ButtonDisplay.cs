using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonDisplay<T> : Display<T>
    {
        protected Button button;

        public Button Button
        {
            get
            {
                if (button == null) button = GetComponent<Button>();
                return button;
            }
        }
    }
}