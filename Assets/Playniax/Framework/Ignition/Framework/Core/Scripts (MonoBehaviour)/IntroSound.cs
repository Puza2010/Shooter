using UnityEngine;

namespace Playniax.Ignition
{
    [AddComponentMenu("Playniax/Ignition/IntroSound")]
    // Plays a specified sound when the GameObject is enabled.
    public class IntroSound : MonoBehaviour
    {
        [Tooltip("Sound to play when OnEnable is called.")]
        public AudioProperties audioProperties;

        void OnEnable()
        {
            _audioProperties = audioProperties;
        }

        void Update()
        {
            if(_audioProperties != null && AudioSourceHelper.instance != null)
            {
                _audioProperties.Play();

                _audioProperties = null;
            }
        }

        AudioProperties _audioProperties;
    }
}