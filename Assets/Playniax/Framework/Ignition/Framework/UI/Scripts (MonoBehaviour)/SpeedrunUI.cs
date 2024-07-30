using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition.UI
{
    // Displays game running time.

    public class SpeedrunUI : MonoBehaviour
    {
        public Text text;

        void Awake()
        {
            if (text == null) text = GetComponent<Text>();

            _Update();
        }

        void Update()
        {
            _Update();
        }

        void _Update()
        {
            if (text == null) return;

            text.text = Speedrun.Get();
        }
    }
}