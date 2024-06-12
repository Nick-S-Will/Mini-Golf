using MiniGolf.Managers.Game;
using Mirror;
using UnityEngine;

namespace MiniGolf.Network
{
    public class RoomDataSync : NetworkBehaviour
    {
        public static RoomDataSync singleton;

        [SyncVar(hook = nameof(SelectedCourseIndexChanged))]
        private int selectedCourseIndex;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(RoomDataSync)}s loaded");
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }

        [Command]
        public void SetSelectedCourse(int courseIndex)
        {
            selectedCourseIndex = courseIndex;
        }
        
        private void SelectedCourseIndexChanged(int _, int newIndex)
        {
            GameManager.singleton.SelectedCourseIndex = newIndex;
        }
    }
}