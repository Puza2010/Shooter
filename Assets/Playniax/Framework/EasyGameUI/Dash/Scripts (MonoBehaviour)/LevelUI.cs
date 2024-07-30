using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition
{
    public class LevelUI : MonoBehaviour
    {
        public int format;
        public string prefix;
        public string suffix;
        public Text text;

        void Awake()
        {
            if (text == null) text = GetComponent<Text>();
        }

        void Update()
        {
            if (EasyGameUI.instance == null)
            {
                text.text = prefix + " 0 " + suffix;

                return;
            }

            var count = (EasyGameUI.instance.levelIndex + 1).ToString();

            if (format > 0) count = StringHelpers.Format(count, format);

            if (prefix != "") count = prefix + count;
            if (suffix != "") count += suffix;

            text.text = count;
        }
    }
}