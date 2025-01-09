using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner instance;
    public GameObject damageNumberPrefab;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SpawnDamageNumber(float damage, Vector3 position)
    {
        if (damageNumberPrefab == null || damage <= 0) return;
        
        GameObject numberObj = Instantiate(damageNumberPrefab, position, Quaternion.identity);
        DamageNumber damageNumber = numberObj.GetComponent<DamageNumber>();
        if (damageNumber != null)
        {
            damageNumber.SetDamageText(damage);
        }
    }
} 