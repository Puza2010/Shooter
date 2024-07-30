using UnityEngine;

namespace Playniax.Pyro
{
    public class TrailsParticle : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public float scale;
        public Vector3 startScale;
        public Vector3 targetScale;
        public bool scaleDownOvertime;
        public Vector3 startPosition;
        public Vector3 targetPosition;
        public Color startColor;
        public Color targetColor;
        public float ttl;
        public float timeScale;
        public float timer;

        public void Update()
        {
            spriteRenderer.color = targetColor - (targetColor - startColor) * (timer / ttl);
            transform.localPosition = targetPosition - (targetPosition - startPosition) * (timer / ttl);
            transform.localScale = targetScale - (targetScale - startScale) * (timer / ttl);
            transform.localScale *= scale;

            if (scaleDownOvertime) transform.localScale *= timer / ttl;

            timer -= timeScale * Time.deltaTime;

            if (timer <= 0) Destroy(gameObject);
        }
    }
}