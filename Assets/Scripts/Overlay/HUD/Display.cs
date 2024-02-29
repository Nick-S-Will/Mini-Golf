using UnityEngine;

namespace MiniGolf.Overlay.HUD
{
    public abstract class Display<T> : MonoBehaviour
    {
        protected T displayObject;

        public T DisplayObject => displayObject;

        public virtual void SetObject(T element)
        {
            displayObject = element;
            UpdateText();
        }

        /// <summary>Updates the <see cref="Display"/>'s overlay to reflect any changes in <see cref="DisplayObject"/></summary>
        public abstract void UpdateText();
    }
}