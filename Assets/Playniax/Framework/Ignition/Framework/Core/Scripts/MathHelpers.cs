using UnityEngine;

namespace Playniax.Ignition
{
    // Collection of math functions.
    public class MathHelpers
    {
        public static string Dif(float current, float increase, string suffix = "%")
        {
            float increaseInPercentage = (current + increase - current) / current * 100;

            return increaseInPercentage.ToString("F0") + suffix;
        }
        public static Vector3 GetVelocity(Vector3 origin, Vector3 target)
        {
            float x = target.x - origin.x;
            float z = target.z - origin.z;
            float y = target.y - origin.y;

            return new Vector3(x, y, z);
        }

        // Calculates the modulus (remainder) of a division operation between two floating-point numbers. It ensures that the result always remains within the range of [0, x).
        public static float Mod(float n, float x)
        {
            return ((n % x) + x) % x;
        }
    }
}