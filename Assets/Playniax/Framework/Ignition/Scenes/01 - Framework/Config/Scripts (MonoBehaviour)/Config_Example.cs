using UnityEngine;
using Playniax.Ignition;

public class Config_Example : MonoBehaviour
{
    void Start()
    {
        // Create Config object.
        Config data = new Config();

        // Fill with data.
        data.SetString("name", "Tony");
        data.SetInt("score", 100);

        // Get data.
        string output = "Player " + data.GetString("name") + " has scored " + data.GetInt("score") + " points.";

        Debug.Log(output);
    }
}
