using UnityEngine;
using UnityEngine.UI;
using Playniax.Ignition;

namespace Playniax.Pyro.UI
{
    public class StructuralIntegrityUI : MonoBehaviour
    {
        public string id = "Player 1";
        public int format;
        public string prefix;
        public string suffix = "%";
        public Text text;
        public int maximum;
        public bool selectedPlayerOnly = true;
        void Awake()
        {
            if (text == null) text = GetComponent<Text>();
        }
        void Update()
        {
            if (text == null) return;

            if (maximum == 0) maximum += _GetStructuralIntegrity(id, selectedPlayerOnly);
            if (maximum == 0) return;

            var count = "0";

            if (maximum > 0) count = ((int)((float)_GetStructuralIntegrity(id, selectedPlayerOnly) / maximum * 100)).ToString();

            if (format > 0) count = StringHelpers.Format(count, format);

            if (prefix != "") count = prefix + count;
            if (suffix != "") count += suffix;

            text.text = count;
        }
        int _GetStructuralIntegrity(string id, bool selectedPlayerOnly)
        {
            float count = 0;

            var list = PlayerGroup.GetList();
            if (list == null) return 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] && list[i].id == id && list[i].gameObject)
                {
                    //if (selectedPlayerOnly && list[i] && list[i].gameObject != PlayerGroup.GetSelected()) continue;

                    var scoreBase = list[i].GetComponent<IScoreBase>();
                    if (scoreBase != null) count += scoreBase.structuralIntegrity;
                }
            }

            return (int)count;
        }
    }
}