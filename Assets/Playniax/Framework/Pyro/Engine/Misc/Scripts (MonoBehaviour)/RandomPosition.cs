using UnityEngine;

namespace Playniax.Pyro
{
    public class RandomPosition : MonoBehaviour
    {
        // The range in which the random position will be generated
        [Tooltip("The range in which the random position will be generated for each axis.")]
        public Vector3 range;

        void Start()
        {
            // Generate random values within the specified range for each axis
            float x = Random.Range(-range.x, range.x);
            float y = Random.Range(-range.y, range.y);
            float z = Random.Range(-range.z, range.z);

            // Set the object's position to the generated random position
            transform.position = new Vector3(x, y, z);
        }
    }
}
