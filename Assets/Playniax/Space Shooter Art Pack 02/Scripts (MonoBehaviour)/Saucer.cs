using UnityEngine;
using Playniax.Ignition;

namespace Playniax.SpaceShooterArtPack02
{
    public class Saucer : MonoBehaviour
    {
        public GameObject laser;

        void Awake()
        {
            if (laser) laser.SetActive(false);
        }

        void Update()
        {
            var velocity = gameObject.transform.position - _position;

            if (laser && laser.activeInHierarchy == false && velocity.magnitude < .005f) laser.SetActive(true);

            _position = transform.position;
        }

        Vector3 _position;
    }
}