using MiniGolf.UI;
using TMPro;
using UnityEngine;

namespace MiniGolf.Network.UI
{
    public class PlayerNameDisplay : Display<PlayerScore>
    {
        [SerializeField] private TMP_Text nameText;

        private void Update()
        {
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            gameObject.SetActive(displayObject);
            if (displayObject == null) return;
            
            transform.position = displayObject.transform.position;
            transform.forward = Vector3.ProjectOnPlane(transform.position - Camera.main.transform.position, Vector3.up);
        }

        public override void UpdateText()
        {
            if (displayObject == null) return;

            nameText.text = displayObject.Name;
        }
    }
}