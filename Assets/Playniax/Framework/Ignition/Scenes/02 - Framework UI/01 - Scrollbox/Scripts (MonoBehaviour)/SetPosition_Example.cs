using UnityEngine;
using UnityEngine.UI;
using Playniax.Ignition.UI;

public class SetPosition_Example : MonoBehaviour
{
    public string text;
    public Button button;
    public ScrollBoxSwipe scrollBoxSwipe;
    void Awake()
    {
        // Below some fancy code to assign the component variables:

        // Try to find ScrollBoxSwipe component in the scene.
        if (scrollBoxSwipe == null) scrollBoxSwipe = FindObjectOfType<ScrollBoxSwipe>();

        // Try to find the button.
        if (button == null) button = GetComponent<Button>();

        // Add a listener for detecting button click.
        if (button && scrollBoxSwipe && text != "") button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        // Stop scrolling.
        scrollBoxSwipe.Stop();
        // Set scroll position using the button text as keyword.
        if (scrollBoxSwipe.scrollBox) scrollBoxSwipe.scrollBox.SetPosition(text);
    }
}
