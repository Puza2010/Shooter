using UnityEngine;

namespace Playniax.Sequencer
{
    public class Delay : SequenceBase
    {
        public float timer = 1;

        public override void OnSequencerUpdate()
        {
            timer -= Time.deltaTime;

            if (timer <= 0) enabled = false;
        }
    }
}
