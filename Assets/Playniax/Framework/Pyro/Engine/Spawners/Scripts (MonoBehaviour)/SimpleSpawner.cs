﻿using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    [AddComponentMenu("Playniax/Pyro/SimpleSpawner")]
    public class SimpleSpawner : SpawnerBase
    {
        public enum StartPosition { Left, Right, Top, Bottom, LeftOrRight, TopOrBottom, Random, Fixed };

        [Tooltip("Prefabs to instantiate.")]
        public GameObject[] prefab;

        [Tooltip("Parent transform for instantiated objects.")]
        public Transform parent;

        [Tooltip("Starting position for instantiated objects.")]
        public StartPosition startPosition = StartPosition.Random;

        [Tooltip("Time before the spawning starts.")]
        public float timer;

        [Tooltip("Interval between each spawn.")]
        public float interval = 1;

        [Tooltip("Range to randomize interval between spawns.")]
        public float intervalRange;

        [Tooltip("Number of objects to spawn.")]
        public int counter;

        [Tooltip("Whether to track the progress of spawning.")]
        public bool trackProgress;

        public override void OnInitialize()
        {
            for (int i = 0; i < prefab.Length; i++)
            {
                if (prefab[i] && prefab[i].scene.rootCount > 0) prefab[i].SetActive(false);
            }
        }

        public override void Awake()
        {
            if (_GetPrefabs() == 0)
            {
                enabled = false;

                return;
            }

            base.Awake();

            if (trackProgress)
            {
                GameData.progressScale += counter;
                GameData.progress += counter;
            }
        }
        public virtual GameObject OnSpawn()
        {
            var index = Random.Range(0, prefab.Length);

            var clone = Instantiate(prefab[index]);
            if (clone)
            {
                if (trackProgress)
                {
                    var progress = clone.GetComponent<ProgressCounter>();
                    if (progress == null) progress = clone.AddComponent<ProgressCounter>();
                }

                SetPosition(clone);

                clone.transform.SetParent(parent);

                clone.SetActive(true);
            }

            return clone;
        }
        public virtual void SetPosition(GameObject obj)
        {
            if (startPosition == StartPosition.Left)
            {
                _SetPosition(obj, 0);
            }
            else if (startPosition == StartPosition.Right)
            {
                _SetPosition(obj, 1);
            }
            else if (startPosition == StartPosition.Top)
            {
                _SetPosition(obj, 2);
            }
            else if (startPosition == StartPosition.Bottom)
            {
                _SetPosition(obj, 3);
            }
            else if (startPosition == StartPosition.LeftOrRight)
            {
                _SetPosition(obj, Random.Range(0, 2));
            }
            else if (startPosition == StartPosition.TopOrBottom)
            {
                _SetPosition(obj, Random.Range(2, 4));
            }
            else if (startPosition == StartPosition.Random)
            {
                _SetPosition(obj, Random.Range(0, 4));
            }
            else if (startPosition == StartPosition.Fixed)
            {
                obj.transform.position = transform.position;
            }
        }
        public virtual void UpdateSpawner()
        {
            if (_GetPrefabs() == 0)
            {
                enabled = false;

                return;
            }
            else if (counter == -1)
            {
                if (timer <= 0)
                {
                    OnSpawn();

                    timer = Random.Range(interval, interval + intervalRange);
                }
                else
                {
                    timer -= 1 * Time.deltaTime;
                }
            }
            else if (counter > 0)
            {
                if (timer <= 0)
                {
                    OnSpawn();

                    counter -= 1;

                    if (counter > 0)
                    {
                        timer = Random.Range(interval, interval + intervalRange);

                        UpdateSpawner();
                    }
                    else
                    {
                        enabled = false;
                    }
                }
                else
                {
                    timer -= 1 * Time.deltaTime;
                }
            }
        }
        void LateUpdate()
        {
            UpdateSpawner();
        }
        int _GetPrefabs()
        {
            var prefabs = 0;

            for (int i = 0; i < prefab.Length; i++)
            {
                if (prefab[i]) prefabs += 1;
            }

            return prefabs;
        }
        void _SetPosition(GameObject obj, int segment)
        {
            // Segment:

            // 0 = left
            // 1 = right
            // 2 = top
            // 3 = bottom

            var size = RendererHelpers.GetSize(obj) * .5f;

            var min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, transform.position.z - Camera.main.transform.position.z));
            var max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, transform.position.z - Camera.main.transform.position.z));

            min.x -= size.x;
            max.x += size.x;

            min.y += size.y;
            max.y -= size.y;

            var position = Vector3.zero;

            if (segment == 0)
            {
                position.x = min.x;
                position.y = Random.Range(min.y + size.y, max.y - size.y);
            }
            else if (segment == 1)
            {
                position.x = max.x;
                position.y = Random.Range(min.y + size.y, max.y - size.y);
            }
            else if (segment == 2)
            {
                position.x = Random.Range(min.x - size.x, max.x + size.x);
                position.y = min.y;
            }
            else if (segment == 3)
            {
                position.x = Random.Range(min.x - size.x, max.x + size.x);
                position.y = max.y;
            }

            obj.transform.position = position;
        }
    }
}