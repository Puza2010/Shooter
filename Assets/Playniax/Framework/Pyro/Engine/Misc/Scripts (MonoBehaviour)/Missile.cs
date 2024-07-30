using UnityEngine;

namespace Playniax.Pyro
{
    public class Missile : MonoBehaviour
    {

        //public int playerIndex = -1;
        public int index;
        public float intro = 0;
        public float introSpeed = 8;
        public float speed = 8;
        public float rotationSpeed = 250f;
        public float friction;

        void Update()
        {
            if (_target == null) _target = Targetable.GetClosest(index, gameObject, true);

            if (intro > 0)
            {
                speed -= friction * Time.deltaTime;
                intro -= 1 * Time.deltaTime;
                if (intro < 0) intro = 0;
            }
            else
            {
                if (_target) _direction = _target.transform.position - transform.position;

                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

                var targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            transform.position += transform.right * speed * Time.deltaTime;
        }

        Vector3 _direction;
        Targetable _target;
    }
}