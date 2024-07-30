using System.Collections.Generic;
using UnityEngine;

namespace Playniax.Ignition
{
    [AddComponentMenu("Playniax/Ignition/GameObjectPooler")]
    // Instantiate() and Destroy() are useful and necessary methods during gameplay. Each generally requires minimal CPU time. However, for objects created during gameplay that have a short lifespan and get destroyed in vast numbers per second, the CPU needs to allocate considerably more time. Additionally, Unity uses Garbage Collection to deallocate memory that’s no longer in use. Repeated calls to Destroy() frequently trigger this task, and it has a knack for slowing down CPUs and introducing pauses to gameplay. This behavior is critical in resource-constrained environments such as mobile devices and web builds. Object pooling is where you pre-instantiate all the objects you’ll need at any specific moment before gameplay — for instance, during a loading screen. Instead of creating new objects and destroying old ones during gameplay, your game reuses objects from a 'pool'.
    public class GameObjectPooler : MonoBehaviour
    {
        [Tooltip("The object to pre-instantiate.")]
        public GameObject prefab;
        [Tooltip("Number of objects to pre-instantiate.")]
        public int initialize;
        [Tooltip("Whether to allow the number instantiated objects to grow or not.")]
        public bool allowGrowth;
        [Tooltip("Whether to hide the objects in the hierarchy or not.")]
        public bool hideInHierarchy;
        public static int GetIndex(string name, int id)
        {
            for (int i = 0; i < _objectPooler.Count; i++)
                if (_objectPooler[i] && _objectPooler[i].prefab && _objectPooler[i].prefab.name == name && _objectPooler[i].prefab.GetInstanceID() == id) return i;

            return -1;
        }
        public static GameObject GetAvailableObject(int index)
        {
            //if (_objectPooler.Count < index) return null;

            if (_objectPooler[index] == null) return null;
            if (_objectPooler[index].prefab == null) return null;

            for (int i = 0; i < _objectPooler[index]._pool.Count; i++)
            {
                if (_objectPooler[index]._pool[i] && !_objectPooler[index]._pool[i].activeInHierarchy)
                {
                    GameObject obj = _objectPooler[index]._pool[i];
                    obj.SetActive(false);
                    obj.transform.position = _objectPooler[index].prefab.transform.position;
                    obj.transform.localScale = _objectPooler[index].prefab.transform.localScale;
                    return obj;
                }
            }

            if (_objectPooler[index].allowGrowth == false) return null;

            GameObject instance = Instantiate(_objectPooler[index].prefab);
            instance.SetActive(false);
            instance.name = instance.name.Replace("(Clone)", ObjectPooler.MARKER);
            if (_objectPooler[index].hideInHierarchy) instance.hideFlags = HideFlags.HideInHierarchy;
            _objectPooler[index]._pool.Add(instance);
            return instance;
        }
        public static GameObject GetAvailableObject(GameObject prefab)
        {
            if (prefab == null) return null;

            //if (prefab.activeInHierarchy) prefab.SetActive(false);

            int index = GetIndex(prefab.name, prefab.GetInstanceID());
            if (index == -1)
            {
                GameObject instance = Instantiate(prefab);
                instance.SetActive(false);
                return instance;
            }
            else
            {
                return GetAvailableObject(index);
            }
        }
        void Awake()
        {
            if (prefab == null || _objectPooler == null) return;
            if (prefab.scene.rootCount > 0) prefab.SetActive(false);

            for (int i = 0; i < initialize; i++)
            {
                GameObject instance = Instantiate(prefab);
                instance.name = instance.name.Replace("(Clone)", ObjectPooler.MARKER);
                instance.SetActive(false);
                if (hideInHierarchy) instance.hideFlags = HideFlags.HideInHierarchy;
                _pool.Add(instance);
            }

            _objectPooler.Add(this);
        }
        void OnDestroy()
        {
            _objectPooler.Remove(this);
        }

        static List<GameObjectPooler> _objectPooler = new List<GameObjectPooler>();
        List<GameObject> _pool = new List<GameObject>();
    }
}