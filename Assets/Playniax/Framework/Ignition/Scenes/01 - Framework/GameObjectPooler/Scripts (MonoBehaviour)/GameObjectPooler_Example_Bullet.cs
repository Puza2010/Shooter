using UnityEngine;
using Playniax.Ignition;

public class GameObjectPooler_Example_Bullet : MonoBehaviour
{
    void Start()
    {
        // Get renderer.
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Move the bullet.
        transform.localPosition += Vector3.right * 10 * Time.deltaTime;
        if (CameraHelpers.IsVisible(_renderer) == false) gameObject.SetActive(false);
    }

    Renderer _renderer;
}
