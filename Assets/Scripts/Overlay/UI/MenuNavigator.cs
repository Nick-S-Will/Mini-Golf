using System.Linq;
using UnityEngine;

namespace MiniGolf.Overlay.UI
{
    public class MenuNavigator : MonoBehaviour
    {
        [SerializeField] private GameObject[] subMenus;

        public void ToggleSubMenu(GameObject menu)
        {
            if (!subMenus.Contains(menu))
            {
                Debug.LogError($"Menu '{menu.name}' not in {nameof(subMenus)} array.");
                return;
            }

            foreach (var menuObject in subMenus)
            {
                if (menu == menuObject) menu.SetActive(!menu.activeSelf);
                else menuObject.SetActive(false);
            }
        }
    }
}