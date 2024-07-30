using UnityEngine;

namespace Playniax.Pyro
{
    public class LayerCollisions2D : MonoBehaviour
    {
        [System.Serializable]
        public class Setting
        {
            public int layer1;
            public int layer2;
            public bool ignore;
        }

        public Setting[] settings;

        void Start()
        {
            for (int i = 0; i < settings.Length; i++)
                Physics2D.IgnoreLayerCollision(settings[i].layer1, settings[i].layer2, settings[i].ignore);
        }
    }
}