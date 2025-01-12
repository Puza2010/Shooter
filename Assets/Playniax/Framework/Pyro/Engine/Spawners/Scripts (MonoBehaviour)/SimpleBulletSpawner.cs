using UnityEngine;

namespace Playniax.Pyro
{
    public class SimpleBulletSpawner : BulletSpawnerBase
    {
        [Tooltip("Prefab of the bullet to spawn.")]
        public GameObject prefab;

        [Tooltip("Parent transform for the instantiated bullet.")]
        public Transform parent;

        [Tooltip("Initial position of the instantiated bullet.")]
        public Vector3 position;

        [Tooltip("Scale of the instantiated bullet.")]
        public float scale = 1;

        public override void UpdateSpawner()
        {
            if (prefab == null) return;

            if (BulletSpawnerHelper.count > 0 && timer.Update())
            {
                if (id == "Missile Barrage")
                {
                    // Spawn center missile
                    OnSpawn();
                    
                    // Store original position and scale
                    var originalPosition = position;
                    var originalScale = scale;
                    
                    // Spawn left missile
                    position += new Vector3(0, -0.25f, 0);
                    scale = 1.5f;  // Make missiles slightly bigger
                    OnSpawn();
                    
                    // Spawn right missile
                    position = originalPosition + new Vector3(0, 0.25f, 0);
                    OnSpawn();
                    
                    // Reset position and scale
                    position = originalPosition;
                    scale = originalScale;
                }
                else
                {
                    OnSpawn();
                }
            }
        }

        public override void OnInitialize()
        {
            if (prefab && prefab.scene.rootCount > 0) prefab.SetActive(false);
        }

        public override void OnSpawn()
        {
            var instance = Instantiate(prefab, transform.position, transform.rotation);

            if (instance)
            {
                instance.transform.localScale *= scale;
                instance.transform.Translate(position, Space.Self);

                if (parent)
                {
                    instance.transform.parent = parent;
                }
                else
                {
                    instance.transform.parent = transform.parent;
                }

                instance.SetActive(true);
            }
        }
    }
}
