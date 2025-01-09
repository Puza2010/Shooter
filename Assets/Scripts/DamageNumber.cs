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
        textColor = Color.white; // Default color is white
        textMesh.color = textColor;
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
            // Set color based on damage value
            if (damageInt >= 50)
            {
                textColor = new Color(0.5f, 0f, 0.8f, 1f); // Strong violet
            }
            else if (damageInt >= 30)
            {
                textColor = Color.red;
            }
            else if (damageInt >= 20)
            {
                textColor = new Color(1f, 0.27f, 0f, 1f); // Stronger orange
            }
            else if (damageInt >= 10)
            {
                textColor = Color.yellow;
            }
            else
            {
                textColor = Color.white;
            }
            
            textMesh.color = textColor;
            textMesh.text = damageInt.ToString();
        }
    }
} 