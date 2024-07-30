using UnityEngine;

namespace Playniax.Ignition
{
    public class MusicButton : Rotator
    {
        public override void OnClick(int state)
        {
            if (EasyGameUI.instance)
            {
                PlayerPrefs.SetInt("musicOff", state);

                MusicPlayer.PlayAll(state == 0 ? true : false);
            }
        }

        public void OnEnable()
        {
            if (EasyGameUI.instance)
            {
                state = PlayerPrefs.GetInt("musicOff");

                if (states[state]) targetGraphic.sprite = states[state];
            }
        }
    }
}