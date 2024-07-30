using System.Collections.Generic;
using UnityEngine;

namespace Playniax.Ignition
{
    // Allows for dynamic creation, retrieval, and modification of string properties during runtime, providing flexibility in managing game parameters.
    //
    // Properties are automatically destroyed when a new scene is loaded.
    public class VirtualStrings : MonoBehaviour
    {
        [System.Serializable]
        public class Property
        {
            public string key;
            public string value;
        }

        public List<Property> properties = new List<Property>();

        public static VirtualStrings instance
        {
            get
            {
                return _Init();
            }
        }

        static VirtualStrings _Init()
        {
            if (_instance == null)
            {
                VirtualStrings[] instances = FindObjectsOfType<VirtualStrings>();

                if (instances.Length == 0)
                {
                    _instance = new GameObject("Virtual Strings").AddComponent<VirtualStrings>();
                }
                else if (instances.Length == 1)
                {
                    _instance = instances[0];
                }
                else
                {
                    _instance = new GameObject("Virtual Strings: ").AddComponent<VirtualStrings>();

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
        public string Get(string key, string defaultValue)
        {
            for (int i = 0; i < properties.Count; i++)
                if (properties[i].key == key) return properties[i].value;

            return defaultValue;
        }

        // Sets the value of a property with the given key in the properties list.
        public void Set(string key, string value)
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
            VirtualStrings[] instances = FindObjectsOfType<VirtualStrings>();

            if (instances.Length > 1) _instance = null;
        }
        void OnDestroy()
        {
            VirtualStrings[] instances = FindObjectsOfType<VirtualStrings>();

            if (instances.Length == 0) _instance = null;
        }
        void _Merge(VirtualStrings source)
        {
            for (int i = 0; i < source.properties.Count; i++)
            {
                bool contains = Contains(source.properties[i].key);

                if (contains == false) Set(source.properties[i].key, source.properties[i].value);
            }
        }

        static VirtualStrings _instance;
    }
}