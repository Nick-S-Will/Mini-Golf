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
        [SyncVar(hook = nameof(UsingWallsChanged))]
        private bool usingWalls;

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

        [Command]
        public void SetUsingWalls(bool usingWalls)
        {
            this.usingWalls = usingWalls;
        }
        
        private void SelectedCourseIndexChanged(int _, int newIndex)
        {
            GameManager.singleton.SelectedCourseIndex = newIndex;
        }

        private void UsingWallsChanged(bool _, bool newUsingWalls)
        {
            GameManager.singleton.UsingWalls = newUsingWalls;
        }
    }
}