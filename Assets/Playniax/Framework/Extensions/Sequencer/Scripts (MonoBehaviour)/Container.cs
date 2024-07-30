using UnityEngine;

namespace Playniax.Sequencer
{
    public class Container : SequenceBase
    {
        public Transform parent;
        public bool dontWait;
        public override void OnSequencerAwake()
        {
            ProgressCounter.Add(transform.childCount);
        }

        public override void OnSequencerUpdate()
        {
            if (state == 1)
            {
                foreach (Transform child in transform)
                    child.gameObject.AddComponent<ProgressCounter>();

                state = 2;
            }
            
            if (state == 2)
            {
                if (dontWait)
                {
                    foreach (Transform child in transform)
                        child.transform.parent = parent;
                }

                if (transform.childCount == 0) enabled = false;
            }
        }
    }
}