using UnityEngine;

namespace Playniax.Sequencer
{
    public class Repeater : SequenceBase
    {
        public GameObject[] prefab;
        public int repeats = 1;
        public override void OnSequencerInit()
        {
            for (int r = 0; r < repeats; r++)
            {
                for (int i = 0; i < prefab.Length; i++)
                {
                    Instantiate(prefab[i], transform.position, Quaternion.identity, transform);
                }
            }
        }

        public override void OnSequencerUpdate()
        {
            enabled = false;
        }
    }
}