using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;
using Playniax.Sequencer;

namespace Playniax.Portals
{
    public class Portal : PortalBase
    {
        public BulletSpawner.Settings bulletSettings = new BulletSpawner.Settings();
        public CargoSettings cargoSettings = new CargoSettings();
        public SurpriseSettings surpriseSettings = new SurpriseSettings();
        public ChildSettings childSettings = new ChildSettings();
        public AISettings simpleAISettings = new AISettings();
        public OffCamera.Mode offCameraMode = OffCamera.Mode.Loop;
        public bool bodyCount;

        public override void OnInitialize()
        {
            base.OnInitialize();

            if (surpriseSettings.prefab && surpriseSettings.prefab.scene.rootCount > 0) surpriseSettings.prefab.SetActive(false);
            if (surpriseSettings.markerSettings.prefab && surpriseSettings.markerSettings.prefab.scene.rootCount > 0) surpriseSettings.markerSettings.prefab.SetActive(false);
        }

        public override GameObject OnSpawn()
        {
            var instance = base.OnSpawn();

            var collisionState = instance.GetComponent<CollisionState>();

            if (bodyCount && collisionState)
            {
                collisionState.bodyCount = true;

                GameData.spawned += 1;
            }

            var scoreBase = instance.GetComponent<IScoreBase>();
            if (scoreBase != null) scoreBase.structuralIntegrity *= AdvancedSpawnerSettings.GetStructuralIntegrityMultiplier();

            if (simpleAISettings.enabled)
            {
                var enemyAI = instance.GetComponent<EnemyAI>();
                if (enemyAI == null) enemyAI = instance.AddComponent<EnemyAI>();

                enemyAI.cruiserSettings = JsonUtility.FromJson<EnemyAI.CruiserSettings>(JsonUtility.ToJson(simpleAISettings));

                enemyAI.mode = EnemyAI.Mode.Cruiser;
            }

            if (offCameraMode != OffCamera.Mode.None)
            {
                var offCamera = instance.GetComponent<OffCamera>();
                if (offCamera == null) offCamera = instance.AddComponent<OffCamera>();

                offCamera.mode = offCameraMode;
            }

            if (bulletSettings.useTheseSettings && bulletSettings.prefab)
            {
                var bulletSpawner = instance.GetComponent<BulletSpawner>();
                if (bulletSpawner == null) bulletSpawner = instance.AddComponent<BulletSpawner>();

                bulletSpawner.mode = BulletSpawner.Mode.TargetPlayer;

                bulletSpawner.Set(bulletSettings);
            }

            if (collisionState && surpriseSettings.prefab != null && _surprise == true)
            {
                var surprise = Random.Range(0, 2);

                if (surprise == 1)
                {
                    _surprise = false;

                    collisionState.cargoSettings.prefab = new GameObject[1] { surpriseSettings.prefab };
                    collisionState.cargoSettings.scale = surpriseSettings.scale;

                    collisionState.cargoSettings.effectSettings = JsonUtility.FromJson<CollisionState.CargoSettings.EffectSettings>(JsonUtility.ToJson(surpriseSettings.effectSettings));

                    if (surpriseSettings.markerSettings.prefab) surpriseSettings.markerSettings.Create(instance);
                }
            }
            else if (collisionState && (_cargo || cargoSettings.releaseMode == CargoSettings.ReleaseMode.All))
            {
                collisionState.cargoSettings = JsonUtility.FromJson<CollisionState.CargoSettings>(JsonUtility.ToJson(cargoSettings));
            }
            else
            {
                _cargo = !_cargo;
            }

            if (childSettings.prefab && childSettings.random == true && Random.Range(0, 2) == 1 || childSettings.prefab && childSettings.random == false)
            {
                childSettings.Create(instance);
            }

            return instance;
        }

        bool _cargo;
        bool _surprise = true;
    }
}
