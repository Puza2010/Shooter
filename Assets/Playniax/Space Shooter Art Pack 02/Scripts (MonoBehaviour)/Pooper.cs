using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.SpaceShooterArtPack02
{
    public class Pooper : MonoBehaviour
    {
        [System.Serializable]
        public class AdditionalSettings
        {
            public SpriteRenderer spriteRenderer;
            public Animator animator;

            public void Get(GameObject obj)
            {
                if (spriteRenderer == null) spriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (animator == null) animator = obj.GetComponent<Animator>();
            }
        }

        [System.Serializable]
        public class BulletSettings
        {
            public GameObject prefab;
            public float scale = 1;
            public float rotation = -90;
            public Vector3 position;
            public float speed = 16f;
            public GameObject effectPrefab;
            public float effectScale = 1;
        }

        [System.Serializable]
        public class SoundSettings
        {
            public AudioProperties intro;
        }

        public enum StartPosition { Left, Right };

        public float timer;
        public StartPosition startPosition = StartPosition.Left;
        public bool movement = true;
        public float speed = 1f;
        public int fireFrame = 3;
        public BulletSettings bulletSettings;
        public SoundSettings soundSettings;
        public AdditionalSettings additionalSettings;

        void Awake()
        {
            additionalSettings.Get(gameObject);

            if (movement) _StartOffScreen();

            if (timer > 0) additionalSettings.spriteRenderer.enabled = false;
        }

        void Update()
        {
            if (additionalSettings.spriteRenderer == null) return;

            if (additionalSettings.spriteRenderer.enabled == false)
            {
                timer -= 1 * Time.deltaTime;
                if (timer > 0) return;

                soundSettings.intro.Play();

                additionalSettings.spriteRenderer.enabled = true;
            }

            if (movement) _Movement();
            _Fire();
        }

        void _Fire()
        {
            var frame = additionalSettings.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length * (additionalSettings.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) * additionalSettings.animator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate;

            if (_state == 0 && (int)frame == fireFrame)
            {
                if (bulletSettings.prefab)
                {
                    var obj = Instantiate(bulletSettings.prefab);
                    if (obj)
                    {
                        obj.transform.position = transform.position;
                        obj.transform.rotation = transform.rotation;
                        obj.transform.localScale *= bulletSettings.scale;

                        var bulletBase = obj.GetComponent<BulletBase>();
                        if (bulletBase)
                        {
                            bulletBase.transform.Translate(bulletSettings.position, Space.Self);
                            bulletBase.transform.eulerAngles = new Vector3(0, 0, bulletSettings.rotation);

                            bulletBase.velocity = obj.transform.rotation * new Vector3(bulletSettings.speed, 0, 0);
                        }

                        obj.SetActive(true);
                    }
                }

                if (bulletSettings.effectPrefab)
                {
                    var obj = Instantiate(bulletSettings.effectPrefab);
                    obj.transform.SetParent(transform, false);
                    obj.transform.Translate(bulletSettings.position, Space.Self);
                    obj.transform.localScale *= bulletSettings.effectScale;

                    obj.SetActive(true);
                }

                _state = 1;
            }
            else if (_state == 1 && (int)frame != fireFrame)
            {
                _state = 0;
            }
        }

        void _Movement()
        {
            var size = RendererHelpers.GetSize(gameObject) * .5f;

            var min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, transform.position.z - Camera.main.transform.position.z));
            var max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, transform.position.z - Camera.main.transform.position.z));

            var position = transform.position;

            if (startPosition == StartPosition.Left)
            {
                position += Vector3.right * speed * Time.deltaTime;

                if (position.x > max.x + size.x / 2)
                {
                    position.x = min.x - size.x / 2;
                }
            }
            else if (startPosition == StartPosition.Right)
            {
                position += Vector3.left * speed * Time.deltaTime;

                if (position.x < min.x - size.x / 2)
                {
                    position.x = max.x + size.x / 2;
                }
            }

            transform.position = position;
        }

        void _StartOffScreen()
        {
            var size = RendererHelpers.GetSize(gameObject) * .5f;

            var min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, transform.position.z - Camera.main.transform.position.z));
            var max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, transform.position.z - Camera.main.transform.position.z));

            min.x -= size.x;
            max.x += size.x;

            min.y += size.y;
            max.y -= size.y;

            var position = transform.position;

            if (startPosition == StartPosition.Left)
            {
                position.x = min.x;
            }
            else if (startPosition == StartPosition.Right)
            {
                position.x = max.x;
            }

            transform.position = position;
        }

        int _state;
    }
}