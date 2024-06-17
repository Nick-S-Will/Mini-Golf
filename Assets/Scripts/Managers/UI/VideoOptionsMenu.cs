using UnityEngine;

namespace MiniGolf.Managers.UI
{
    public class VideoOptionsMenu : MonoBehaviour
    {
        public void SetFullscreen(bool fullscreen) => Screen.fullScreen = fullscreen;
    }
}