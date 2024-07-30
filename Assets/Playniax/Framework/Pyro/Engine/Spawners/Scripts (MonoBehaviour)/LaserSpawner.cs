using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class LaserSpawner : BulletSpawnerBase
    {
        [Tooltip("Prefab of the laser.")]
        public GameObject prefab;

        [Tooltip("Index of the player.")]
        public int playerIndex = -1;

        [Tooltip("Size of the laser.")]
        public float size = 1;

        [Tooltip("Order in layer for rendering.")]
        public int orderInLayer = 100;

        [Tooltip("Firing range of the laser.")]
        public float range = 0;

        [Tooltip("Amount of damage the laser inflicts.")]
        public int damage = 1000;

        [Tooltip("Time to live for the laser.")]
        public float ttl = .25f;

        [Tooltip("Index for referencing.")]
        public int index;

        [Tooltip("Properties for audio of the laser.")]
        public AudioProperties audioProperties;

        public override void OnInitialize()
        {
            if (prefab.scene.rootCount > 0) prefab.SetActive(false);
        }

        public override void UpdateSpawner()
        {
            Laser.Fire(prefab, playerIndex, orderInLayer, timer, gameObject, range, ttl, size, damage, audioProperties, index);
        }
    }
}
