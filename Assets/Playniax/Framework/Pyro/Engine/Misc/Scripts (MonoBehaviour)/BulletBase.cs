using UnityEngine;

namespace Playniax.Pyro
{
    [DefaultExecutionOrder(500)]
    [AddComponentMenu("Playniax/Pyro/BulletBase")]
    public class BulletBase : MonoBehaviour
    {
        public Vector3 velocity;

        void Update()
        {
            transform.position += velocity * Time.deltaTime;
        }
    }
}