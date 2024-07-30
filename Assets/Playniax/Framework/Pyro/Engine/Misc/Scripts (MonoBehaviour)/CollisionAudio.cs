using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    [AddComponentMenu("Playniax/Pyro/CollisionAudio")]
    // Determines what sound to play when 2 objects collide.
    public class CollisionAudio : MonoBehaviour
    {
        [Tooltip("Material 1.")]
        public string material1 = "Metal";
        [Tooltip("Material 2.")]
        public string material2 = "Metal";
        [Tooltip("Sound to play.")]
        public AudioProperties audioProperties;

        public static void Play(string material1, string material2)
        {
            if (_collisionAudio == null)
            {
                _collisionAudio = FindObjectsOfType<CollisionAudio>();

                if (_collisionAudio.Length == 0)
                {
                    var collisionAudio = Resources.Load("Prototyping/Collision Audio");

                    if (collisionAudio != null)
                    {
                        Instantiate(collisionAudio).name = "Collision Audio";

                        _collisionAudio = FindObjectsOfType<CollisionAudio>();
                    }
                }
            }

            for (int i = 0; i < _collisionAudio.Length; i++)
            {
                if (_collisionAudio[i].material1 != "" && _collisionAudio[i].material2 != "" && _collisionAudio[i].material1 + _collisionAudio[i].material2 == material1 + material2 || _collisionAudio[i].material1 + _collisionAudio[i].material2 == material2 + material1)
                {
                    _collisionAudio[i].audioProperties.Play();

                    return;
                }
            }
        }
        void OnDisable()
        {
            if (_collisionAudio != null && _collisionAudio.Length > 0) _collisionAudio = null;
        }

        static CollisionAudio[] _collisionAudio;
    }
}