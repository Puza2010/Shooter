using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.Sequencer
{
    public class DemoSpawner : AdvancedSpawnerBase
    {
        public enum StartPosition { Left, Right, Top, Bottom, LeftOrRight, TopOrBottom, Random, Fixed };

        [Tooltip("Startposition.")]
        public StartPosition startPosition = StartPosition.Random;
        public float margin;
        [Tooltip("Number of objects to spawn.")]
        public int counter = 1;
        [Tooltip("Timer.")]
        public float timer;
        [Tooltip("Interval.")]
        public float interval = 1;
        public float intervalRange;
        public OffCamera.Mode offCameraMode = OffCamera.Mode.Loop;
        public OffCamera.Directions offCameraDirections = OffCamera.Directions.All;
        public BulletSpawner.Settings bulletSettings = new BulletSpawner.Settings();
        public CargoSettings cargoSettings = new CargoSettings();
        public SurpriseSettings surpriseSettings = new SurpriseSettings();
        public ChildSettings childSettings = new ChildSettings();
        public AISettings simpleAISettings = new AISettings();
        public bool bodyCount;
        public override void OnInitialize()
        {
            for (int i = 0; i < prefabs.Length; i++)
                if (prefabs[i] && prefabs[i].scene.rootCount > 0) prefabs[i].SetActive(false);

            if (surpriseSettings.prefab && surpriseSettings.prefab.scene.rootCount > 0) surpriseSettings.prefab.SetActive(false);
            if (surpriseSettings.markerSettings.prefab && surpriseSettings.markerSettings.prefab.scene.rootCount > 0) surpriseSettings.markerSettings.prefab.SetActive(false);
        }
        public override void OnSequencerAwake()
        {
            //ProgressCounter.Add(counter * prefabs.Length);
            ProgressCounter.Add(counter);
        }
        public override void OnSequencerUpdate()
        {
            if (timer <= 0)
            {
                _Spawn();

                timer = Random.Range(interval, interval + intervalRange);

                counter -= 1;

                if (counter <= 0) enabled = false;
            }
            else
            {
                timer -= 1 * Time.deltaTime;
            }

            void _Spawn()
            {
                OnSpawn();
            }
        }
        public virtual GameObject OnSpawn()
        {
            var position = Vector3.zero;
            var rotation = 0f;

            var index = Random.Range(0, prefabs.Length);
            var prefab = prefabs[index];
            var scale = prefab.transform.localScale;

            GetPosition(prefab, ref position, ref rotation, ref scale);

            var instance = Instantiate(prefab, position, Quaternion.Euler(0, 0, rotation), null);

            instance.transform.localScale = scale;

            instance.AddComponent<Register>();
            instance.AddComponent<ProgressCounter>();

            var collisionState = instance.GetComponent<CollisionState>();

            if (bodyCount && collisionState)
            {
                collisionState.bodyCount = true;

                GameData.spawned += 1;
            }

            var scoreBase = instance.GetComponent<IScoreBase>();
            if (scoreBase != null)
            {
                PyroHelpers.OverrideStructuralIntegrity(prefab.name, instance, scoreBase);

                scoreBase.structuralIntegrity *= AdvancedSpawnerSettings.GetStructuralIntegrityMultiplier();
            }

            if (simpleAISettings.enabled)
            {
                var enemyAI = instance.GetComponent<EnemyAI>();
                if (enemyAI == null)
                {
                    enemyAI = instance.AddComponent<EnemyAI>();

                    enemyAI.cruiserSettings = JsonUtility.FromJson<EnemyAI.CruiserSettings>(JsonUtility.ToJson(simpleAISettings));

                    enemyAI.mode = EnemyAI.Mode.Cruiser;
                }
            }

            if (offCameraMode != OffCamera.Mode.None)
            {
                var offCamera = instance.GetComponent<OffCamera>();
                if (offCamera == null) offCamera = instance.AddComponent<OffCamera>();

                offCamera.mode = offCameraMode;
                offCamera.directions = offCameraDirections;
            }

            if (bulletSettings.useTheseSettings && bulletSettings.prefab)
            {
                var bulletSpawner = instance.GetComponent<BulletSpawner>();
                if (bulletSpawner == null) bulletSpawner = instance.AddComponent<BulletSpawner>();

                bulletSpawner.Set(bulletSettings);
            }

            if (collisionState && surpriseSettings.prefab != null && counter == 1)
            {
                collisionState.cargoSettings.prefab = new GameObject[1] { surpriseSettings.prefab };
                collisionState.cargoSettings.scale = surpriseSettings.scale;

                collisionState.cargoSettings.effectSettings = JsonUtility.FromJson<CollisionState.CargoSettings.EffectSettings>(JsonUtility.ToJson(surpriseSettings.effectSettings));

                if (surpriseSettings.markerSettings.prefab) surpriseSettings.markerSettings.Create(instance);
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

            instance.SetActive(true);

            return instance;
        }
        public void GetPosition(GameObject obj, ref Vector3 position, ref float rotation, ref Vector3 scale)
        {
            if (startPosition == StartPosition.Left)
            {
                _GetPosition(obj, 0, ref position, ref rotation, ref scale);
            }
            else if (startPosition == StartPosition.Right)
            {
                _GetPosition(obj, 1, ref position, ref rotation, ref scale);
            }
            else if (startPosition == StartPosition.Top)
            {
                _GetPosition(obj, 2, ref position, ref rotation, ref scale);
            }
            else if (startPosition == StartPosition.Bottom)
            {
                _GetPosition(obj, 3, ref position, ref rotation, ref scale);
            }
            else if (startPosition == StartPosition.LeftOrRight)
            {
                _GetPosition(obj, Random.Range(0, 2), ref position, ref rotation, ref scale);
            }
            else if (startPosition == StartPosition.TopOrBottom)
            {
                _GetPosition(obj, Random.Range(2, 4), ref position, ref rotation, ref scale);
            }
            else if (startPosition == StartPosition.Random)
            {
                _GetPosition(obj, Random.Range(0, 4), ref position, ref rotation, ref scale);
            }
            else
            {
                position = transform.position;
                rotation = Random.Range(0, 360);
            }
        }
        void _GetPosition(GameObject obj, int segment, ref Vector3 position, ref float rotation, ref Vector3 scale)
        {
            // Segment:

            // 0 = left
            // 1 = right
            // 2 = top
            // 3 = bottom

            var size = RendererHelpers.GetSize(obj);

            var min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, transform.position.z - Camera.main.transform.position.z));
            var max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, transform.position.z - Camera.main.transform.position.z));

            min.x -= size.x * .5f;
            max.x += size.x * .5f;
            min.y += size.y * .5f;
            max.y -= size.y * .5f;

            if (segment == 0)
            {
                position.x = min.x;
                position.y = Random.Range(min.y - size.y - margin, max.y + size.y + margin);
            }
            else if (segment == 1)
            {
                position.x = max.x;
                position.y = Random.Range(min.y - size.y - margin, max.y + size.y + margin);

                rotation = 180;
                scale.y *= -1;
            }
            else if (segment == 2)
            {
                position.x = Random.Range(min.x + size.x + margin, max.x - size.x - margin);
                position.y = min.y;

                rotation = -90;
            }
            else if (segment == 3)
            {
                position.x = Random.Range(min.x + size.x + margin, max.x - size.x - margin);
                position.y = max.y;

                rotation = 90;
            }
        }

        bool _cargo;
    }
}