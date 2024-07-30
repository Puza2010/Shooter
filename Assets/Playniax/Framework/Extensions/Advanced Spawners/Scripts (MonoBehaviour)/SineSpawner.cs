using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.Sequencer
{
    public class SineSpawner : AdvancedSpawnerBase
    {
        public class Motions : MonoBehaviour
        {
            public Vector3 angle;
            public Vector3 range;
            public Vector3 speed;
            public Vector3 velocity;
            public bool allowRotationWithPath;

            void Update()
            {
                var x = Mathf.Cos(angle.x * Mathf.Deg2Rad) * range.x * speed.x;
                var y = Mathf.Cos(angle.y * Mathf.Deg2Rad) * range.y * speed.y;
                var z = Mathf.Cos(angle.z * Mathf.Deg2Rad) * range.z * speed.z;

                var offset = velocity + new Vector3(x, y, z);

                transform.localPosition += offset * Time.deltaTime;

                if (allowRotationWithPath)
                {
                    float rotation = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

                    transform.localRotation = Quaternion.AngleAxis(rotation, Vector3.forward);
                }

                angle.x += Time.deltaTime * speed.x * 100;
                angle.y += Time.deltaTime * speed.y * 100;
                angle.z += Time.deltaTime * speed.z * 100;

                angle.x = MathHelpers.Mod(angle.x, 360);
                angle.y = MathHelpers.Mod(angle.y, 360);
                angle.z = MathHelpers.Mod(angle.z, 360);
            }
        }

        [System.Serializable]
        public class MotionSettings
        {
            [Header("Linear Settings")]
            public float linearSpeed = 4;
            public float linearSpeedIncrease;

            [Header("Sine Settings")]
            public Vector3 angle;
            public Vector3 range;
            public Vector3 speed;
            public Vector3 angleIncrease;
            public Vector3 rangeIncrease;
            public Vector3 speedIncrease;
            public bool allowRotationWithPath = true;
        }

        public enum Enterance { Left, Right, Top, Bottom, LeftOrRight, TopOrBottom, Random };

        public Enterance enterance;
        public float timer = 1;
        public float interval = 1;
        public int counter = 1;
        public float offset;
        public bool offsetFlip;
        public bool battleRope;
        public MotionSettings motionSettings = new MotionSettings();
        public BulletSpawner.Settings bulletSettings = new BulletSpawner.Settings();
        public CargoSettings cargoSettings = new CargoSettings();
        public SurpriseSettings surpriseSettings = new SurpriseSettings();
        public ChildSettings childSettings = new ChildSettings();
        public bool bodyCount;

        /* WIP!
#if UNITY_EDITOR
        public bool drawGizmos;
        public Color gizmoColor = Color.green;
#endif

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (drawGizmos == false) return;

            Vector3 angle = motionSettings.angle;
            Vector3 range = motionSettings.range;

            var smoothness = .015f;

            var velocity = Vector3.left * motionSettings.linearSpeed * smoothness;

            var point = new Vector3(7.657143f, offset, 0);

            for (int i = 0; i < 600; i++)
            {
                var x = Mathf.Cos(angle.x * Mathf.Deg2Rad) * range.x * motionSettings.speed.x;
                var y = Mathf.Cos(angle.y * Mathf.Deg2Rad) * range.y * motionSettings.speed.y;
                var z = Mathf.Cos(angle.z * Mathf.Deg2Rad) * range.z * motionSettings.speed.z;

                var offset = velocity + new Vector3(x, y, z) * smoothness;

                point += offset;

                angle.x += smoothness * motionSettings.speed.x * 100;
                angle.y += smoothness * motionSettings.speed.y * 100;
                angle.z += smoothness * motionSettings.speed.z * 100;

                angle.x = MathHelpers.Mod(angle.x, 360);
                angle.y = MathHelpers.Mod(angle.y, 360);
                angle.z = MathHelpers.Mod(angle.z, 360);

                Gizmos.DrawSphere(point, .035f);
            }

        }
#endif
        */
        public override void OnInitialize()
        {
            for (int i = 0; i < prefabs.Length; i++)
                if (prefabs[i] && prefabs[i].scene.rootCount > 0) prefabs[i].SetActive(false);

            if (surpriseSettings.prefab && surpriseSettings.prefab.scene.rootCount > 0) surpriseSettings.prefab.SetActive(false);
            if (surpriseSettings.markerSettings.prefab && surpriseSettings.markerSettings.prefab.scene.rootCount > 0) surpriseSettings.markerSettings.prefab.SetActive(false);

            cargoSettings.Init();
        }

        public override void OnSequencerAwake()
        {
            ProgressCounter.Add(counter);

            GetEnterance();
        }

        public override void OnSequencerUpdate()
        {
            if (timer <= 0)
            {
                Spawner();

                _cargo = !_cargo;

                timer = interval;

                counter -= 1;

                if (counter <= 0) enabled = false;
            }
            else
            {
                timer -= 1 * Time.deltaTime;
            }

            void Spawner()
            {
                Spawn();

                _battleRope = -_battleRope;

                motionSettings.angle += motionSettings.angleIncrease;
                motionSettings.linearSpeed += motionSettings.linearSpeedIncrease;
                motionSettings.range += motionSettings.rangeIncrease;
                motionSettings.speed += motionSettings.speedIncrease;

                if (offsetFlip) offset = -offset;

                void Spawn()
                {
                    var prefab = Random.Range(0, prefabs.Length);

                    var instance = Instantiate(prefabs[prefab], _enterance, Quaternion.Euler(0, 0, _rotation), null);

                    instance.transform.localScale = Vector3.Scale(instance.transform.localScale, _flip);

                    instance.AddComponent<ProgressCounter>();
                    instance.AddComponent<Register>();

                    //instance.AddComponent<BulletSpawnerHelper>();

                    var scoreBase = instance.GetComponent<IScoreBase>();
                    if (scoreBase != null)
                    {
                        PyroHelpers.OverrideStructuralIntegrity(prefabs[prefab].name, instance, scoreBase);

                        scoreBase.structuralIntegrity *= AdvancedSpawnerSettings.GetStructuralIntegrityMultiplier();
                    }

                    var collisionState = instance.GetComponent<CollisionState>();

                    if (bodyCount && collisionState)
                    {
                        collisionState.bodyCount = true;

                        GameData.spawned += 1;
                    }

                    var offCamera = instance.AddComponent<OffCamera>();

                    offCamera.mode = OffCamera.Mode.Destroy;

                    var motions = instance.AddComponent<Motions>();

                    motions.angle = motionSettings.angle;
                    motions.range = motionSettings.range;
                    motions.speed = motionSettings.speed;
                    motions.allowRotationWithPath = motionSettings.allowRotationWithPath;

                    if (battleRope) motions.speed *= _battleRope;

                    if (bulletSettings.useTheseSettings && bulletSettings.prefab)
                    {
                        var bulletSpawner = instance.GetComponent<BulletSpawner>();
                        if (bulletSpawner == null) bulletSpawner = instance.AddComponent<BulletSpawner>();

                        bulletSpawner.mode = BulletSpawner.Mode.TargetPlayer;

                        bulletSpawner.Set(bulletSettings);
                    }

                    if (surpriseSettings.prefab != null && counter == 1)
                    {
                        if (collisionState)
                        {
                            collisionState.cargoSettings.prefab = new GameObject[1] { surpriseSettings.prefab };
                            collisionState.cargoSettings.scale = surpriseSettings.scale;

                            collisionState.cargoSettings.effectSettings = JsonUtility.FromJson<CollisionState.CargoSettings.EffectSettings>(JsonUtility.ToJson(surpriseSettings.effectSettings));

                            if (surpriseSettings.markerSettings.prefab) surpriseSettings.markerSettings.Create(instance);
                        }
                    }
                    else if (_cargo || cargoSettings.releaseMode == CargoSettings.ReleaseMode.All)
                    {
                        if (collisionState)
                        {
                            collisionState.cargoSettings = JsonUtility.FromJson<CollisionState.CargoSettings>(JsonUtility.ToJson(cargoSettings));
                        }
                    }

                    if (childSettings.prefab && childSettings.random == true && Random.Range(0, 2) == 1 || childSettings.prefab && childSettings.random == false)
                    {
                        childSettings.Create(instance);
                    }

                    if (_segment == 0)
                    {
                        offCamera.directions = OffCamera.Directions.Right;

                        motions.velocity = Vector3.right * motionSettings.linearSpeed;

                        instance.transform.position += Vector3.up * offset;
                    }
                    else if (_segment == 1)
                    {
                        offCamera.directions = OffCamera.Directions.Left;

                        motions.velocity = Vector3.left * motionSettings.linearSpeed;

                        instance.transform.position += Vector3.down * offset;
                    }
                    if (_segment == 2)
                    {
                        offCamera.directions = OffCamera.Directions.Bottom;

                        motions.velocity = Vector3.down * motionSettings.linearSpeed;

                        instance.transform.position += Vector3.left * offset;
                    }
                    else if (_segment == 3)
                    {
                        offCamera.directions = OffCamera.Directions.Top;

                        motions.velocity = Vector3.up * motionSettings.linearSpeed;

                        instance.transform.position += Vector3.right * offset;
                    }

                    instance.SetActive(true);
                }
            }
        }

        public void GetEnterance()
        {
            var camera = Camera.main;

            var size = _GetAverageSize() * .5f;

            var min = camera.ViewportToWorldPoint(new Vector3(0, 1, -camera.transform.position.z));
            var max = camera.ViewportToWorldPoint(new Vector3(1, 0, -camera.transform.position.z));

            min.x -= size.x;
            max.x += size.x;

            min.y += size.y;
            max.y -= size.y;

            if (enterance == Enterance.Left)
            {
                _segment = 0;
            }
            else if (enterance == Enterance.Right)
            {
                _segment = 1;
            }
            else if (enterance == Enterance.Top)
            {
                _segment = 2;
            }
            else if (enterance == Enterance.Bottom)
            {
                _segment = 3;
            }
            else if (enterance == Enterance.LeftOrRight)
            {
                _segment = Random.Range(0, 2);
            }
            else if (enterance == Enterance.TopOrBottom)
            {
                _segment = Random.Range(2, 4);
            }
            else if (enterance == Enterance.Random)
            {
                _segment = Random.Range(0, 4);
            }

            // Segment:

            // 0 = left
            // 1 = right
            // 2 = top
            // 3 = bottom

            if (_segment == 0)
            {
                _enterance.x = -_enterance.x + min.x;
            }
            else if (_segment == 1)
            {
                _enterance.x += max.x;

                _rotation = 180;
                _flip.y *= -1;
            }
            else if (_segment == 2)
            {
                _enterance.y += min.y;

                _rotation = -90;
            }
            else if (_segment == 3)
            {
                _enterance.y = -_enterance.y + max.y;

                _rotation = 90;
            }
        }

        Vector2 _GetAverageSize()
        {
            var averageSize = Vector3.zero;

            for (int i = 0; i < prefabs.Length; i++)
            {
                var size = RendererHelpers.GetSize(prefabs[i].gameObject);

                if (size.magnitude > averageSize.magnitude) averageSize = size;
            }

            return averageSize;
        }

        int _battleRope = 1;
        bool _cargo;
        Vector3 _enterance;
        Vector3 _flip = Vector3.one;
        float _rotation;
        int _segment;
    }
}