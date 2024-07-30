using UnityEngine;

namespace Playniax.Ignition
{
    // Collection of array functions.
    public class ArrayHelpers
    {
        // Adds value to array and increases size and count.
        public static int Add<T>(ref T[] array, ref int count, T value)
        {
            if (count == array.Length) System.Array.Resize(ref array, array.Length * 2 + 10);

            array[count++] = value;

            return count - 1;
        }

        // Returns a new array with added value.
        public static T[] Add<T>(T[] array, T value)
        {
            if (array == null) array = new T[0];
            T[] result = new T[array.Length + 1];
            array.CopyTo(result, 0);
            result[array.Length] = value;
            return result;
        }

        // Returns a new array with inserted value.
        public static T[] Insert<T>(T[] array, T value)
        {
            if (array == null) array = new T[0];
            T[] result = new T[array.Length + 1];
            array.CopyTo(result, 1);
            result[0] = value;
            return result;
        }

        // Returns a new array with both arrays merged.
        public static T[] Merge<T>(T[] array1, T[] array2)
        {
            T[] result = new T[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);
            return result;
        }

        // Returns a new array with the first value removed.
        public static T[] Skim<T>(T[] array)
        {
            if (array.Length < 1) return array;
            T[] result = new T[array.Length - 1];
            System.Array.Copy(array, 1, result, 0, result.Length);
            return result;
        }

        // Shuffle array.
        public static T[] Shuffle<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                // Randomize a number between 0 and i (range decreases each time).
                T value = array[i];
                // New random position.
                int position = Random.Range(0, i);
                // Swap the values.
                array[i] = array[position];
                array[position] = value;
            }

            return array;
        }
    }
}
