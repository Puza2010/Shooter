using UnityEngine;

namespace Playniax.Ignition
{
    public class MusicPlayerHelper : MonoBehaviour
    {
        public AudioClip audioClip;
        public bool loop = true;
        public bool dontDestroyOnLoad;
        void Start()
        {
            if (audioClip)
            {
                var musicPlayer = FindObjectOfType<MusicPlayer>();
                if (musicPlayer == null)
                {
                    musicPlayer = new GameObject("Music Player").AddComponent<MusicPlayer>();
                    musicPlayer.audioClip = audioClip;
                    musicPlayer.loop = loop;

                    if (dontDestroyOnLoad) DontDestroyOnLoad(musicPlayer.gameObject);
                }
            }
        }
    }
}