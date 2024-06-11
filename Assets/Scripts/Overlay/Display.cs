using UnityEngine;

namespace MiniGolf.Overlay
{
    public abstract class Display<T> : MonoBehaviour
    {
        protected T displayObject;

        public T DisplayObject => displayObject;

        public virtual void SetObject(T newObject)
        {
            displayObject = newObject;
            UpdateText();
        }

        /// <summary>Updates the <see cref="Display"/>'s overlay to reflect any changes in <see cref="DisplayObject"/></summary>
        public abstract void UpdateText();
    }
}