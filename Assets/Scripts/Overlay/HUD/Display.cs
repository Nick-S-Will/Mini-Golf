using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniGolf.Overlay.HUD
{
    public abstract class Display<T> : MonoBehaviour
    {
        protected Button button;

        public Button Button => button;

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
        }

        public abstract void UpdateText(T element);

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