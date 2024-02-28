using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniGolf.Overlay.HUD
{
    public abstract class Display<T> : MonoBehaviour
    {
        protected T displayObject;
        protected Button button;

        public T DisplayObject => displayObject;
        public Button Button => button;

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
        }

        public virtual void SetObject(T element)
        {
            displayObject = element;
            UpdateText();
        }

        /// <summary>Updates the <see cref="Display"/>'s overlay to reflect any changes in <see cref="DisplayObject"/></summary>
        public abstract void UpdateText();

        public virtual bool TryAddOnClick(UnityAction action)
        {
            if (button == null) return false;

            button.onClick.AddListener(action);
            return true;
        }

        public virtual bool TryRemoveOnClick(UnityAction action)
        {
            if (button == null) return false;

            button.onClick.RemoveListener(action);
            return true;
        }
    }
}