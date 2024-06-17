using UnityEngine;

namespace MiniGolf.Overlay.UI
{
    public class VideoOptionsMenu : MonoBehaviour
    {
        public void SetFullscreen(bool fullscreen) => Screen.fullScreen = fullscreen;
    }
}