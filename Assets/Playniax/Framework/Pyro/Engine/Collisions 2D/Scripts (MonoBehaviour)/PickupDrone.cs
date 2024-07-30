using UnityEngine;

namespace Playniax.Pyro
{
    public class PickupDrone : CollisionBase2D, IPurchasableHelper
    {
        [System.Serializable]
        public class MessengerSettings
        {
            public string activatedId = "Activated";
        }

        public MessengerSettings messengerSettings;
        public GameObject prefab;
        public string activatedText = "Drone Activated";

        [SerializeField] string _purchasableId = "Drone";
        [SerializeField] int _purchasableMax;

        public int purchasableCounter
        {
            get
            {
                var drones = FindObjectsOfType<Drone>();
                return drones.Length;
            }

            set
            {
            }
        }

        public string purchasableId
        {
            get { return _purchasableId; }

            set { _purchasableId = value; }
        }

        public int purchasableMax
        {
            get { return _purchasableMax; }
            set { _purchasableMax = value; }
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

        public override void Awake()
        {
            base.Awake();

            if (prefab && prefab.scene.rootCount > 0) prefab.SetActive(false);
        }

        public void OnBuy()
        {
            if (prefab == null) return;
            if (prefab.GetComponent<Drone>() == null) return;

            var drone = Instantiate(prefab, transform.position, transform.rotation).GetComponent<Drone>();

            drone.controller = gameObject;

            drone.gameObject.SetActive(true);
        }
        public override void OnCollision(CollisionBase2D collision)
        {
            if (prefab == null) return;
            if (prefab.GetComponent<Drone>() == null) return;

            if (collision.id != id) return;

            var drone = Instantiate(prefab, transform.position, transform.rotation).GetComponent<Drone>();

            drone.controller = gameObject;

            drone.gameObject.SetActive(true);

            if (Messenger.instance) Messenger.instance.Create(messengerSettings.activatedId, activatedText, collision.transform.position);

            Destroy(collision.gameObject);
        }
    }
}