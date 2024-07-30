using UnityEngine;

namespace Playniax.Pyro
{
    public class Dissolver : MonoBehaviour
    {
        public float ttl = 10;
        public float speed = 1;

        void Update()
        {
            if (ttl > 0)
            {
                ttl -= Time.deltaTime;
            }
            else
            {
                var scale = transform.localScale;

                scale -= speed * Vector3.one * Time.deltaTime;

                if (scale.x <= 0 || scale.y <= 0 || scale.z <= 0)
                {
                    scale = Vector3.zero;

                    transform.localScale = scale;

                    Destroy(gameObject);
                }
                else
                {
                    transform.localScale = scale;
                }
            }
        }
    }
}