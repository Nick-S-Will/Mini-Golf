using UnityEngine;

namespace MiniGolf.Managers
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T instance;

        [SerializeField] private bool isPersistent = true, logIfMultiple;

        protected virtual void Awake()
        {
            if (instance == null) instance = this as T;
            else
            {
                if (logIfMultiple) Debug.LogError($"Multiple {typeof(T).Name}s loaded");
                Destroy(gameObject);
                return;
            }

            if (isPersistent) DontDestroyOnLoad(instance.gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (instance == this) instance = null;
        }
    }
}