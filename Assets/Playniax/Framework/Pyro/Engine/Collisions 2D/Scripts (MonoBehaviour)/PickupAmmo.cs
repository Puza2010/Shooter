using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class PickupAmmo : CollisionBase2D, IPurchasableHelper
    {
        [System.Serializable]
        public class MessengerSettings
        {
            public string activatedId = "Activated";
            public string deactivatedId = "Deactivated";
            public string reloadedId = "Reloaded";
        }

        public enum Mode { increase, straightToMax };

        public MessengerSettings messengerSettings;
        public BulletSpawnerBase spawner;
        public string spawnerId;

        public Mode mode;
        public int increase = 250;
        public int max = 1000;

        public string activatedText = "Gun Activated At %";
        public string deactivatedText = "Gun Deactivated";
        public string reloadedText = "Gun At %";

#if UNITY_EDITOR
        [Header("Simulation keys (+ Left Shift)")]
        public KeyCode load = KeyCode.None;
#endif
        public int purchasableCounter
        {
            get
            {
                var spawner = GetSpawner();
                if (spawner) return spawner.timer.counter;
                return 0;
            }

            set
            {
                var spawner = GetSpawner();
                if (spawner) spawner.timer.counter = value;
            }
        }
        public string purchasableId
        {
            get { return spawnerId; }

            set { spawnerId = value; }
        }
        public int purchasableMax
        {
            get { return max; }
            set { max = value; }
        }

        public int playerIndex
        {
            get
            {
                var helper = FindObjectOfType<ShopHelper>();
                if (helper) return helper.playerIndex;
                return 0;
            }

            set
            {
                var helper = FindObjectOfType<ShopHelper>();
                if (helper) helper.playerIndex = value;
            }
        }

        public BulletSpawnerBase GetSpawner()
        {
            if (spawner == null && spawnerId != "") spawner = _GetSpawner(spawnerId);

            return spawner;
        }
            
        public void OnBuy()
        {
            if (mode == Mode.increase)
            {
                GetSpawner().timer.counter += increase;

                if (GetSpawner().timer.counter > max) GetSpawner().timer.counter = max;
            }
            else if (mode == Mode.straightToMax)
            {
                GetSpawner().timer.counter = max;
            }
        }

        public override void OnCollision(CollisionBase2D collision)
        {
            if (collision.id != id) return;

            if (mode == Mode.increase)
            {
                _Message(GetSpawner().timer.counter + increase);

                GetSpawner().timer.counter += increase;

                if (GetSpawner().timer.counter > max) GetSpawner().timer.counter = max;
            }
            else if (mode == Mode.straightToMax)
            {
                _Message(max);

                GetSpawner().timer.counter = max;
            }

            void _Message(int increasedCounter)
            {
                if (Messenger.instance == null) return;

                if (increasedCounter > max) increasedCounter = max;

                if (GetSpawner().timer.counter == 0)
                {
                    Messenger.instance.Create(messengerSettings.activatedId, activatedText.Replace("%", MathHelpers.Dif(max, increasedCounter)), collision.transform.position);
                }
                else
                {
                    Messenger.instance.Create(messengerSettings.reloadedId, reloadedText.Replace("%", MathHelpers.Dif(max, increasedCounter)), collision.transform.position);
                }
            }

            Destroy(collision.gameObject);
        }
        void Update()
        {
            if (GetSpawner().timer.GetCounterZeroState() == true && Messenger.instance) Messenger.instance.Create(messengerSettings.deactivatedId, deactivatedText, transform.position);

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(load))
                {
                    OnBuy();

                    var spawner = GetSpawner();

                    Debug.Log(spawnerId + " = " + spawner.timer.counter +  " / " + max);
                }
            }
#endif
        }
        BulletSpawnerBase _GetSpawner(string id)
        {
            var spawners = GetComponentsInChildren<BulletSpawnerBase>();

            for (int i = 0; i < spawners.Length; i++)
                if (spawners[i].id == id) return spawners[i];

            return null;
        }
    }
}