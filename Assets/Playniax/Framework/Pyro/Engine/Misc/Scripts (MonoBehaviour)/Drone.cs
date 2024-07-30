using System.Collections.Generic;
using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class Drone : MonoBehaviour
    {
        public static List<Drone> list = new List<Drone>();

        public GameObject controller;
        public float speedStep = 4;
        public float maxSpeed = 100;
        public float friction = 1;
        public float distanceBetweenDrones = 1;
        public float adjustmentForce = .05f;
        public bool distanceX = true;
        public bool distanceY;

        void OnEnable()
        {
            list.Add(this);
        }

        void OnDisable()
        {
            list.Remove(this);
        }
        void LateUpdate()
        {
            if (controller == null || controller && controller.activeInHierarchy == false) return;

            _MoveTowards();

            if (distanceX || distanceY) _KeepDistance();

            _LimitVelocity();

            transform.position += _velocity * Time.deltaTime;

            if (friction != 0) _velocity *= 1 / (1 + (Time.deltaTime * friction));
        }

        void _LimitVelocity()
        {
            if (_velocity.x > maxSpeed) _velocity.x = maxSpeed;
            if (_velocity.x < -maxSpeed) _velocity.x = -maxSpeed;
            if (_velocity.y > maxSpeed) _velocity.y = maxSpeed;
            if (_velocity.y < -maxSpeed) _velocity.y = -maxSpeed;
        }
        void _MoveTowards()
        {
            var angle = Math2DHelpers.GetAngle(controller, gameObject);
            int distance = (int)(Mathf.Abs(controller.transform.position.x - transform.position.x) + Mathf.Abs(controller.transform.position.y - transform.position.y));

            if (distance > 0)
            {
                _velocity.x += Mathf.Cos(angle) * speedStep * Time.deltaTime;
                _velocity.y += Mathf.Sin(angle) * speedStep * Time.deltaTime;
            }
        }

        void _KeepDistance()
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].isActiveAndEnabled == false) continue;
                if (list[i] == this) continue;

                var distance = Vector3.Distance(list[i].transform.position, transform.position);

                if (distance > 0 && distance < distanceBetweenDrones)
                {
                    var forceValue = adjustmentForce / distance;
                    var forceDirection = (transform.position - list[i].transform.position);

                    if (distanceX) _velocity.x += forceDirection.x * forceValue;
                    if (distanceY) _velocity.y += forceDirection.y * forceValue;
                }
            }
        }

        Vector3 _velocity;
    }
}