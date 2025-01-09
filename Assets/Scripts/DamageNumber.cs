using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float lifetime = 1f;
    public float moveSpeed = 1f;
    public float fadeSpeed = 1f;
    
    private TextMeshPro textMesh;
    private Color textColor;
    private float timeAlive;
    
    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.color = new Color(1f, 0f, 0f, 1f);
        textColor = textMesh.color;
    }
    
    void Update()
    {
        // Move up
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        
        // Fade out
        timeAlive += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timeAlive * fadeSpeed);
        textMesh.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
        
        // Destroy when lifetime is over
        if (timeAlive >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    public void SetDamageText(float damage)
    {
        int damageInt = Mathf.FloorToInt(damage);
        if (damageInt > 0)
        {
            textMesh.text = damageInt.ToString();
        }
    }
} 