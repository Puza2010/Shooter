using System.Linq;
using UnityEngine;

namespace Playniax.Ignition
{
    [DefaultExecutionOrder(-1000)]
    public class IgnitionBehaviour : MonoBehaviour, IIgnitionBehaviour
    {
        public static void Initialize(IIgnitionBehaviour instance)
        {
            if (_instance == null)
            {
                var behaviours = FindObjectsOfType<MonoBehaviour>().OfType<IIgnitionBehaviour>();

                _instance = new GameObject("IgnitionBehaviour = " + behaviours.Count());

                //_instance.hideFlags = HideFlags.HideInHierarchy;

                foreach (IIgnitionBehaviour behaviour in behaviours)
                {
                    if (behaviour.isInitialized == false)
                    {
                        behaviour.OnInitialize();

                        behaviour.isInitialized = true;
                    }
                }
            }
            else
            {
                if (instance.isInitialized == false)
                {
                    instance.OnInitialize();

                    instance.isInitialized = true;
                }
            }
        }
        public bool isInitialized
        {
            get;
            set;
        }
        public virtual void Awake()
        {
            Initialize(this);
        }
        public virtual void OnInitialize()
        {
        }

        static GameObject _instance;
    }
}