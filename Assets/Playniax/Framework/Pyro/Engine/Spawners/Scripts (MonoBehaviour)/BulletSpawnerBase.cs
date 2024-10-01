using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    [DefaultExecutionOrder(250)]
    public class BulletSpawnerBase : IgnitionBehaviour
    {
        public static float weaponSpeedMultiplier = 1.0f;
        public float bootTime;
        public bool automatically = true;
        public Timer timer = new Timer();
        public string id;
        public bool allowResidue = true;
        public bool onlyFireBelowMaxVelocity;
        public float maxVelocity = 1;

        public static BulletSpawnerBase GetSpawner(string id)
        {
            var allBulletSpawners = FindObjectsOfType<BulletSpawnerBase>();

            for (int i = 0; i < allBulletSpawners.Length; i++)
                if (allBulletSpawners[i].id == id) return allBulletSpawners[i];

            return null;
        }

        public virtual bool AllowSpawning() { return true; }

        public void GetResidue(string residueName)
        {
            if (allowResidue == true && timer.counter != -1 && residueName != "")
            {
                var virtualInts = VirtualInts.instance;

                var property = virtualInts.Get(residueName);

                if (property != null) timer.counter = property.value;
            }
        }

        public void SetResidue(string residueName)
        {
            if (allowResidue == true && timer.counter != -1 && residueName != "")
            {
                var virtualInts = VirtualInts.instance;

                if (virtualInts != null) virtualInts.Set(residueName, timer.counter);
            }
        }

        public virtual void Update()
        {
            if (bootTime > 0)
            {
                bootTime -= Time.deltaTime;

                if (bootTime < 0)
                {
                    bootTime = 0;
                }
                else
                {
                    return;
                }
            }

            if (_previousPosition == Vector3.zero) _previousPosition = transform.position;

            var velocity = (_previousPosition - transform.position).magnitude / Time.deltaTime;

            if (automatically == true)
            {
                if (onlyFireBelowMaxVelocity == true)
                {
                    if (velocity < maxVelocity) UpdateSpawner();
                }
                else
                {
                    UpdateSpawner();
                }
            }

            _previousPosition = transform.position;
        }
        public virtual void OnSpawn() { }
        public virtual void UpdateSpawner() { }

        Vector3 _previousPosition;
    }
}
