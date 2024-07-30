using UnityEngine;

namespace Playniax.Ignition
{
    public class ObjectHelpers : MonoBehaviour
    {
        public static bool Safe(object obj)
        {
            if (obj == null || obj.Equals(null)) return false;

            return true;
        }

    }
}
