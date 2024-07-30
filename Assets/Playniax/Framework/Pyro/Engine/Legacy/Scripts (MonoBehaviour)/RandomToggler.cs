using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class RandomToggler : MonoBehaviour
    {
        public Object obj;
        public Timer timer;
        void Awake()
        {
            /*
            var gameObject = obj as GameObject;
            var component = obj as MonoBehaviour;

            if (gameObject && gameObject.scene.rootCount > 0) gameObject.SetActive(false); else if (component) component.enabled = false;
            */
        }

        void Update()
        {
            if (timer.Update())
            {
                var gameObject = obj as GameObject;
                var component = obj as MonoBehaviour;

                if (component)
                {
                    component.enabled = !component.enabled;
                }
                else if (gameObject && gameObject.scene.rootCount > 0)
                {
                    gameObject.SetActive(!gameObject.activeSelf);
                }
            }
        }
    }

}