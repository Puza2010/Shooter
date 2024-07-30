using UnityEngine;
using Playniax.Ignition;

public class ArrayHelpers_Example : MonoBehaviour
{
    // Custom type.
    public class MyType
    {
        public string name = "Unknown";
    }
    void Start()
    {
        // Create types.
        MyType myType1 = new MyType();
        MyType myType2 = new MyType();
        MyType myType3 = new MyType();

        // Fill types with data.
        myType1.name = "Tony";
        myType2.name = "Tanya";
        myType3.name = "David";

        // Declare the array.
        MyType[] list = null;

        // Add the types to the array.
        list = ArrayHelpers.Add(list, myType1);
        list = ArrayHelpers.Add(list, myType2);
        list = ArrayHelpers.Insert(list, myType3);

        // Shuffle the array.
        list = ArrayHelpers.Shuffle(list);

        // Show results.
        for (int i = 0; i < list.Length; i++)
        {
            Debug.Log(list[i].name);
        }
    }
}
