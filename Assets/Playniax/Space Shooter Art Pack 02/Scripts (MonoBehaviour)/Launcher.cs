using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.SpaceShooterArtPack02
{
    public class Launcher : AnimationPool
    {
        [System.Serializable]
        public class Rocket
        {
            public GameObject prefab;
            public Vector3 position;
            public Vector3 rotation;
            public float timer = 1;
            public float interval = 3;
        }

        public float range = 10;
        public Rocket rocket;
        public SpriteRenderer spriteRenderer;
        void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }
        void Update()
        {
            if (spriteRenderer == null) return;

            if (_state == 0)
            {
                var player = PlayerGroup.GetFirstAvailable();

                if (player != null)
                {
                    if (Vector3.Distance(player.transform.position, transform.position) < range || range == 0)
                    {
                        rocket.timer -= 1 * Time.deltaTime;

                        if (rocket.timer < 0) _state = 1;
                    }
                }
            }
            else if (_state == 1)
            {
                if (Find("Hatch").PlayOnce(spriteRenderer, .08f) == true) _state = 2;
            }
            else if (_state == 2)
            {
                var instance = Instantiate(rocket.prefab, transform.position + rocket.position, transform.rotation * Quaternion.Euler(rocket.rotation), transform.parent);

                instance.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder - 1;

                _state = 3;
            }
            else if (_state == 3)
            {
                if (Find("Hatch").PlayOnce(spriteRenderer, .08f, true) == true)
                {
                    //Find("Hatch").SetFrame(spriteRenderer, 0);

                    rocket.timer = Random.Range(1, 1 + rocket.interval);

                    _state = 0;
                }
            }
        }

        int _state;
    }
}