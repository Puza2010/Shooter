using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.Sequencer
{
    public class MessageSkip : MonoBehaviour
    {
        void Update()
        {
            if (Input.anyKeyDown)
            {
                if (EasyGameUI.instance)
                {
                    EasyGameUI.instance.effects.messenger.gameObject.SetActive(false);
                }
                else if (TextEffect.current && TextEffect.current.gameObject)
                {
                    Destroy(TextEffect.current.gameObject);
                }
                else if (Messenger.instance.queue.Count > 0)
                {
                    for (int i = 0; i < Messenger.instance.queue.Count; i++)
                        Destroy(Messenger.instance.queue[i]);

                    Messenger.instance.queue.Clear();
                }
            }
        }
    }
}