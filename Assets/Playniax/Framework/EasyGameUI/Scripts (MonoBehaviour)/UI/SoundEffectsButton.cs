namespace Playniax.Ignition
{
    public class SoundEffectsButton : Rotator
    {
        public override void OnClick(int state)
        {
            AudioProperties.mute = state != 0;
        }

        public void OnEnable()
        {
            if (states[state]) targetGraphic.sprite = states[state];
        }
    }
}