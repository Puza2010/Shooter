#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Events;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    // Collision properties.
    public class CollisionState : CollisionBase2D, IScoreBase
    {
        [System.Serializable]
        // Cargo is released when an object is destroyed.
        public class CargoSettings
        {
            [System.Serializable]
            public class EffectSettings
            {
                public class Effect : MonoBehaviour
                {
                    public Motion motion;
                    public float motionSpeed = 1;
                    public bool magnetic;
                    public float magneticRange = 1;
                    public float magneticSpeed = 3;

                    void Update()
                    {
                        if (magnetic == false)
                        {
                            _Motion();
                        }
                        else
                        {
                            var closest = _GetClosest();

                            if (closest == null)
                            {
                                _Motion();
                            }
                            else
                            {
                                transform.position = Vector3.Lerp(transform.position, closest.transform.position, Time.deltaTime * magneticSpeed);
                            }
                        }

                        void _Motion()
                        {
                            if (motion == Motion.Down)
                            {
                                transform.position += Vector3.down * motionSpeed * Time.deltaTime;
                            }
                            if (motion == Motion.Up)
                            {
                                transform.position += Vector3.up * motionSpeed * Time.deltaTime;
                            }
                            if (motion == Motion.Left)
                            {
                                transform.position += Vector3.left * motionSpeed * Time.deltaTime;
                            }
                            if (motion == Motion.Right)
                            {
                                transform.position += Vector3.right * motionSpeed * Time.deltaTime;
                            }
                        }
                    }

                    GameObject _GetClosest()
                    {
                        var list = FindObjectsOfType<PlayerGroup>();

                        GameObject closest = null;
                        float closestDistance = float.MaxValue;

                        for (int i = 0; i < list.Length; i++)
                        {
                            var player = list[i];

                            if (player.gameObject != null && player.gameObject.activeInHierarchy == true)
                            {

                                float distance = Vector3.Distance(transform.position, player.transform.position);

                                if (distance < closestDistance && distance <= magneticRange)
                                {
                                    closestDistance = distance;
                                    closest = player.gameObject;
                                }
                            }
                        }

                        return closest;
                    }
                }
                public enum Motion { None, Down, Up, Left, Right };

                public Motion motion;
                public float motionSpeed = 1;
                public bool magnetic;
                public float magneticRange = 1;
                public float magneticSpeed = 3;
                public bool killOffCamera = true;
            }

            // The list of cargo objects.
            public GameObject[] prefab = new GameObject[0];

            // Determines the scale of cargo objects.
            public float scale = 1;

            public EffectSettings effectSettings = new EffectSettings();
            public void Add(GameObject obj)
            {
                var length = prefab.Length;
                System.Array.Resize(ref prefab, length + 1);
                prefab[length] = obj;
            }
            public void Clear()
            {
                System.Array.Resize(ref prefab, 0);
            }
            public void Init()
            {
                if (prefab == null) return;

                for (int i = 0; i < prefab.Length; i++)
                    if (prefab[i] && prefab[i].scene.rootCount > 0) prefab[i].SetActive(false);
            }
            public void Release(CollisionState sprite)
            {
                var i = Random.Range(0, prefab.Length);

                if (i >= prefab.Length) return;

                if (prefab[i] == null) return;

                var cargo = Instantiate(prefab[i], sprite.transform.position, Quaternion.identity);
                cargo.transform.localScale *= scale;
                cargo.SetActive(true);

                _Effect(cargo);
            }
            void _Effect(GameObject cargo)
            {
                if (effectSettings.motion != EffectSettings.Motion.None || effectSettings.magnetic == true)
                {
                    var effect = cargo.AddComponent<EffectSettings.Effect>();
                    effect.motion = effectSettings.motion;
                    effect.motionSpeed = effectSettings.motionSpeed;
                    effect.magnetic = effectSettings.magnetic;
                    effect.magneticRange = effectSettings.magneticRange;
                    effect.magneticSpeed = effectSettings.magneticSpeed;
                }

                if (effectSettings.killOffCamera)
                {
                    var offCamera = cargo.AddComponent<OffCamera>();
                    if (offCamera == null) offCamera = cargo.AddComponent<OffCamera>();

                    offCamera.mode = OffCamera.Mode.Destroy;
                    offCamera.directions = OffCamera.Directions.All;
                }
            }

            //int _index;
        }

        [System.Serializable]
        public class AdditionalSettings
        {
            public SpriteRenderer[] spriteRenderer = new SpriteRenderer[0];

            public void Init()
            {
                _defaultMaterial = new Material[spriteRenderer.Length];

                for (int i = 0; i < spriteRenderer.Length; i++)
                    _defaultMaterial[i] = spriteRenderer[i].material;
            }

            public void Ghost(Material material)
            {
                for (int i = 0; i < spriteRenderer.Length; i++)
                    if (material && spriteRenderer[i].material != material) spriteRenderer[i].material = material;
            }

            public void RestoreMaterial()
            {
                if (_defaultMaterial == null || _defaultMaterial != null && _defaultMaterial.Length == 0) return;

                for (int i = 0; i < spriteRenderer.Length; i++)
                    if (_defaultMaterial[i] != null) spriteRenderer[i].material = _defaultMaterial[i];
            }

            Material[] _defaultMaterial;
        }

        [System.Serializable]
        // Outro Settings determine what effect to play when an object is destroyed.
        public class EventSettings
        {
            public UnityEvent onAwake = new UnityEvent();
            public UnityEvent onGhost = new UnityEvent();
            public UnityEvent onOutro = new UnityEvent();
        }

        [System.Serializable]
        // Outro Settings determine what effect to play when an object is destroyed.
        public class OutroSettings
        {
            [System.Serializable]
            // On outro effects settings.
            public class EffectsSettings
            {
                // GameObjects to instantiate on outro.
                public GameObject prefab;
                // Scale of the GameObjects.
                public float scale = 1;
                // Sorting order.
                public int orderInLayer = 0;
                // Reference size.
                public float sizeInPixels = 64;
            }

            [System.Serializable]
            // Messenger Settings determine what messenger to use and if text or rewards are to be displayed.
            public class MessengerSettings
            {
                // Determines what messenger to use.
                public string messengerId = "Score";
                // Display text.
                //
                // Displays score points when left blank.
                public string text;
                // Determines if messages are enabled or not.
                public bool enabled = true;
                public static void Message(CollisionState collisionState)
                {
                    if (Messenger.instance == null || collisionState.outroSettings.messengerSettings.enabled == false) return;

                    if (collisionState.outroSettings.messengerSettings.text == "" && collisionState.points > 0)
                    {
                        Messenger.instance.Create(collisionState.outroSettings.messengerSettings.messengerId, collisionState.points.ToString() + "+", collisionState.transform.position);
                    }
                    else if (collisionState.outroSettings.messengerSettings.text != "")
                    {
                        Messenger.instance.Create(collisionState.outroSettings.messengerSettings.messengerId, collisionState.outroSettings.messengerSettings.text, collisionState.transform.position);
                    }
                }
            }

            // Effects Settings.
            public EffectsSettings[] effectsSettings = new EffectsSettings[0];
            // Messenger Settings.
            public MessengerSettings messengerSettings = new MessengerSettings();
            // Audio Settings.
            public AudioProperties audioSettings = new AudioProperties();
            // Makes sure the effect has no parent.
            public bool ignoreParentForEffects;
            // Determines if outro is used.
            public bool enabled = true;

            public void Play(CollisionState collisionState)
            {
                if (enabled == true && effectsSettings.Length > 0) _PlayEffects(collisionState);
            }

            void _PlayEffects(CollisionState collisionState)
            {
                for (int i = 0; i < effectsSettings.Length; i++)
                {
                    var prefab = effectsSettings[i].prefab;
                    if (prefab == null) continue;

                    var scale = GetScale();

                    if (collisionState.outroSettings.ignoreParentForEffects)
                    {
                        var instance = Instantiate(prefab, collisionState.transform.position, Quaternion.identity, null);
                        instance.transform.localScale *= scale;

                        var instanceRenderer = instance.GetComponent<Renderer>();
                        if (instanceRenderer && collisionState.spriteRenderer != null) instanceRenderer.sortingOrder = collisionState.spriteRenderer.sortingOrder + effectsSettings[i].orderInLayer;
                    }
                    else
                    {
                        var instance = Instantiate(prefab, collisionState.transform.position, Quaternion.identity, collisionState.transform.parent);
                        instance.transform.localScale *= scale;

                        var instanceRenderer = instance.GetComponent<Renderer>();
                        if (instanceRenderer && collisionState.spriteRenderer != null) instanceRenderer.sortingOrder = collisionState.spriteRenderer.sortingOrder + effectsSettings[i].orderInLayer;
                    }

                    audioSettings.Play();

                    float GetScale()
                    {
                        if (collisionState.spriteRenderer)
                        {
                            var scale = effectsSettings[i].scale;

                            if (effectsSettings[i].sizeInPixels > 0) scale *= Mathf.Max(collisionState.spriteRenderer.sprite.rect.size.x, collisionState.spriteRenderer.sprite.rect.size.y) / effectsSettings[i].sizeInPixels;

                            return scale *= Mathf.Max(collisionState.transform.localScale.x, collisionState.transform.localScale.y);
                        }

                        return 1;
                    }
                }
            }
        }

        public static int autoPointsMultiplier = 10;

        [SerializeField] string _material = "Metal";
        [SerializeField] float _structuralIntegrity = 1;
        // Determines if points are automatically set by multiplying structuralIntegrity by 10 (default value 10 can be changed by setting 'autoPointsMultiplier')
        public bool autoPoints = true;
        [SerializeField] int _points;
        // Determines what player is rewarded.
        //
        // -1 does nothing but anything above -1 adds the points of an object destroyed to the player the index is set to.
        //
        // For example if playerIndex = 0 the points will be rewarded to player 1. playerIndex = 1 and the points will be rewarded to player 2 etc.
        //
        // To get the points you can do something like: PlayerData.Get(0).score
        public int playerIndex = -1;
        public bool playerLivesCount;

        [SerializeField] bool _indestructible;

        [SerializeField] GameObject _friend;

        public GameObject parent;

        public CollisionBase2D shield;
        public bool isParentShield;

        public bool dontDestroy;
        public bool bodyCount;
        // Outro Settings.
        public OutroSettings outroSettings = new OutroSettings();
        // Cargo Settings.
        public CargoSettings cargoSettings = new CargoSettings();
        // Event Settings.
        public EventSettings eventSettings = new EventSettings();
        // SpriteRenderer to use.
        public SpriteRenderer spriteRenderer;
        // Determines if a BoxCollider should be created automatically when a collider is missing.
        public bool generateBoxCollider = true;
        public Material ghostMaterial;
        public int ghostSustain = 3;
        public AdditionalSettings additionalSettings = new AdditionalSettings();

        public GameObject friend
        {
            get { return _friend; }
            set { _friend = value; }
        }

        public bool indestructible
        {
            get { return _indestructible; }
            set { _indestructible = value; }
        }

        public override bool isAllowed
        {
            get
            {
                if (shield && shield.gameObject && shield.gameObject.activeInHierarchy && shield != this) return false;

                return base.isAllowed;
            }
        }

        public GameObject isTargeted
        {
            get;
            set;
        }

        public bool isVisible
        {
            get { return true; }
        }

        public string material
        {
            get { return _material; }
            set { _material = value; }
        }

        public int points
        {
            get { return _points; }
            set { _points = value; }
        }

        public float structuralIntegrity
        {
            get { return _structuralIntegrity; }
            set { _structuralIntegrity = value; }
        }

        public override void Awake()
        {
            base.Awake();

            if (isParentShield && transform.parent)
            {
                var parent = transform.parent.GetComponent<CollisionState>();
                if (parent && parent.shield == null) parent.shield = this;
            }

            cargoSettings.Init();

            if (autoPoints)
            {
                if (structuralIntegrity == 0) points = autoPointsMultiplier;

                if (points == 0) points = (int)structuralIntegrity * autoPointsMultiplier;
            }

            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer) _defaultMaterial = spriteRenderer.material;

            if (spriteRenderer && generateBoxCollider && colliders.Length == 0)
            {
                colliders = new BoxCollider2D[] { gameObject.AddComponent<BoxCollider2D>() };

                (colliders[0] as BoxCollider2D).size = spriteRenderer.sprite.bounds.size;
                (colliders[0] as BoxCollider2D).isTrigger = true;
            }

            additionalSettings.Init();

            eventSettings.onAwake.Invoke();
        }
        public virtual void DoDamage(float damage)
        {
            structuralIntegrity -= damage;

            if (structuralIntegrity <= 0)
            {
                structuralIntegrity = 0;

                Kill();
            }
        }
        public void Ghost()
        {
            if (structuralIntegrity > 0)
            {
                eventSettings.onGhost.Invoke();

                if (ghostMaterial)
                {
                    if (spriteRenderer && spriteRenderer.material != ghostMaterial) spriteRenderer.material = ghostMaterial;

                    additionalSettings.Ghost(ghostMaterial);

                    _frameCount = Time.frameCount + ghostSustain;
                }
            }
        }
        public virtual void Kill()
        {
            if (playerLivesCount == true && playerIndex > -1 && FindObjectsOfType<PlayerLivesHelper>().Length == 0) PlayerData.Get(playerIndex).lives -= 1;

            GameData.bodyCount += bodyCount ? 1 : 0;

            OnOutro();

            cargoSettings.Release(this);

            if (dontDestroy)
            {
                suspended = true;
            }
            else
            {
                if (parent)
                {
                    Destroy(parent.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        public override void OnCollision(CollisionBase2D collision)
        {
            _UpdateState(collision);

        }
        public virtual void OnOutro()
        {
            eventSettings.onOutro.Invoke();

            outroSettings.Play(this);
        }

#if UNITY_EDITOR
        void Reset()
        {
            var prefix = "Assets/Playniax/Framework/Ignition/Value Pack/Particle Effects/Prefabs/";

            outroSettings.effectsSettings = new OutroSettings.EffectsSettings[5];

            outroSettings.effectsSettings[0] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[0].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix + "Explosion Fire (Red).prefab", typeof(GameObject));
            outroSettings.effectsSettings[0].sizeInPixels = 80;
            outroSettings.effectsSettings[0].orderInLayer = 0;

            outroSettings.effectsSettings[1] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[1].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix + "Explosion Flash (Red).prefab", typeof(GameObject));
            outroSettings.effectsSettings[1].sizeInPixels = 80;
            outroSettings.effectsSettings[1].orderInLayer = 1;

            outroSettings.effectsSettings[2] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[2].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix + "Explosion Smoke.prefab", typeof(GameObject));
            outroSettings.effectsSettings[2].sizeInPixels = 80;
            outroSettings.effectsSettings[2].orderInLayer = 2;

            outroSettings.effectsSettings[3] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[3].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix + "Explosion Trails (Red).prefab", typeof(GameObject));
            outroSettings.effectsSettings[3].sizeInPixels = 80;
            outroSettings.effectsSettings[3].orderInLayer = 3;

            outroSettings.effectsSettings[4] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[4].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix + "Explosion Blastwave.prefab", typeof(GameObject));
            outroSettings.effectsSettings[4].sizeInPixels = 80;
            outroSettings.effectsSettings[4].orderInLayer = 4;
        }
#endif

        void Update()
        {
            if (Time.frameCount >= _frameCount && spriteRenderer && _defaultMaterial && spriteRenderer.material != _defaultMaterial) spriteRenderer.material = _defaultMaterial;

            if (Time.frameCount >= _frameCount) additionalSettings.RestoreMaterial();
        }

        void _UpdateState(CollisionBase2D collision)
        {
            var collisionState = collision as CollisionState;

            if (collisionState == null) return;

            //if (playerIndex >= 0 && sprite.playerIndex >= 0 && playerIndex == sprite.playerIndex) return;

            if (collisionState.friend != null && collisionState.friend == gameObject) return;
            if (friend != null && friend == collisionState.gameObject) return;

            if (indestructible == true && collisionState.indestructible == true) return;

            if (indestructible)
            {
                if (playerIndex > -1)
                {
                    PlayerData.Get(playerIndex).scoreboard += collisionState.points;

                    OutroSettings.MessengerSettings.Message(collisionState);
                }

                collisionState.Kill();
            }
            else if (collisionState.indestructible)
            {
                if (collisionState.playerIndex > -1)
                {
                    PlayerData.Get(collisionState.playerIndex).scoreboard += points;

                    OutroSettings.MessengerSettings.Message(this);
                }

                Kill();
            }
            else
            {
                var damage1 = structuralIntegrity;
                var damage2 = collisionState.structuralIntegrity;

                DoDamage(damage2);
                collisionState.DoDamage(damage1);

                if (structuralIntegrity > 0) Ghost();
                if (collisionState.structuralIntegrity > 0) collisionState.Ghost();

                if (structuralIntegrity > 0 || collisionState.structuralIntegrity > 0) CollisionAudio.Play(material, collisionState.material);

                if (playerIndex > -1 && collisionState.structuralIntegrity == 0)
                {
                    PlayerData.Get(playerIndex).scoreboard += collisionState.points;

                    OutroSettings.MessengerSettings.Message(collisionState);
                }

                if (collisionState.playerIndex > -1 && structuralIntegrity == 0)
                {
                    PlayerData.Get(collisionState.playerIndex).scoreboard += points;

                    OutroSettings.MessengerSettings.Message(this);
                }
            }
        }

        Material _defaultMaterial;
        int _frameCount;
    }
}