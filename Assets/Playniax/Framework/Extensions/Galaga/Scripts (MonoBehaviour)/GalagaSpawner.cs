//https://youtu.be/ENhxtZZjJSA

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Playniax.Pyro;
using Playniax.Ignition;
using Playniax.Sequencer;

namespace Playniax.Galaga
{

    public class GalagaSpawner : AdvancedSpawnerBase
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(GalagaSpawner))]
        public class GalagaSpawnerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                var myScript = (GalagaSpawner)target;

                EditorGUILayout.Separator();

                if (GUILayout.Button("Mirror Points X"))
                {
                    for (int i = 0; i < myScript.points.Length; i++)
                        myScript.points[i].x = -myScript.points[i].x;

                    SceneView.RepaintAll();
                }
                else if (GUILayout.Button("Mirror Points Y"))
                {
                    for (int i = 0; i < myScript.points.Length; i++)
                        myScript.points[i].y = -myScript.points[i].y;

                    SceneView.RepaintAll();
                }
                /*
                else if (GUILayout.Button("Draw Circle"))
                {
                    DrawCircle(myScript.points[myScript.points.Length - 1], 0, 3, 8, -1);

                    SceneView.RepaintAll();
                }

                void DrawCircle(Vector3 position, float start, float radius, float steps, int direction)
                {
                    var step = 360 / steps;

                    for (int i = 0; i < steps; i++)
                    {
                        var angle = start + i * direction * step * Mathf.Deg2Rad;

                        var point = position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

                        myScript.points = ArrayHelpers.Add(myScript.points, point);
                    }
                }
                */
            }
            void OnSceneGUI()
            {
                var galagaSpawner = (GalagaSpawner)target;

                if (galagaSpawner.drawGizmos && galagaSpawner.points.Length > 0 && Selection.activeGameObject == galagaSpawner.gameObject)
                {
                    //Handles.DrawAAPolyLine(3, galagaBehaviour.points);

                    for (int i = 0; i < galagaSpawner.points.Length; i++)
                    {
                        galagaSpawner.points[i] = Handles.PositionHandle(galagaSpawner.points[i], Quaternion.identity);
                    }
                }
            }
        }
#endif
        public enum SpawnMode { Mono, TwinVertical, TwinHorizontal };

        [Tooltip("Determines were the sprite enters the screen.")]
        public Randomizer.Position entrance = Randomizer.Position.RandomLeft;
        [Tooltip("Determines were the sprite exits the screen.")]
        public Randomizer.Position exit = Randomizer.Position.RandomRight;
        [Tooltip("Mode can be Mode.Mono, Mode.TwinVertical or Mode.TwinHorizontal.")]
        public SpawnMode spawnMode;
        [Tooltip("Speed.")]
        public float speed = 128;
        [Tooltip("Determines if the sprites rotate with movement.")]
        public bool allowRotationWithPath = true;
        [Tooltip("Rotation offset.")]
        public int rotationOffset;
        [Tooltip("Rotation offset (when formation).")]
        public int formationRotationOffset = 90;
        [Tooltip("Timer.")]
        public float timer = 1;
        [Tooltip("Timer interval.")]
        public float interval = .1f;
        [Tooltip("Time to stay on the grid.")]
        public float sustain = 3;
        [Tooltip("Randomizes the time to stay on the grid.")]
        public bool randomSustain;
        [Tooltip("Determines the number of sprites.")]
        public int counter = 8;
        [Tooltip("Number of bezier loops (-1 = inifinte).")]
        public int loops = -1;
        [Tooltip("Number of bezier points that are created automatcially.")]
        public int randomPoints = 3;
        [Tooltip("Manual bezier points (randomPoints must be set to zero).")]
        public Vector3[] points;
        [Tooltip("Count the number of destroyed objects.")]
        public bool bodyCount;
        [Tooltip("Determines if bullets are fired, the speed of the bullets etc.")]
        public BulletSpawner.Settings bulletSettings = new BulletSpawner.Settings();
        [Tooltip("Determines if the spawned object contains cargo like for example a coin or pickup.")]
        public CargoSettings cargoSettings = new CargoSettings();
        [Tooltip("Determines if the spawned object contains a surprise like for example a coin or pickup.")]
        public SurpriseSettings surpriseSettings = new SurpriseSettings();
        [Tooltip("Determines if the spawned object carries a child object.")]
        public ChildSettings childSettings = new ChildSettings();

        [HideInInspector]
        public Vector2 lastEnterance;
        [HideInInspector]
        public Vector2 lastExit;

#if UNITY_EDITOR
        public bool drawGizmos;
        public Color gizmoColor = Color.green;
        void OnDrawGizmos()
        {
            if (points == null) return;
            if (points.Length < 2) return;
            if (drawGizmos == false) return;
            if (Selection.activeGameObject != gameObject) return;

            GalagaBehaviour.DrawGizmos(points, gizmoColor);
        }
#endif
        public static Vector3[] GetRandomPoints(Vector2 start, Vector2 range, int count, FloatRange distance, float failsafe = 10, Vector2 position = default)
        {
            var points = new Vector3[count];

            for (int i = 0; i <= points.Length - 1; i++)
            {
                points[i].x = Random.Range(-range.x, range.x);
                points[i].y = Random.Range(-range.y, range.y);

                var attempts = failsafe;

                var currentDistance = Vector2.Distance(start, points[i]);

                while (attempts > 0 && (currentDistance < distance.min || currentDistance > distance.max))
                {
                    points[i].x = Random.Range(-range.x, range.x);
                    points[i].y = Random.Range(-range.y, range.y);

                    currentDistance = Vector3.Distance(start, points[i]);

                    attempts -= 1;
                }

                start = points[i];

                points[i].x += position.x;
                points[i].y += position.y;
            }

            return points;
        }
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
            var count = counter;

            if (spawnMode != SpawnMode.Mono) count *= 2;

            ProgressCounter.Add(count);

            _EnsurePoints();
        }

        public override void OnSequencerUpdate()
        {
            if (timer <= 0)
            {
                _Spawn();

                _cargo = !_cargo;

                timer = interval;

                counter -= 1;

                if (counter <= 0) enabled = false;
            }
            else
            {
                timer -= 1 * Time.deltaTime;
            }

            void _Spawn()
            {
                var prefab = Random.Range(0, prefabs.Length);

                var instance = Instantiate(prefabs[prefab], lastEnterance, Quaternion.identity);

                instance.AddComponent<Register>();
                instance.AddComponent<ProgressCounter>();

                var scoreBase = instance.GetComponent<IScoreBase>();
                if (scoreBase != null)
                {
                    PyroHelpers.OverrideStructuralIntegrity(prefabs[prefab].name, instance, scoreBase);

                    scoreBase.structuralIntegrity *= AdvancedSpawnerSettings.GetStructuralIntegrityMultiplier();
                }

                var galagaBase = instance.AddComponent<GalagaBehaviour>();

                galagaBase.points = new Vector3[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    galagaBase.points[i] = points[i];
                }

                galagaBase.speed = speed;
                galagaBase.allowRotationWithPath = allowRotationWithPath;
                galagaBase.rotationOffset = rotationOffset;
                galagaBase.formationRotationOffset = formationRotationOffset;
                galagaBase.sustain = sustain;
                galagaBase.randomSustain = randomSustain;

                galagaBase.loops = loops;

                if (bulletSettings.useTheseSettings && bulletSettings.prefab)
                {
                    var bulletSpawner = instance.GetComponent<BulletSpawner>();
                    if (bulletSpawner == null) bulletSpawner = instance.AddComponent<BulletSpawner>();

                    bulletSpawner.mode = BulletSpawner.Mode.TargetPlayer;

                    bulletSpawner.Set(bulletSettings);
                }

                galagaBase.Go(lastEnterance, lastExit);

                var collisionState = instance.GetComponent<CollisionState>();

                if (collisionState && (_cargo || cargoSettings.releaseMode == CargoSettings.ReleaseMode.All && cargoSettings.prefab != null))
                    collisionState.cargoSettings = JsonUtility.FromJson<CollisionState.CargoSettings>(JsonUtility.ToJson(cargoSettings));

                if (childSettings.prefab && childSettings.random == true && Random.Range(0, 2) == 1 || childSettings.prefab && childSettings.random == false)
                {
                    childSettings.Create(instance);
                }

                instance.SetActive(true);

                if (spawnMode != SpawnMode.Mono)
                {
                    var twin = Instantiate(instance, lastEnterance, Quaternion.identity).GetComponent<GalagaBehaviour>();

                    if (spawnMode == SpawnMode.TwinVertical)
                    {
                        twin.lastEnterance.x = -twin.lastEnterance.x;
                        twin.lastExit.x = -twin.lastExit.x;

                        for (int i = 0; i < twin.points.Length; i++)
                        {
                            twin.points[i].x = -twin.points[i].x;
                        }
                    }
                    else if (spawnMode == SpawnMode.TwinHorizontal)
                    {
                        twin.lastEnterance.y = -twin.lastEnterance.y;
                        twin.lastExit.y = -twin.lastExit.y;

                        for (int i = 0; i < twin.points.Length; i++)
                        {
                            twin.points[i].y = -twin.points[i].y;
                        }
                    }

                    twin.Go(twin.lastEnterance, twin.lastExit);
                }

                if (collisionState)
                {
                    if (bodyCount)
                    {
                        collisionState.bodyCount = true;

                        GameData.spawned += 1;
                    }

                    if (surpriseSettings.prefab != null && counter == 1)
                    {
                        collisionState.cargoSettings.prefab = new GameObject[1] { surpriseSettings.prefab };
                        collisionState.cargoSettings.scale = surpriseSettings.scale;

                        collisionState.cargoSettings.effectSettings = JsonUtility.FromJson<CollisionState.CargoSettings.EffectSettings>(JsonUtility.ToJson(surpriseSettings.effectSettings));

                        if (surpriseSettings.markerSettings.prefab) surpriseSettings.markerSettings.Create(instance);
                    }
                }
            }
        }

        void _EnsurePoints()
        {
            var size = Vector3.zero;

            for (int i = 0; i < prefabs.Length; i++)
                if (prefabs[i].GetComponent<SpriteRenderer>().bounds.size.magnitude > size.magnitude) size = prefabs[i].GetComponent<SpriteRenderer>().bounds.size;

            _EnsurePoints(size);
        }
        void _EnsurePoints(Vector3 spriteSize)
        {
            spriteSize /= 2;

            var margin = GalagaGrid.GetViewportMargin();

            var windowSize = GalagaGrid.GetViewportSize(margin);

            lastEnterance = Randomizer.GetPosition(entrance, spriteSize, margin);
            lastExit = Randomizer.GetPosition(exit, spriteSize, margin);

            if (randomPoints > 0)
            {
                points = new Vector3[randomPoints];

                points = GalagaSpawner.GetRandomPoints(lastEnterance, new Vector2(windowSize.x, windowSize.y), points.Length, GalagaGrid.GetRandomPointsDistance());
            }
            else if (points.Length == 0)
            {
                points = new Vector3[Random.Range(4, 8)];

                points = GalagaSpawner.GetRandomPoints(lastEnterance, new Vector2(windowSize.x, windowSize.y), points.Length, GalagaGrid.GetRandomPointsDistance());
            }
        }

        bool _cargo;
    }
}