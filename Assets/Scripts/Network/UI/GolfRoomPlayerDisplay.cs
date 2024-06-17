using Displayable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGolf.Network.UI
{
    public class GolfRoomPlayerDisplay : Display<GolfRoomPlayer>
    {
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private Toggle readyToggle;

        public override void SetObject(GolfRoomPlayer newObject)
        {
            if (displayObject) displayObject.OnReadyChanged.RemoveListener(UpdateText);
            if (newObject) newObject.OnReadyChanged.AddListener(UpdateText);

            base.SetObject(newObject);
        }

        public override void UpdateText()
        {
            var hasPlayer = displayObject != null;
            playerName.text = hasPlayer ? displayObject.Name : string.Empty;
            readyToggle.isOn = hasPlayer ? displayObject.readyToBegin : false;
        }
    }
}