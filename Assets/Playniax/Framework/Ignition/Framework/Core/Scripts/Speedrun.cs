using System;
using UnityEngine;

namespace Playniax.Ignition
{
    public class Speedrun
    {
        public static float speedRun;

        public static string Get()
        {
            if (_unscaledTime == 0) _unscaledTime = Time.unscaledTime;

            TimeSpan time = TimeSpan.FromSeconds(speedRun);
            if (Time.timeScale > 0 && PlayerData.CountLives() > 0) speedRun += Time.unscaledTime - _unscaledTime;
            _unscaledTime = Time.unscaledTime;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        }

        public static void Reset()
        {
            speedRun = 0;
            _unscaledTime = 0;
        }

        static float _unscaledTime;
    }
}