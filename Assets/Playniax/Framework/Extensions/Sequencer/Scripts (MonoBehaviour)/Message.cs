using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.Sequencer
{
    public class Message : SequenceBase
    {
        public string messengerId = "Generic";
        [Multiline]
        [Tooltip("The message to display.")]
        public string text = "Get Ready For Wave %WAVE%";
        [Tooltip("Time to wait before fading")]
        public float sustain = 1;
        [Tooltip("Fading display time")]
        public float fadeTime = 1.5f;
        public float scaleStep;
        [Tooltip("Font")]
        public Font font;
        [Tooltip("Font size")]
        public int fontSize = 32;
        [Tooltip("Font color")]
        public Color fontColor = Color.white;
        public bool dontWait = true;
        public override void OnSequencerUpdate()
        {
            if (state == 1)
            {
                var display = _Fetch(text);

                if (sequencer && display.Contains("%WAVE%"))
                {
                    display = display.Replace("%WAVE%", sequencer.wave.ToString());

                    sequencer.wave += 1;
                }

                if (EasyGameUI.instance != null)
                {
                    EasyGameUI.instance.effects.Message(font, fontSize, fontColor, display, sustain, scaleStep);
                }
                else
                {
                    Messenger.instance.Create(messengerId, display, Vector3.zero);
                }

                if (dontWait == true) enabled = false; else state = 2;
            }
            else if (state == 2)
            {
                if (EasyGameUI.instance)
                {
                    if (EasyGameUI.instance.isMessengerBusy == false) enabled = false;
                }
                else
                {
                    if (Messenger.instance.queue.Count == 0) enabled = false;
                }
            }
        }

        string _Fetch(string text)
        {
            if (EasyGameUI.instance)
            {
                text = text.Replace("%LEVEL%", (EasyGameUI.instance.levelIndex + 1).ToString());
            }
            else
            {
                text = text.Replace("%LEVEL%", "0");
            }

            return text;
        }
    }
}