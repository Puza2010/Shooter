using UnityEngine;

namespace Playniax.Pyro
{
    public interface IBoundsHelpers
    {
        Bounds bounds { get; }
        GameObject gameObject { get; }
        Vector3 size
        {
            get;
            set;
        }
    }
}