using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition.UI
{
    // Displays the number of lives left.
    public class LivesUI : MonoBehaviour
    {
        [Tooltip("The index of the player.")]
        public int playerIndex;

        [Tooltip("The format of the text (e.g., font size, style).")]
        public int format;

        [Tooltip("Prefix to be added before the text.")]
        public string prefix;

        [Tooltip("Suffix to be added after the text.")]
        public string suffix;

        [Tooltip("Reference to the Text component for displaying the text.")]
        public Text text;
        
        void Awake()
        {
            if (text == null) text = GetComponent<Text>();
        }
        void Update()
        {
            if (text == null) return;

            string count = PlayerData.Get(playerIndex).lives.ToString();

            if (format > 0) count = StringHelpers.Format(count, format);

            if (prefix != "") count = prefix + count;
            if (suffix != "") count += suffix;

            text.text = count;
        }
    }
}