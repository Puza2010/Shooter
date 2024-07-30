using UnityEngine;
using Playniax.Ignition;

public class AudioSourceHelper_Example : MonoBehaviour
{
    // Ignition sound object.
    public AudioProperties audioProperties;

    void Start()
    {
        // State to console.
        Debug.Log(AudioSourceHelper.mute ? "off" : "on");
    }
    public void Play()
    {
        // Play sound
        audioProperties.Play();
    }

    public void Mute()
    {
        // Toggle sound
        AudioSourceHelper.mute = !AudioSourceHelper.mute;

        // State to console.
        Debug.Log(AudioSourceHelper.mute ? "off" : "on");
    }
}
