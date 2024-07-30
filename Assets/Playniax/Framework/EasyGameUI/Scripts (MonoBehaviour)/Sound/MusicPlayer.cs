using UnityEngine;

namespace Playniax.Ignition
{
    // EasyGameUI MusicPlayer
    public class MusicPlayer : MonoBehaviour
    {
        public AudioClip audioClip;
        public AudioSource audioSource;
        public bool loop = true;

        // Play or stop all music.
        public static void PlayAll(bool state)
        {
            MusicPlayer[] musicPlayers = FindObjectsOfType<MusicPlayer>();

            for (int i = 0; i < musicPlayers.Length; i++)
                musicPlayers[i].Play(state);
        }

        // Play or stop music.
        public void Play(bool state)
        {
            if (state)
            {
                AudioListener audioListener = FindObjectOfType<AudioListener>();

                if (audioListener == null) gameObject.AddComponent<AudioListener>();

                audioSource = GetComponent<AudioSource>();

                if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

                if (PlayerPrefs.GetInt("musicOff") == 0 && audioSource && audioClip)
                {
                    audioSource.clip = audioClip;
                    audioSource.loop = loop;

                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource) audioSource.Stop();
            }
        }

        void Start()
        {
            Play(true);
        }
    }
}