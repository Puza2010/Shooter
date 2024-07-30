using UnityEngine;

namespace Playniax.Ignition
{
    [AddComponentMenu("Playniax/Ignition/Audio Source Helper")]
    public class AudioSourceHelper : MonoBehaviour
    {

        // Global variable to enable or disable sound.
        public static bool mute = false;

        public int channels = 3;

        // Returns the first available channel.
        public static AudioSourceHelper instance
        {
            get
            {
                if (_instance == null)
                {
                    if (_audioListener == null && Camera.main)
                    {
                        _audioListener = FindObjectOfType<AudioListener>();

                        if (_audioListener == null) _audioListener = Camera.main.gameObject.AddComponent<AudioListener>();
                    }

                    _instance = FindObjectOfType<AudioSourceHelper>();

                    if (_instance == null)
                    {
                        _instance = new GameObject("Audio Source Helper").AddComponent<AudioSourceHelper>();
                        _instance._Init();
                    }
                    else
                    {
                        if (_instance._channels == null) _instance._channels = _instance.GetComponents<AudioSource>();

                        if (_instance._channels.Length == 0) _instance._Init();
                    }
                }

                return _instance;
            }
        }

        public AudioSource GetAvailableChannel()
        {
            if (_channels == null) _channels = FindObjectsOfType<AudioSource>();
            if (_channels == null) return null;
            if (_channels.Length == 0) return null;

            for (int i = 0; i < _channels.Length; i++)
                if (_channels[i] && _channels[i].isPlaying == false) return _channels[i];

            return null;
        }

        // Play AudioClip using the first available channel.
        public AudioSource Play(AudioClip audioClip, float volumeScale = 1f, float panStereo = 0, float pitch = 1)
        {
            if (volumeScale == 0) return null;
            if (audioClip == null) return null;
            if (mute) return null;

            AudioSource channel = GetAvailableChannel();

            if (channel != null)
            {
                channel.panStereo = panStereo;
                channel.pitch = pitch;
                channel.PlayOneShot(audioClip, volumeScale);
            }

            return channel;
        }

        void OnDisable()
        {
            if (_channels != null && _channels.Length > 0) _channels = null;

            _instance = null;
        }

        void _Init()
        {
            _channels = new AudioSource[channels];

            for (int i = 0; i < _channels.Length; i++)
                _channels[i] = gameObject.AddComponent<AudioSource>();
        }

        static AudioListener _audioListener;

        static AudioSourceHelper _instance;

        AudioSource[] _channels;
    }
}