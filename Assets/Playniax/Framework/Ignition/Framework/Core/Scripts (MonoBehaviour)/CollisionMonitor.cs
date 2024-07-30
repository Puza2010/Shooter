using System.Collections.Generic;
using UnityEngine;

namespace Playniax.Ignition
{
    public class CollisionMonitor : MonoBehaviour
    {
        public string group1 = "Player";
        public string group2 = "Enemy";

        public static bool Check(string a, string b)
        {
            for (int i = 0; i < _list.Count; i++)
                if (_list[i].group1 == a && _list[i].group2 == b || _list[i].group1 == b && _list[i].group2 == a) return true;

            return false;
        }

        public void OnDisable()
        {
            _list.Remove(this);
        }

        public void OnEnable()
        {
            _list.Add(this);
        }

        static List<CollisionMonitor> _list = new List<CollisionMonitor>();
    }
}
