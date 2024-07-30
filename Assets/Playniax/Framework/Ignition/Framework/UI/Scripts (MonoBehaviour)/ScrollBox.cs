using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition.UI
{
    // The scrollbox supports text, url links and images and it has its own scripting language. 
    public class ScrollBox : MonoBehaviour
    {
        [Tooltip("The external script file to use for content generation.")]
        public TextAsset externalScript;

        [Tooltip("Flag indicating whether to use an external script for content generation.")]
        public bool useExternalScript;

        [Tooltip("The custom script to use for content generation.")]
        [TextArea(15, 20)]
        public string script;

        [Tooltip("The starting position within the scroll box.")]
        public string start;

        [Tooltip("The character used to break data fields within the script.")]
        public string dataBreak = "&";

        [Tooltip("The character used to break lines within the script.")]
        public string lineBreak = "|";

        [Tooltip("Flag indicating whether to convert text to all capital letters.")]
        public bool allCaps = false;

        [Tooltip("The asset bank to use for content generation.")]
        public AssetBank assetBank;

        [Tooltip("The container for scroll box content.")]
        public GameObject content;

        [Tooltip("The height of the scroll box content.")]
        public float contentHeight;

        // Whether the mouse is hovering the scrollbox or not.
        public bool isMouseOver
        {
            get
            {
                Vector3 mousePosition = _rectTransform.InverseTransformPoint(Input.mousePosition);
                if (_rectTransform.rect.Contains(mousePosition)) return true;
                return false;
            }
        }

        // Returns GameObject if it contains value.
        public GameObject Contains(string value)
        {
            foreach (Transform child in content.transform)
                if (child.name.Contains(value)) return child.gameObject;

            return null;
        }

        // Sets the scrollbox position by value.
        public void SetPosition(string value)
        {
            GameObject child = Contains(value);
            if (child == null) return;

            Vector3 position = new Vector3(child.transform.localPosition.x, -child.transform.localPosition.y, child.transform.localPosition.z);

            position.y += _rectTransform.sizeDelta.y * .5f;
            position.y -= child.GetComponent<Text>().fontSize * .5f;

            content.transform.localPosition = position;
        }

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            content = new GameObject("Content");

            content.transform.SetParent(transform, false);
        }

        void Start()
        {
            if (useExternalScript && externalScript)
            {
                contentHeight = PageEngine.ExecuteScript(content.transform, _rectTransform.sizeDelta, externalScript.text, assetBank, allCaps, dataBreak, lineBreak);
            }
            else
            {
                contentHeight = PageEngine.ExecuteScript(content.transform, _rectTransform.sizeDelta, script, assetBank, allCaps, dataBreak, lineBreak);
            }

            if (start != "") SetPosition(start);
        }

        RectTransform _rectTransform;
    }
}