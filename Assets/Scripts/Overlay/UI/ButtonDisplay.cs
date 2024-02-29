using MiniGolf.Overlay.HUD;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonDisplay<T> : Display<T>
    {
        protected Button button;

        public Button Button => button;

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
        }
    }
}