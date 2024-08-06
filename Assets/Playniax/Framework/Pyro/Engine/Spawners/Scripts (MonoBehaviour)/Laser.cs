using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class Laser : MonoBehaviour
    {
        public SpriteRenderer beam;
        public SpriteRenderer sourceGlow;
        public SpriteRenderer targetGlow;

        public static void Fire(GameObject prefab, int playerIndex, int orderInLayer, Timer timer, GameObject origin, float range = 0, float ttl = .25f, float size = 1, int damage = 1, AudioProperties audioProperties = null, int index = 0)
        {
            if (origin == null) return;
            if (prefab == null) return;

            var target = LaserTarget.GetClosest(index, origin, range);
            if (target == null) return;

            if (timer.counter == 0) return;

            if (target.gameObject == null) return;
            if (target.isActiveAndEnabled == false) return;

            if (TilemapHelpers.RayIntersectingWithTilemap(origin.transform.position, target.transform.position) == true) return;

            if (timer.Update() == false) return;

            var laser = Instantiate(prefab).GetComponent<Laser>();
            laser.Fire(playerIndex, orderInLayer, origin, target, ttl, size, damage);

            if (audioProperties != null) audioProperties.Play();
        }

        public void Fire(int playerindex, int orderInLayer, GameObject origin, LaserTarget target, float ttl = .25f, float size = 1, int damage = 1)
        {
            if (origin == null || target == null) return;
            if (origin.gameObject == null || target.gameObject == null) return;
            if (origin.gameObject.activeSelf == false || target.isActiveAndEnabled == false) return;

            gameObject.SetActive(true);

            beam.sortingOrder = orderInLayer;
            sourceGlow.sortingOrder = orderInLayer + 1;
            targetGlow.sortingOrder = orderInLayer + 1;

            _playerIndex = playerindex;
            _damage = damage;
            _size = size;
            _origin = origin;
            _target = target;
            _ttl = ttl;
            _ttlTimer = ttl;

            _UpdateLaser();
        }

        void LateUpdate()
        {
            _UpdateLaser();
        }

        float _GetAngle(float x1, float y1, float x2, float y2)
        {
            return Mathf.Atan2(y1 - y2, x1 - x2) * Mathf.Rad2Deg;
        }

        void _UpdateLaser()
        {
            if (_origin != null && _target != null)
            {
                var angle = _GetAngle(_target.gameObject.transform.position.x, _target.gameObject.transform.position.y, _origin.transform.position.x, _origin.transform.position.y);

                beam.transform.localRotation = Quaternion.Euler(0, 0, angle);

                transform.GetChild(0).transform.position = _origin.transform.position;

                beam.color = _targetColor - (_targetColor - _startColor) * (_ttlTimer / _ttl);

                beam.transform.localScale = new Vector3(Vector3.Distance(_origin.transform.position, _target.gameObject.transform.position) / (beam.sprite.rect.width / beam.sprite.pixelsPerUnit), _size, 1);

                sourceGlow.color = new Color(sourceGlow.color.r, sourceGlow.color.g, sourceGlow.color.b, beam.color.a);
                targetGlow.color = new Color(targetGlow.color.r, targetGlow.color.g, targetGlow.color.b, beam.color.a);

                sourceGlow.transform.position = _origin.transform.position;
                targetGlow.transform.position = _target.gameObject.transform.position;

                _ttlTimer -= 1 * Time.deltaTime;

                if (_ttlTimer < 0)
                {
                    Destroy(_target.gameObject); // Destroy the target (bullet or enemy)
                    _target = null;
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                }
            }
            else
            {
                _origin = null;
                _target = null;
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }

        int _playerIndex = -1;
        int _damage = 1;
        float _size = 1;
        float _ttl = 1;
        float _ttlTimer = 1;

        GameObject _origin;
        LaserTarget _target;

        Color _startColor = new Color(1, 1, 1, 1);
        Color _targetColor = new Color(1, 1, 1, 0);
    }
}
