using UnityEngine;

namespace Playniax.Ignition
{

    public class VirtualStringDisplay : MonoBehaviour
    {
        public string key = "My String Value";
        public TextMesh text;

        void Awake()
        {
            _Update();
        }

        void Update()
        {
            _Update();
        }

        void _Update()
        {
            if (text == null) text = gameObject.GetComponent<TextMesh>();
            if (text == null) return;

            if (key == null) return;

            if (key == "") return;

            var value = VirtualStrings.instance.Get(key, "");

            text.text = value;
        }
    }
}