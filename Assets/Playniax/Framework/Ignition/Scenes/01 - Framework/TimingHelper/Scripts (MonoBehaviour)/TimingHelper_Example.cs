using UnityEngine;

public class TimingHelper_Example : MonoBehaviour
{
    public float speed = 8;

    private void Start()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2f;
        _screenWidth = screenAspect * cameraHeight;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.x > _screenWidth / 2f + transform.localScale.x / 2f)
        {
            Vector3 newPos = new Vector3(-_screenWidth / 2f - transform.localScale.x / 2f, transform.position.y, transform.position.z);
            transform.position = newPos;
        }
    }

    float _screenWidth;
}
