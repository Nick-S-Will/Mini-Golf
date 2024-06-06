using System.Linq;
using UnityEngine;

namespace MiniGolf.Overlay.UI
{
    public class MenuNavigator : MonoBehaviour
    {
        [SerializeField] private GameObject[] subMenus;

        private GameObject selectedMenu;

        public void ToggleSubMenu(GameObject menu)
        {
            if (!subMenus.Contains(menu))
            {
                Debug.LogError($"Menu '{menu.name}' not in {nameof(subMenus)} array.");
                return;
            }

            foreach (var menuObject in subMenus) menuObject.SetActive(false);

            if (menu == selectedMenu) selectedMenu = null;
            else
            {
                menu.SetActive(true);
                selectedMenu = menu;
            }
        }
    }
}