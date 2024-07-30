using UnityEngine;
using Playniax.Ignition;

namespace Playniax.SpaceShooterArtPack02
{
    public class Crusher : MonoBehaviour
    {
        public enum StartPosition { Left, Right, Top, Bottom };
        public enum Mode { Fixed, Random, Rotating, Mirrored };

        public StartPosition startPosition = StartPosition.Bottom;
        public Mode mode = Mode.Fixed;
        public int clones;

        public float timer = 3;
        public int count = -1;
        public float enteranceSpeed = 2;
        public float exitSpeed = 15;
        public string interval = "2";
        public GameObject body;

        void OnValidate()
        {
            _Rotate();
        }

        void Awake()
        {
            if (_instance == null && clones > 0)
            {
                _instance = this;

                StartPosition start = _GetStartPosition();

                for (int i = 0; i < clones; i++)
                {
                    var clone = Instantiate(gameObject).GetComponent<Crusher>();

                    clone.startPosition = start;
                    start = clone._GetStartPosition();

                    clone.timer = clone.timer + _RandomFloat(interval);

                    RendererHelpers.IncreaseOrder(gameObject, 1, true);
                }

                _instance = null;
            }

            body.SetActive(false);

            //if (startPosition == StartPosition.Random) startPosition = (StartPosition)Random.Range(0, 4);
        }

        void Update()
        {
            if (count == 0)
            {
                Destroy(gameObject);

                return;
            }

            if (_state == 0)
            {
                timer -= 1 * Time.deltaTime;

                if (timer < 0)
                {
                    _Init();

                    if (mode != Mode.Fixed) _RandomPos();

                    body.SetActive(true);

                    _state += 1;
                }
            }
            else if (_state == 1)
            {
                if (startPosition == StartPosition.Bottom)
                {
                    transform.position += enteranceSpeed * Vector3.up * Time.deltaTime;

                    if (transform.position.y > _start.y + _size.y) _state++;
                }
                else if (startPosition == StartPosition.Top)
                {
                    transform.position += enteranceSpeed * Vector3.down * Time.deltaTime;

                    if (transform.position.y < _start.y - _size.y) _state++;
                }
                else if (startPosition == StartPosition.Right)
                {
                    transform.position += enteranceSpeed * Vector3.left * Time.deltaTime;

                    if (transform.position.x < _start.x - _size.x) _state++;
                }
                else if (startPosition == StartPosition.Left)
                {
                    transform.position += enteranceSpeed * Vector3.right * Time.deltaTime;

                    if (transform.position.x > _start.x + _size.x) _state++;
                }
            }
            else if (_state == 2)
            {
                if (startPosition == StartPosition.Bottom)
                {
                    transform.position += exitSpeed * Vector3.down * Time.deltaTime;

                    if (transform.position.y < _start.y)
                    {
                        body.SetActive(false);
                        transform.position = new Vector3(transform.position.x, _start.y, transform.position.z);
                        timer = _RandomFloat(interval, 1);
                        if (count != -1) count -= 1;
                        startPosition = _GetStartPosition();
                        _state = 0;
                    }
                }
                else if (startPosition == StartPosition.Top)
                {
                    transform.position -= exitSpeed * Vector3.down * Time.deltaTime;

                    if (transform.position.y > _start.y)
                    {
                        body.SetActive(false);
                        transform.position = new Vector3(transform.position.x, _start.y, transform.position.z);
                        timer = _RandomFloat(interval, 1);
                        if (count != -1) count -= 1;
                        startPosition = _GetStartPosition();
                        _state = 0;
                    }
                }
                else if (startPosition == StartPosition.Right)
                {
                    transform.position -= exitSpeed * Vector3.left * Time.deltaTime;

                    if (transform.position.x > _start.x)
                    {
                        body.SetActive(false);
                        transform.position = new Vector3(_start.x, transform.position.y, transform.position.z);
                        timer = _RandomFloat(interval, 1);
                        if (count != -1) count -= 1;
                        startPosition = _GetStartPosition();
                        _state = 0;
                    }
                }
                else if (startPosition == StartPosition.Left)
                {
                    transform.position -= exitSpeed * Vector3.right * Time.deltaTime;

                    if (transform.position.x < _start.x)
                    {
                        body.SetActive(false);
                        transform.position = new Vector3(_start.x, transform.position.y, transform.position.z);
                        timer = _RandomFloat(interval, 1);
                        if (count != -1) count -= 1;
                        startPosition = _GetStartPosition();
                        _state = 0;
                    }
                }
            }
        }

        StartPosition _GetStartPosition()
        {
            if (mode == Mode.Rotating)
            {
                if (startPosition == StartPosition.Left)
                {
                    return StartPosition.Top;
                }
                else if (startPosition == StartPosition.Top)
                {
                    return StartPosition.Right;
                }
                else if (startPosition == StartPosition.Right)
                {
                    return StartPosition.Bottom;
                }
                else if (startPosition == StartPosition.Bottom)
                {
                    return StartPosition.Left;
                }
            }
            else if (mode == Mode.Mirrored)
            {
                if (startPosition == StartPosition.Left)
                {
                    return StartPosition.Right;
                }
                else if (startPosition == StartPosition.Right)
                {
                    return StartPosition.Left;
                }
                else if (startPosition == StartPosition.Top)
                {
                    return StartPosition.Bottom;
                }
                else if (startPosition == StartPosition.Bottom)
                {
                    return StartPosition.Top;
                }
            }

            return startPosition;
        }
        void _Init()
        {
            _min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, transform.position.z - Camera.main.transform.position.z));
            _max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, transform.position.z - Camera.main.transform.position.z));

            _Rotate();

            _size = RendererHelpers.GetSize(gameObject);

            _min.x -= _size.x * .5f;
            _max.x += _size.x * .5f;

            _min.y += _size.y * .5f;
            _max.y -= _size.y * .5f;

            _start = transform.position;

            if (startPosition == StartPosition.Left)
            {
                _start.x = _min.x;
            }
            else if (startPosition == StartPosition.Right)
            {
                _start.x = _max.x;
            }
            else if (startPosition == StartPosition.Top)
            {
                _start.y = _min.y;
            }
            else if (startPosition == StartPosition.Bottom)
            {
                _start.y = _max.y;
            }

            transform.position = _start;
        }

        void _RandomPos()
        {
            var position = transform.position;

            if (startPosition == StartPosition.Bottom || startPosition == StartPosition.Top)
            {
                position.x = Random.Range(_min.x, _max.x);
            }
            else if (startPosition == StartPosition.Left || startPosition == StartPosition.Right)
            {
                position.y = Random.Range(_min.y, _max.y);
            }

            transform.position = position;
        }

        float _RandomFloat(string str, float defaultValue = 0)
        {
            if (str.Trim() == "") return defaultValue;
            string[] r = str.Split(',');
            if (r.Length == 1) return float.Parse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
            float min = float.Parse(r[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
            float max = float.Parse(r[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
            return Random.Range(min, max);
        }

        void _Rotate()
        {
            if (startPosition == StartPosition.Bottom)
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else if (startPosition == StartPosition.Top)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            else if (startPosition == StartPosition.Right)
            {
                transform.rotation = Quaternion.Euler(0, 0, -180);
            }
            else if (startPosition == StartPosition.Left)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        static Crusher _instance;

        Vector3 _min;
        Vector3 _max;
        Vector2 _size;
        SpriteRenderer[] _spriteRenderer;
        Vector3 _start;
        int _state;
    }
}