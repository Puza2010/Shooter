using UnityEngine;

namespace Playniax.Ignition
{
    public class SpawnerBase : IgnitionBehaviour
    {
        public class ProgressCounter : MonoBehaviour
        {
            public static int resetCounter;

            public static void Add(int value)
            {
                GameData.progressScale += value;
                GameData.progress += value;
            }

            public static void Init()
            {
                if (resetCounter == 0)
                {
                    GameData.bodyCount = 0;
                    GameData.completed = 0;
                    GameData.progressScale = 0;
                    GameData.progress = 0;
                    GameData.spawned = 0;
                }
            }

            void OnDisable()
            {
                GameData.progress -= 1;
            }
        }

        public class Register : MonoBehaviour
        {
            public static int count;

            void OnEnable()
            {
                count += 1;
            }

            void OnDisable()
            {
                count -= 1;
            }
        }

        public override void Awake()
        {
            base.Awake();

            ProgressCounter.Init();
        }

        public virtual void OnEnable()
        {
            ProgressCounter.resetCounter++;

            count++;
        }

        public virtual void OnDisable()
        {
            ProgressCounter.resetCounter--;

            count--;
        }

        public static int count;
    }
}