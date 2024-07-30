using UnityEngine;
using UnityEngine.EventSystems;

namespace Playniax.Ignition.UI
{
    // This class handles clicking on links to open them in a web browser.
    public class Link : MonoBehaviour, IPointerClickHandler
    {
        // The URL of the link to open.
        [Tooltip("The URL of the link to open.")]
        public string link;

        // Triggered when the object is clicked on.
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            // Open the URL in a web browser.
            Application.OpenURL(link);
        }
    }
}
