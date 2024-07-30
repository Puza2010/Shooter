using System.Collections.Generic;
using UnityEngine;

namespace Playniax.Ignition
{
    // Collection of list functions.
    public class ListHelpers
    {
        // Copy List.
        public static List<T> Copy<T>(List<T> source)
        {
            List<T> output = new List<T>();

            for (int i = 0; i < source.Count; ++i)
                output.Add(source[i]);

            return output;
        }

        // Shuffle list.
        public static List<T> Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                // Randomize a number between 0 and i (range decreases each time).
                T value = list[i];
                // New random position.
                int position = Random.Range(0, i);
                // Swap the values.
                list[i] = list[position];
                list[position] = value;
            }

            return list;
        }
    }
}