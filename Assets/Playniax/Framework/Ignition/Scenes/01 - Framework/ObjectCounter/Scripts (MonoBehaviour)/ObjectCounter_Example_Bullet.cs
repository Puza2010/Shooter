using UnityEngine;
using Playniax.Ignition;

public class ObjectCounter_Example_Bullet : MonoBehaviour
{
    void Start()
    {
        _renderer = GetComponent<Renderer>();

        _direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
    }

    void Update()
    {
        transform.localPosition += _direction * 10 * Time.deltaTime;
        if (CameraHelpers.IsVisible(_renderer) == false) gameObject.SetActive(false);
    }

    Vector3 _direction;
    Renderer _renderer;
}
