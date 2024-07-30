using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition.UI
{
    // Displays game progress based on spawner and sequencer state.
    public class GameProgressUI : MonoBehaviour
    {
        [Tooltip("The format of the text.")]
        public int format;

        [Tooltip("Prefix to be added before the text.")]
        public string prefix;

        [Tooltip("Suffix to be added after the text. Default is '%'.")]
        public string suffix = "%";

        [Tooltip("Reference to the Text component for displaying the text.")]
        public Text text;
        void Awake()
        {
            if (text == null) text = GetComponent<Text>();
        }

        void Update()
        {
            if (text == null) return;

            if (GameData.progressScale == 0) return;

            string count = "0";

            if (GameData.progressScale > 0) count = ((int)(100f - ((float)GameData.progress / GameData.progressScale * 100))).ToString();

            if (format > 0) count = StringHelpers.Format(count, format);

            if (prefix != "") count = prefix + count;
            if (suffix != "") count += suffix;

            text.text = count;
        }
    }
}