using UnityEngine;

namespace Playniax.Pyro
{
    public class LayerHelper : MonoBehaviour
    {
        public int layer;

        void Start()
        {
            gameObject.layer = layer;
        }

        void LateUpdate()
        {
            if (layer != _layer) gameObject.layer = layer;

            _layer = layer;
        }

        int _layer;
    }
}