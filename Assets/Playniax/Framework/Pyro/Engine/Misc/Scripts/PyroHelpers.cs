using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class PyroHelpers
    {
        public static void OverrideStructuralIntegrity(string name, GameObject instance, IScoreBase scoreBase = null)
        {
            var virtualFloats = VirtualFloats.instance;
            if (virtualFloats == null) return;
 
            if (scoreBase == null) scoreBase = instance.GetComponent<IScoreBase>();
            if (scoreBase == null) return;

            var property = virtualFloats.Get(name + ".structuralIntegrity");
            if (property == null) return;

            scoreBase.structuralIntegrity = property.value;
        }
    }
}