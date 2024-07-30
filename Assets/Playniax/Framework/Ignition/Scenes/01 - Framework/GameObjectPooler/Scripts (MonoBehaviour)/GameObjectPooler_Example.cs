using UnityEngine;
using Playniax.Ignition;

public class GameObjectPooler_Example : MonoBehaviour
{
    [Tooltip("Prefab to be used")]
    public GameObject prefab;
    [Tooltip("Parent to be used")]
    public Transform parent;
    [Tooltip("Timer settings")]
    public Timer timer;

    void Update()
    {
        if (timer.Update())
        {
            // See if object is available.
            GameObject bullet = GameObjectPooler.GetAvailableObject(prefab);
            // Was created?
            if (bullet)
            {
                // Override parent.
                if (parent) bullet.transform.parent = parent;
                // Set position.
                bullet.transform.position = transform.position;
                // Don't forget to activate.
                bullet.SetActive(true);
            }
        }
    }
}
