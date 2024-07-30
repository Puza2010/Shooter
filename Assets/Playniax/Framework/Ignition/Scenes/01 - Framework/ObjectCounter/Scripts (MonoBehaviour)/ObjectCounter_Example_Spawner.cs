using UnityEngine;
using UnityEngine.UI;

public class ObjectCounter_Example_Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float interval = .2f;
    private float timer = 0.0f;

    public Text text;

    void Awake()
    {
        if (prefab && prefab.scene.rootCount > 0) prefab.SetActive(false);
    }

    void Update()
    {
        // Spawn the objects.
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0;

            GameObject instance = Instantiate(prefab);
            instance.SetActive(true);
        }
    }
}
