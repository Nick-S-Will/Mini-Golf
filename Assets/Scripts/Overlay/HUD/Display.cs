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
        }

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