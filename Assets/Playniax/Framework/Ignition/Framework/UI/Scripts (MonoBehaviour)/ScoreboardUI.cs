using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition.UI
{
    public class ScoreboardUI : MonoBehaviour
    {
        public int playerIndex;
        public int format = 8;
        public string prefix;
        public string suffix;
        public Text text;
        public bool update = true;

        private void Awake()
        {
            if (text == null) text = GetComponent<Text>();
        }

        private void OnEnable()
        {
            _Update();
        }

        private void Update()
        {
            if (update) _Update();
        }

        private void _Update()
        {
            if (text == null) return;

            string count = PlayerData.Get(playerIndex).scoreboard.ToString();

            if (format > 0) count = StringHelpers.Format(count, format);

            if (prefix != "") count = prefix + count;
            if (suffix != "") count += suffix;

            text.text = count;

            // Update Firebase every 10 seconds
            FirebaseScoreUpdater.Instance.UpdateScore(PlayerData.Get(playerIndex).scoreboard);
        }
    }
}
