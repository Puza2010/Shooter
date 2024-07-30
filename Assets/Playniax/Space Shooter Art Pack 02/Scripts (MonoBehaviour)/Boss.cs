using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.SpaceShooterArtPack02
{
    public class Boss : MonoBehaviour
    {
        [System.Serializable]
        public class BulletSettings
        {
            public GameObject prefab;
            public int layer = 10;
            public Vector3 position;
            public float speed = 16f;
            public GameObject effectPrefab;
            public float effectScale = 1;
        }

        public float rotation = 0;
        public float radius = 1.7f;
        public Sprite[] frames;
        public SpriteRenderer[] guns;

        public BulletSettings bulletSettings;

        void Update()
        {
            _Update();
        }

        void _Update()
        {
            var fire = -1;

            _timer += 1 * Time.deltaTime;

            if (_timer > .1)
            {
                fire = Random.Range(0, guns.Length);

                _timer = 0;
            }

            for (int i = 0; i < guns.Length; i++)
            {
                if (guns[i] == null) continue;
                if (guns[i].enabled == false) continue;

                var angle = i * Mathf.PI * 2f / guns.Length;

                angle += rotation;

                guns[i].transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, -Mathf.Sin(angle) * radius);

                var frame = (int)MathHelpers.Mod(angle * Mathf.Rad2Deg, 360);

                guns[i].sprite = frames[frame];

                if (bulletSettings.prefab && fire == i)
                {
                    var instance = Instantiate(bulletSettings.prefab, guns[i].transform.position, guns[i].transform.rotation, transform.parent);
                    if (instance)
                    {
                        if (instance.layer != bulletSettings.layer) instance.layer = bulletSettings.layer;

                        var bulletBase = instance.GetComponent<BulletBase>();
                        if (bulletBase)
                        {
                            bulletBase.transform.eulerAngles = new Vector3(0, 0, -frame);

                            bulletBase.velocity = instance.transform.rotation * new Vector3(bulletSettings.speed + Random.Range(0, 5), 0, 0);

                            bulletBase.transform.Translate(bulletSettings.position, Space.Self);
                        }

                        instance.SetActive(true);
                    }
                }

                if (bulletSettings.effectPrefab && fire == i)
                {
                    var instance = Instantiate(bulletSettings.effectPrefab);
                    instance.transform.SetParent(guns[i].transform, false);
                    instance.transform.localScale *= bulletSettings.effectScale;

                    instance.SetActive(true);
                }
            }

            rotation += -1 * Time.deltaTime;
        }

        float _timer;
    }
}