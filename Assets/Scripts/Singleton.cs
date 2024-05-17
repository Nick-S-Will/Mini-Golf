using UnityEngine;

namespace MiniGolf
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T singleton;

        [SerializeField] private bool isPersistent = true, logIfMultiple;

        protected virtual void Awake()
        {
            if (singleton == null) singleton = this as T;
            else
            {
                if (logIfMultiple) Debug.LogError($"Multiple {typeof(T).Name}s loaded");
                Destroy(gameObject);
                return;
            }

            if (isPersistent) DontDestroyOnLoad(singleton.gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }
    }
}