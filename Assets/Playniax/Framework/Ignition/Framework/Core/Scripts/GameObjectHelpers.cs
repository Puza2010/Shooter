using UnityEngine;

namespace Playniax.Ignition
{
    // A collection of functions for GameObjects.
    public class GameObjectHelpers
    {
        // Activates or Deactivates the children in the GameObject, depending on the given true or false value.
        public static void SetActiveChildren(GameObject parent, bool value)
        {
            int children = parent.transform.childCount;

            for (int i = 0; i < children; ++i)
                parent.transform.GetChild(i).gameObject.SetActive(value);
        }
    }
}