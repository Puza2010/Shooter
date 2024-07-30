using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Sequencer
{
    public class SequenceSpawner : SequenceBase
    {
        public enum StartPosition { Left, Right, Top, Bottom, LeftOrRight, TopOrBottom, Random, Fixed };

        [Tooltip("Prefabs to use.")]
        public GameObject[] prefabs;
        [Tooltip("Startposition.")]
        public StartPosition startPosition = StartPosition.Random;
        [Tooltip("Number of objects to spawn.")]
        public int counter = 1;
        [Tooltip("Timer.")]
        public float timer;
        [Tooltip("Interval.")]
        public float interval = 1;
        public float intervalRange;

        public override void OnInitialize()
        {
            for (int i = 0; i < prefabs.Length; i++)
                if (prefabs[i] && prefabs[i].scene.rootCount > 0) prefabs[i].SetActive(false);
        }

        public override void OnSequencerAwake()
        {
            ProgressCounter.Add(counter * prefabs.Length);
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

        public virtual GameObject[] OnSpawn()
        {
            GameObject[] clone = new GameObject[prefabs.Length];

            for (int i = 0; i < clone.Length; i++)
            {
                var prefab = Random.Range(0, prefabs.Length);

                clone[i] = Instantiate(prefabs[prefab], GetPosition(prefabs[prefab]), Quaternion.identity);

                clone[i].AddComponent<Register>();
                clone[i].AddComponent<ProgressCounter>();

                clone[i].SetActive(true);
            }

            return clone;
        }

        public virtual Vector3 GetPosition(GameObject obj)
        {
            if (startPosition == StartPosition.Left)
            {
                return _GetPosition(obj, 0);
            }
            else if (startPosition == StartPosition.Right)
            {
                return _GetPosition(obj, 1);
            }
            else if (startPosition == StartPosition.Top)
            {
                return _GetPosition(obj, 2);
            }
            else if (startPosition == StartPosition.Bottom)
            {
                return _GetPosition(obj, 3);
            }
            else if (startPosition == StartPosition.LeftOrRight)
            {
                return _GetPosition(obj, Random.Range(0, 2));
            }
            else if (startPosition == StartPosition.TopOrBottom)
            {
                return _GetPosition(obj, Random.Range(2, 4));
            }
            else if (startPosition == StartPosition.Random)
            {
                return _GetPosition(obj, Random.Range(0, 4));
            }
            else// if (startPosition == StartPosition.Fixed)
            {
                return  transform.position;
            }
        }

        Vector3 _GetPosition(GameObject obj, int segment)
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

            return position;
        }
    }
}