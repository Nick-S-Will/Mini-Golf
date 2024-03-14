using MiniGolf.Managers.Game;
using Mirror;
using UnityEngine;

namespace MiniGolf.Network
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class RoomDataSync : NetworkBehaviour
    {
        public static RoomDataSync instance;

        [SyncVar]
        public int courseIndex;

        private void Awake()
        {
            if (instance && instance != this)
            {
                Debug.LogError($"Multiple {nameof(RoomDataSync)}s loaded");
                Destroy(gameObject);
                return;
            }

            instance = this;
            GameManager.instance.OnSelectedCourseChange.AddListener(UpdateCourse);

            DontDestroyOnLoad(gameObject);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            UpdateCourse();
        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
            if (GameManager.instance) GameManager.instance.OnSelectedCourseChange.RemoveListener(UpdateCourse);
        }

        // Sends local selected index for server command
        private void UpdateCourse() => CmdUpdateCourse(GameManager.instance.SelectedIndex);

        [Command]
        private void CmdUpdateCourse(int courseIndex)
        {
            this.courseIndex = courseIndex;
        }
    }
}