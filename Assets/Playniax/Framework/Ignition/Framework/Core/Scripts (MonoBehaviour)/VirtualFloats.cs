using System.Collections.Generic;
using UnityEngine;

namespace Playniax.Ignition
{
    // Allows for dynamic creation, retrieval, and modification of float properties during runtime, providing flexibility in managing game parameters.
    //
    // Properties are automatically destroyed when a new scene is loaded.
    public class VirtualFloats : MonoBehaviour
    {
        [System.Serializable]
        public class Property
        {
            public string key;
            public float value;
        }

        public List<Property> properties = new List<Property>();

        public static VirtualFloats instance
        {
            get
            {
                return _Init();
            }
        }

        static VirtualFloats _Init()
        {
            if (_instance == null)
            {
                VirtualFloats[] instances = FindObjectsOfType<VirtualFloats>();

                if (instances.Length == 0)
                {
                    _instance = new GameObject("Virtual Floats").AddComponent<VirtualFloats>();
                }
                else if (instances.Length == 1)
                {
                    _instance = instances[0];
                }
                else
                {
                    _instance = new GameObject("Virtual Floats: ").AddComponent<VirtualFloats>();

                    for (int i = 0; i < instances.Length; i++)
                    {
#if UNITY_EDITOR
                        _instance.name += instances[i].gameObject.name;

                        if (i < instances.Length - 1) _instance.name += " + ";
#endif
                        _instance._Merge(instances[i]);

                        Destroy(instances[i].gameObject);
                    }
                }

            }

            return _instance;
        }

        // Checks if a property with the given key exists in the properties list.
        public bool Contains(string key)
        {
            for (int i = 0; i < properties.Count; i++)
                if (properties[i].key == key) return true;

            return false;
        }

        // Retrieves a property with the given key from the properties list.
        public Property Get(string key)
        {
            for (int i = 0; i < properties.Count; i++)
                if (properties[i].key == key) return properties[i];

            return null;
        }

        // Retrieves the value of a property with the given key from the properties list.
        //
        // If no property with the given key is found, returns the default value specified.
        public float Get(string key, float defaultValue)
        {
            for (int i = 0; i < properties.Count; i++)
                if (properties[i].key == key) return properties[i].value;

            return defaultValue;
        }

        // Sets the value of a property with the given key in the properties list.
        public void Set(string key, float value)
        {
            Property property = Get(key);

            if (property == null)
            {
                property = new Property();
                property.key = key;
                property.value = value;
                properties.Add(property);
            }
            else
            {
                property.value = value;
            }
        }

        void Awake()
        {
            VirtualFloats[] instances = FindObjectsOfType<VirtualFloats>();

            if (instances.Length > 1) _instance = null;
        }

        void OnDestroy()
        {
            VirtualFloats[] instances = FindObjectsOfType<VirtualFloats>();

            if (instances.Length == 0) _instance = null;
        }
        void _Merge(VirtualFloats source)
        {
            for (int i = 0; i < source.properties.Count; i++)
            {
                bool contains = Contains(source.properties[i].key);

                if (contains == false) Set(source.properties[i].key, source.properties[i].value);
            }
        }

        static VirtualFloats _instance;
    }
}