using UnityEngine;

public class DemoInstantiator : MonoBehaviour
{
    public GameObject[] prefabs;

    public void Play()
    {
        Vector2 position = _GetRandomPosition();

        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject prefab = prefabs[i];

            if (prefab == null) continue;

            Instantiate(prefab, position, Quaternion.identity);
        }
    }

    Vector2 _GetRandomPosition()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float randomX = Random.Range(0f, screenWidth);
        float randomY = Random.Range(0f, screenHeight);

        Vector2 randomPosition = Camera.main.ScreenToWorldPoint(new Vector3(randomX, randomY, 0));

        return randomPosition;
    }
}