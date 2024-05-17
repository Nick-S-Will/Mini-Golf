using UnityEngine;

namespace MiniGolf
{
    public class DestroyIfPlatform : MonoBehaviour
    {
        [SerializeField] private RuntimePlatform platform = RuntimePlatform.WebGLPlayer;

        private void Awake()
        {
            if (Application.platform == platform) Destroy(gameObject);
        }
    }
}