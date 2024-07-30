using UnityEngine;

namespace Playniax.Ignition
{
    [System.Serializable]
    // A convenient way to store and play audio data.
    public class AudioProperties
    {
        public static bool mute = false;

        [Tooltip("AudioClip to play.")]
        public AudioClip audioClip;
        [Tooltip("Sound volume.")]
        public float volumeScale = 1f;
        public float panStereo = 0;
        [Tooltip("Sound frequency.")]
        public float pitch = 1;
        [Tooltip("Sound on / off.")]
        public bool enabled = true;

        public static void Play(AudioProperties[] audioProperties)
        {
            for (int i = 0; i < audioProperties.Length; i++)
                audioProperties[i].Play();
        }

        // Copy the properties of one to the other.
        public static void Copy(AudioProperties source, AudioProperties target)
        {
            target.audioClip = source.audioClip;
            target.volumeScale = source.volumeScale;
            target.panStereo = source.panStereo;
            target.pitch = source.pitch;
            target.enabled = source.enabled;
        }

        // Returns a clone with the same properties.
        public AudioProperties Clone()
        {
            AudioProperties clone = new AudioProperties();

            clone.audioClip = audioClip;
            clone.volumeScale = volumeScale;
            clone.panStereo = panStereo;
            clone.pitch = pitch;
            clone.enabled = enabled;

            return clone;
        }

        // Play AudioClip.
        public AudioSource Play()
        {
            if (mute == false && enabled == true) return AudioSourceHelper.instance.Play(audioClip, volumeScale, panStereo, pitch);

            return null;
        }
    }
}
