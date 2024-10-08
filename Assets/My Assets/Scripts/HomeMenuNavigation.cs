using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Playniax.Ignition
{
    public class HomeMenuNavigation : MonoBehaviour
    {
        [Header("Menu Items")]
        // Assign these in the Inspector in the order: Play, Music Toggle, Sound Effects Toggle, About
        public List<GameObject> menuItems = new List<GameObject>();

        [Header("Text Scaling Settings")]
        public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1.2f); // Scale when selected
        public Vector3 normalScale = Vector3.one; // Normal scale

        [Header("Color Settings")]
        public Color selectedColor = Color.red; // Color when selected
        public Color normalColor = Color.white; // Normal color

        private int currentIndex = 0; // Tracks the currently selected menu item

        void Start()
        {
            if (menuItems.Count == 0)
            {
                Debug.LogError("No menu items assigned in HomeMenuNavigation.");
                return;
            }

            // Ensure all menu items have EventTrigger and TMP Text components
            foreach (GameObject item in menuItems)
            {
                if (item == null)
                {
                    Debug.LogError("One of the menu items is not assigned.");
                    return;
                }

                EventTrigger trigger = item.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    Debug.LogError($"Menu item '{item.name}' does not have an EventTrigger component.");
                    return;
                }

                TMP_Text text = GetMenuItemText(item);
                if (text == null)
                {
                    Debug.LogError($"Menu item '{item.name}' does not have a TMP Text component as a child.");
                    return;
                }
            }

            // Select the first menu item by default
            SelectMenuItem(currentIndex);
        }

        void Update()
        {
            HandleInput();
        }

        /// <summary>
        /// Handles keyboard inputs for navigation and selection.
        /// </summary>
        void HandleInput()
        {
            // Navigate Up
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex--;
                if (currentIndex < 0)
                    currentIndex = menuItems.Count - 1; // Wrap around to the last menu item

                SelectMenuItem(currentIndex);
            }

            // Navigate Down
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex++;
                if (currentIndex >= menuItems.Count)
                    currentIndex = 0; // Wrap around to the first menu item

                SelectMenuItem(currentIndex);
            }

            // Select the current menu item
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ActivateMenuItem(currentIndex);
            }
        }

        /// <summary>
        /// Selects a menu item at the specified index, updates its visual state.
        /// </summary>
        /// <param name="index">Index of the menu item to select.</param>
        void SelectMenuItem(int index)
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                GameObject item = menuItems[i];
                TMP_Text text = GetMenuItemText(item);

                if (text != null)
                {
                    if (i == index)
                    {
                        // Selected menu item: enlarge the text and change color to selectedColor
                        text.transform.localScale = selectedScale;
                        text.color = selectedColor;
                    }
                    else
                    {
                        // Unselected menu items: reset to normal scale and color
                        text.transform.localScale = normalScale;
                        text.color = normalColor;
                    }
                }
            }
        }

        /// <summary>
        /// Activates the menu item's click event.
        /// </summary>
        /// <param name="index">Index of the menu item to activate.</param>
        void ActivateMenuItem(int index)
        {
            if (index < 0 || index >= menuItems.Count)
            {
                Debug.LogError("Invalid menu item index.");
                return;
            }

            GameObject selectedItem = menuItems[index];
            EventTrigger trigger = selectedItem.GetComponent<EventTrigger>();

            if (trigger == null)
            {
                Debug.LogError($"Menu item '{selectedItem.name}' does not have an EventTrigger component.");
                return;
            }

            // Execute the PointerClick event
            ExecuteEventClick(selectedItem);
        }

        /// <summary>
        /// Executes the PointerClick event on the specified GameObject.
        /// </summary>
        /// <param name="target">The GameObject to trigger the click event on.</param>
        void ExecuteEventClick(GameObject target)
        {
            // Create a PointerEventData instance
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            // Execute the event
            ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerClickHandler);
        }

        /// <summary>
        /// Retrieves the TMP Text component from a menu item's child.
        /// </summary>
        /// <param name="menuItem">The menu item GameObject.</param>
        /// <returns>The TMP Text component if found; otherwise, null.</returns>
        TMP_Text GetMenuItemText(GameObject menuItem)
        {
            // Get all TMP_Text components in children
            TMP_Text[] texts = menuItem.GetComponentsInChildren<TMP_Text>();

            if (texts.Length > 0)
            {
                // Optionally, log which TMP_Text component is found
                // Debug.Log($"Menu item '{menuItem.name}' has TMP_Text component: '{texts[0].name}'");
                return texts[0]; // Return the first TMP_Text found
            }
            else
            {
                Debug.LogError($"Menu item '{menuItem.name}' does not have a TMP Text component as a child.");
                return null;
            }
        }
    }

    /// <summary>
    /// Extension method to find a child by name recursively.
    /// </summary>
    public static class TransformExtensions
    {
        public static Transform FindDeepChild(this Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;
                Transform result = child.FindDeepChild(name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
