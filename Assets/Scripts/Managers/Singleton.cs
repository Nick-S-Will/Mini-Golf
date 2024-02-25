using UnityEngine;

namespace MiniGolf.Managers
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T instance;

        protected virtual void Awake()
        {
            if (instance == null) instance = this as T;
            else
            {
                Debug.LogError($"Multiple {typeof(T).Name}s loaded");
                return;
            }

            DontDestroyOnLoad(instance.gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (instance == this) instance = null;
        }
    }
}