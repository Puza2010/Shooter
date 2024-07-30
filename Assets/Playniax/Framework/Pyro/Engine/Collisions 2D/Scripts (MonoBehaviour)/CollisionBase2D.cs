// https://www.jacksondunstan.com/articles/3753

using UnityEngine;

namespace Playniax.Pyro
{
    [DefaultExecutionOrder(750)]
    public class CollisionBase2D : MonoBehaviour
    {
        public Collider2D[] colliders = new Collider2D[0];
        public bool suspended;
        public string id;
        public string group = "Enemy";
        public int delay = 1;

        public int frameStart { get; set; }

        public virtual void Awake()
        {
            //frameStart = Time.frameCount + delay;

            if (colliders.Length == 0) colliders = GetComponentsInChildren<Collider2D>();
        }

        public virtual bool isAllowed
        {
            get { return true; }
        }

        public virtual void OnEnable()
        {
            frameStart = Time.frameCount + delay;

            if (colliders.Length > 0) CollisionMonitor2D.Add(group, this);
        }

        public virtual void OnDisable()
        {
            CollisionMonitor2D.Remove(group, this);

            suspended = false;
        }

        public virtual void OnCollision(CollisionBase2D collision)
        {
        }
    }
}