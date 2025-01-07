using UnityEngine;

public class FireballRotator : MonoBehaviour 
{
    public float rotationSpeed = 360f; // Degrees per second

    void Update()
    {
        // Rotate clockwise
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }
} 