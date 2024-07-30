using UnityEngine;
using System.Collections.Generic;
using Playniax.Pyro;

namespace Playniax.Sequencer
{

    public class InvadersSpawner : AdvancedSpawnerBase
    {
        public float distance = 1;
        public float speedScale = 1;

        public AnimationCurve speedCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 5));
        public int total => rows * columns;
        public float killed => (float)invadersLeft / total;

        public int rows = 3;
        public int columns = 12;

        public Vector3 rotation = new Vector3(0, 0, 90);

        public float structuralIntegrityMultiplier = 1;
        public BulletSpawner.Settings bulletSettings = new BulletSpawner.Settings();
        public CargoSettings cargoSettings = new CargoSettings();
        public SurpriseSettings surpriseSettings = new SurpriseSettings();
        public ChildSettings childSettings = new ChildSettings();

        public int invadersLeft
        {
            get { return total - _Count(); }
        }

        public override void OnInitialize()
        {
            for (int i = 0; i < prefabs.Length; i++)
                if (prefabs[i] && prefabs[i].scene.rootCount > 0) prefabs[i].SetActive(false);

            if (surpriseSettings.prefab && surpriseSettings.prefab.scene.rootCount > 0) surpriseSettings.prefab.SetActive(false);
            if (surpriseSettings.markerSettings.prefab && surpriseSettings.markerSettings.prefab.scene.rootCount > 0) surpriseSettings.markerSettings.prefab.SetActive(false);
        }
        public override void OnSequencerAwake()
        {
            ProgressCounter.Add(rows * columns);
        }

        public override void OnSequencerUpdate()
        {
            _Clean();

            if (state == 1)
            {
                _Spawn();

                state = 2;
            }
            else if (state > 1)
            {
                if (_list.Count == 0)
                {
                    enabled = false;

                    return;
                }
            }

            if (state == 2)
            {
                float speed = speedCurve.Evaluate(killed) * speedScale;

                transform.position += _direction * speed * Time.deltaTime;

                Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
                Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

                foreach (GameObject invader in _list)
                {
                    if (invader == null) continue;
                    if (invader.gameObject == null) continue;
                    if (invader.gameObject.activeInHierarchy == false) continue;

                    if (_direction.x > 0 && invader.transform.position.x >= (rightEdge.x - 1f))
                    {
                        _Advance();

                        break;
                    }
                    else if (_direction.x < 0 && invader.transform.position.x <= (leftEdge.x + 1f))
                    {
                        _Advance();

                        break;
                    }
                }
            }
            else if (state == 3)
            {
                float speed = speedCurve.Evaluate(killed) * speedScale;

                transform.position += Vector3.down * speed * Time.deltaTime;

                if (transform.position.y <= _position.y)
                {
                    transform.position = _position;

                    state = 2;
                }
            }
        }

        void _Advance()
        {
            _direction.x = -_direction.x;

            _position = transform.position + Vector3.down * distance;

            state = 3;
        }

        void _Clean()
        {
            for (int i = 0; i < _list.Count; i++)
                if (_list[i] == null) _list.RemoveAt(i);
        }
        int _Count()
        {
            _Clean();

            return _list.Count;
        }

        void _Spawn()
        {
            var localScale = transform.localScale;

            transform.localScale = Vector3.one;

            int surprise = Random.Range(0, rows * columns);

            for (int x = 0; x < rows; x++)
            {
                float width = distance * (columns - 1);
                float height = distance * (rows - 1);

                Vector2 centerOffset = new Vector2(-width * 0.5f, -height * 0.5f);
                Vector3 rowPosition = new Vector3(centerOffset.x, (distance * x) + centerOffset.y, 0f);

                for (int y = 0; y < columns; y++)
                {
                    var position = rowPosition;
                    position.x += distance * y;
                    position += transform.position;

                    var prefab = Random.Range(0, prefabs.Length);

                    var instance = Instantiate(prefabs[prefab], position, Quaternion.Euler(rotation), transform);

                    instance.SetActive(true);

                    instance.AddComponent<Register>();
                    instance.AddComponent<ProgressCounter>();

                    var scoreBase = instance.GetComponent<IScoreBase>();

                    PyroHelpers.OverrideStructuralIntegrity(prefabs[prefab].name, instance, scoreBase);

                    if (scoreBase != null) scoreBase.structuralIntegrity *= structuralIntegrityMultiplier;

                    var offCamera = instance.GetComponent<OffCamera>();
                    if (offCamera == null) offCamera = instance.AddComponent<OffCamera>();

                    offCamera.mode = OffCamera.Mode.Destroy;
                    offCamera.directions = OffCamera.Directions.Bottom;

                    if (bulletSettings.useTheseSettings && bulletSettings.prefab)
                    {
                        var bulletSpawner = instance.GetComponent<BulletSpawner>();
                        if (bulletSpawner == null) bulletSpawner = instance.AddComponent<BulletSpawner>();

                        bulletSpawner.ignoreParent = true;

                        bulletSpawner.mode = BulletSpawner.Mode.TargetPlayer;

                        bulletSpawner.Set(bulletSettings);
                    }

                    var collisionState = instance.GetComponent<CollisionState>();
                    if (collisionState) collisionState.outroSettings.ignoreParentForEffects = true;

                    if (surpriseSettings.prefab != null && _list.Count == surprise && collisionState != null)
                    {
                        collisionState.cargoSettings.prefab = new GameObject[1] { surpriseSettings.prefab };
                        collisionState.cargoSettings.scale = surpriseSettings.scale;

                        collisionState.cargoSettings.effectSettings = JsonUtility.FromJson<CollisionState.CargoSettings.EffectSettings>(JsonUtility.ToJson(surpriseSettings.effectSettings));

                        if (surpriseSettings.markerSettings.prefab) surpriseSettings.markerSettings.Create(instance);
                    }
                    else if (_cargo || cargoSettings.releaseMode == CargoSettings.ReleaseMode.All && collisionState != null)
                    {
                        collisionState.cargoSettings = JsonUtility.FromJson<CollisionState.CargoSettings>(JsonUtility.ToJson(cargoSettings));
                    }

                    if (childSettings.prefab && childSettings.random == true && Random.Range(0, 2) == 1 || childSettings.prefab && childSettings.random == false)
                    {
                        childSettings.Create(instance);
                    }

                    var animator = instance.GetComponent<Animator>();

                    if (animator)
                    {
                        var state = animator.GetCurrentAnimatorStateInfo(0);
                        animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
                    }

                    _cargo = !_cargo;

                    _list.Add(instance);
                }
            }

            transform.localScale = localScale;
        }

        List<GameObject> _list = new List<GameObject>();

        bool _cargo;

        Vector3 _position;
        Vector3 _direction = Vector3.right;
    }
}