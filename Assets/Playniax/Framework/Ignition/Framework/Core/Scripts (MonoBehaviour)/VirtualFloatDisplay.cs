using UnityEngine;

namespace Playniax.Ignition
{

    public class VirtualFloatDisplay : MonoBehaviour
    {
        public string key = "My Float Value";
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

            var value = VirtualFloats.instance.Get(key, 0);

            text.text = value.ToString();
        }
    }
}