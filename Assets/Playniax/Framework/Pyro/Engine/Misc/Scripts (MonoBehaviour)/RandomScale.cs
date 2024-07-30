using UnityEngine;

namespace Playniax.Pyro
{
    public class RandomScale : MonoBehaviour
    {
        [Tooltip("Minimum scale value.")]
        public float minScale = 1;

        [Tooltip("Maximum scale value.")]
        public float maxScale = 1.5f;

        void Start()
        {
            // Generate a random scale factor within the specified range
            float scaleFactor = Random.Range(minScale, maxScale);

            // Ensure the scale factor is not zero
            scaleFactor = Mathf.Max(scaleFactor, Mathf.Epsilon);

            // Apply the random scale factor to all local scale components
            transform.localScale *= scaleFactor;
        }
    }
}
