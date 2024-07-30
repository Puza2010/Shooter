using UnityEngine;

namespace Playniax.Ignition
{
    // A collection of debugging functions.
    public class Debugging
    {
        // Measures time in milliseconds. MeasureTime() should always be called in pairs. The first time MeasureTime() is called, it stores the current time, and the second time MeasureTime() is called, it shows the difference in the console. Typically, MeasureTime() is called before and after a function to measure the execution time in milliseconds.
        public static void MeasureTime()
        {
            if (_time == -1)
            {
                _time = Time.deltaTime;
            }
            else
            {
                float time = Time.deltaTime - _time;

                _time = -1;

                if (time > 0) Debug.Log(time);
            }
        }

        static float _time = -1;
    }
}
