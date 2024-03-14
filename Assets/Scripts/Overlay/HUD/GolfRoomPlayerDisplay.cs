using MiniGolf.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Overlay.HUD
{
    public class GolfRoomPlayerDisplay : Display<GolfRoomPlayer>
    {
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private Toggle readyToggle;

        public override void UpdateText()
        {
            var hasPlayer = displayObject != null;
            playerName.text = hasPlayer ? displayObject.Name : string.Empty;
            readyToggle.isOn = hasPlayer ? displayObject.readyToBegin : false;
        }
    }
}