using UnityEngine;

namespace Playniax.Ignition
{
    [System.Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;

        public float GetRandom()
        {
            return Random.Range(min, max);
        }

        public FloatRange(float constant)
        {
            min = constant;
            max = constant;
        }
        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
