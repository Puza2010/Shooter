using UnityEngine;

namespace Playniax.Ignition
{
    [AddComponentMenu("Playniax/Ignition/Self Deactivator")]
    // Disables the GameObject.
    public class SelfDeactivator : IgnitionBehaviour
    {
        public override void OnInitialize()
        {
            gameObject.SetActive(false);
        }
    }
}
