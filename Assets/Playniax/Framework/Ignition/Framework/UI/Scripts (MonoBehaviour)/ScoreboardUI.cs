using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition.UI
{
    // Displays the score.
    public class ScoreboardUI : MonoBehaviour
    {
        [Tooltip("The index of the player.")]
        public int playerIndex;

        [Tooltip("The format of the text.")]
        public int format = 8;

        [Tooltip("Prefix to be added before the text.")]
        public string prefix;

        [Tooltip("Suffix to be added after the text.")]
        public string suffix;

        [Tooltip("Reference to the Text component for displaying the text.")]
        public Text text;

        [Tooltip("Flag indicating whether to update the text. Default is true.")]
        public bool update = true;
        
        void Awake()
        {
            if (text == null) text = GetComponent<Text>();
        }
        void OnEnable()
        {
            _Update();
        }
        void Update()
        {
            if (update) _Update();
        }
        void _Update()
        {
            if (text == null) return;

            string count = PlayerData.Get(playerIndex).scoreboard.ToString();

            if (format > 0) count = StringHelpers.Format(count, format);

            if (prefix != "") count = prefix + count;
            if (suffix != "") count += suffix;

            text.text = count;
        }
    }
}