#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.Galaga
{
    // Galaga sprite AI class.
    public class GalagaBehaviour : MonoBehaviour
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(GalagaBehaviour))]
        public class GalagaBehaviourEditor : Editor
        {
            void OnSceneGUI()
            {
                var galagaBehaviour = (GalagaBehaviour)target;

                if (galagaBehaviour.drawGizmos && galagaBehaviour.points.Length > 0)
                {
                    //Handles.DrawAAPolyLine(3, galagaBehaviour.points);

                    for (int i = 0; i < galagaBehaviour.points.Length; i++)
                    {
                        galagaBehaviour.points[i] = Handles.PositionHandle(galagaBehaviour.points[i], Quaternion.identity);
                    }
                }
            }
        }
#endif
        public GalagaGridPosition gridPosition;
        public Randomizer.Position entrance = Randomizer.Position.RandomTop;
        public Randomizer.Position exit = Randomizer.Position.RandomLeftOrRight;
        public float speed = 50;
        public float recoverySpeed = 1000;
        public bool allowRotationWithPath;
        public int rotationOffset;
        public int formationRotationOffset = 90;
        public int randomPoints;
        public Vector3[] points;
        public int loops = -1;
        public bool mirrorPoints = true;
        public bool shufflePoints;
        public float sustain = 3;
        public bool randomSustain;
        public bool ignoreGrid;
        public float timer;
        public Vector2 lastEnterance;
        public Vector2 lastExit;
#if UNITY_EDITOR
        public bool drawGizmos = true;
        public Color gizmoColor = Color.yellow;

        public static void DrawGizmos(Vector3[] points, Color color)
        {
            points = GetPoints(points);

            var previosPositions = Interpolate(points, 0);
            var smoothness = points.Length * 20;

            Gizmos.color = color;

            for (int i = 0; i < smoothness; i++)
            {
                var t = (float)i / smoothness;
                var currentPositions = Interpolate(points, t);

                Gizmos.DrawLine(currentPositions, previosPositions);

                previosPositions = currentPositions;
            }
        }
        void OnDrawGizmos()
        {
            if (drawGizmos == false) return;

            if (Application.isPlaying)
            {
                if (_points == null) return;
                if (_points.Length < 2) return;
                if (_state == 1) return;
                if (_state == 3) return;

                DrawGizmos(_points, gizmoColor);
            }
            else
            {
                if (points == null) return;
                if (points.Length == 0) return;

                DrawGizmos(points, gizmoColor);
            }
        }
#endif
        public bool initialized
        {
            get;
            set;
        }

        public static void ClearPath(ref SpriteRenderer[] renderers)
        {
            if (renderers == null) return;

            var parent = renderers[0].transform.parent.gameObject;

            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] == null) continue;
                    if (renderers[i].gameObject == null) continue;

                    renderers[i].gameObject.SetActive(false);
                }
            }
        }

        public static void DrawPath(ref SpriteRenderer[] renderers, Vector3[] positions, Sprite sprite, Material material, Color color, Vector3 scale, int smoothness = 100)
        {
            GameObject parent;

            if (renderers == null)
            {
                parent = new GameObject("Points");
            }
            else
            {
                parent = renderers[0].transform.parent.gameObject;
            }

            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] == null) continue;
                    if (renderers[i].gameObject == null) continue;

                    renderers[i].gameObject.SetActive(false);
                }
            }

            if (positions.Length < 2) return;

            positions = GetPoints(positions);

            smoothness *= positions.Length;

            if (renderers == null || renderers != null && renderers.Length < smoothness + 0) System.Array.Resize(ref renderers, smoothness + 0);

            for (int i = 0; i < smoothness; i++)
            {
                float t = (float)i / smoothness;

                var currentPositions = Interpolate(positions, t);

                if (renderers[i] == null) renderers[i] = new GameObject("Point " + currentPositions).AddComponent<SpriteRenderer>();

                renderers[i].sprite = sprite;
                renderers[i].color = color;
                renderers[i].transform.localScale = scale;
                renderers[i].transform.localPosition = currentPositions;

                renderers[i].gameObject.transform.parent = parent.transform;

                renderers[i].gameObject.SetActive(true);
            }
        }
        public static Vector3 Interpolate(Vector3[] points, float t)
        {
            var s = points.Length - 3;
            var i = Mathf.Min(Mathf.FloorToInt(t * s), s - 1);
            var u = t * s - i;

            var a = points[i];
            var b = points[i + 1];
            var c = points[i + 2];
            var d = points[i + 3];

            _startPosition = b;
            _targetPosition = c;

            return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
        }

        public static Vector3[] GetPoints(Vector3[] points)
        {
            var newPoints = new Vector3[points.Length + 2];
            System.Array.Copy(points, 0, newPoints, 1, points.Length);

            newPoints[0] = newPoints[1] + (newPoints[1] - newPoints[2]);
            newPoints[newPoints.Length - 1] = newPoints[newPoints.Length - 2] + (newPoints[newPoints.Length - 2] - newPoints[newPoints.Length - 3]);

            if (newPoints[1] == newPoints[newPoints.Length - 2])
            {
                newPoints[0] = newPoints[newPoints.Length - 3];
                newPoints[newPoints.Length - 1] = newPoints[2];
            }

            return newPoints;
        }
        public static Vector3 GetPosition(Vector3[] points, float position)
        {
            return Interpolate(GetPoints(points), position);
        }
        public void Go(Vector2 enterance, Vector2 exit)
        {
            _position = 0;

            _state = 0;

            _points = new Vector3[points.Length + 2];

            int index = 1;

            for (int i = 0; i < points.Length; i++)
            {
                _points[index].x = points[i].x;
                _points[index].y = points[i].y;

                index += 1;
            }

            if (GalagaGrid.instance && ignoreGrid == false)
            {
                var position = GalagaGrid.GetFreePosition();

                if (position)
                {
                    gridPosition = position;
                    gridPosition.occupied = true;
                }
                else
                {
                    if (gridPosition) gridPosition.occupied = false;

                    gridPosition = null;
                }
            }

            transform.position = enterance;

            _points[0] = enterance;
            _points[_points.Length - 1] = exit;

            lastEnterance = enterance;
            lastExit = exit;
        }
        public void Play()
        {
            if (_state == 0)
            {
                if (_position > 1)
                {
                    // Grid?

                    if (gridPosition)
                    {
                        _position = 0;

                        transform.SetParent(gridPosition.transform);

                        transform.localPosition = Vector3.zero;

                        if (mirrorPoints)
                        {
                            for (int i = 0; i < points.Length; i++)
                                points[i] = -points[i];

                            lastEnterance = -lastEnterance;
                            lastExit = -lastExit;
                        }

                        if (shufflePoints) points = ArrayHelpers.Shuffle(points);

                        _state = 1;
                    }
                    else
                    {
                        // No grid.

                        if (loops == 0)
                        {
                            Destroy(gameObject);
                        }
                        else if (loops == -1 || loops > 0)
                        {
                            if (loops > 0) loops -= 1;

                            if (mirrorPoints)
                            {
                                for (int i = 0; i < points.Length; i++)
                                    points[i] = -points[i];

                                lastEnterance = -lastEnterance;
                                lastExit = -lastExit;
                            }

                            if (shufflePoints) points = ArrayHelpers.Shuffle(points);

                            Go(lastEnterance, lastExit);
                        }
                    }
                }
                else
                {
                    // Follow path.

                    if (gridPosition) _points[_points.Length - 1] = gridPosition.transform.position;

                    transform.position = GetPosition(_points, _position);

                    _position += speed / 100 * Time.deltaTime / _points.Length;

                    if (allowRotationWithPath)
                    {
                        transform.right = Interpolate(GetPoints(_points), _position) - transform.position;

                        transform.rotation *= Quaternion.Euler(0, 0, rotationOffset);
                    }
                }
            }

            // Stay idle for a while.

            else if (_state == 1)
            {
                var step = recoverySpeed * Time.deltaTime;
                var targetRotation = gridPosition.transform.rotation * Quaternion.Euler(0, 0, formationRotationOffset);

                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, step);

                timer += Time.deltaTime;

                if (timer >= sustain && transform.parent && transform.parent.GetComponent<GalagaGridPosition>() == gridPosition)
                {
                    timer = 0;

                    transform.SetParent(null);

                    gridPosition.occupied = false;
                    gridPosition = null;

                    int allowTumble = Random.Range(0, 2);

                    if (allowTumble == 0)
                    {
                        Go(transform.position, lastExit);
                    }
                    else if (allowTumble == 1)
                    {
                        var viewportSize = GalagaGrid.GetViewportSize();

                        _tumblePosition = new Vector3(Random.Range(-viewportSize.x, viewportSize.x), Random.Range(-viewportSize.y, viewportSize.y));

                        _rotation = _rotationSpeed;

                        _rotationSpeed = -_rotationSpeed;

                        _state = 2;
                    }
                }
            }

            // Tumble

            else if (_state == 2)
            {
                var velocity = _tumblePosition - transform.position;

                transform.position += velocity * Time.deltaTime;

                var distance = Vector3.Distance(_tumblePosition, transform.position);

                transform.Rotate(new Vector3(0, 0, _rotation) * distance * Time.deltaTime);

                if (distance <= .05f)
                {
                    Go(transform.position, lastExit);
                }
            }
        }
        void OnDisable()
        {
            if (gridPosition)
            {
                gridPosition.occupied = false;

                gridPosition = null;
            }
        }
        void Update()
        {
            if (_points == null || points == null || (points != null && points.Length == 0))
            {
                var spriteSize = GetComponent<SpriteRenderer>().bounds.size / 2;

                var margin = GalagaGrid.GetViewportMargin();

                var windowSize = GalagaGrid.GetViewportSize(margin);

                lastEnterance = Randomizer.GetPosition(entrance, spriteSize, margin);
                lastExit = Randomizer.GetPosition(exit, spriteSize, margin);

                if (randomPoints > 0)
                {
                    points = new Vector3[randomPoints];

                    points = GalagaSpawner.GetRandomPoints(lastEnterance, new Vector2(windowSize.x, windowSize.y), points.Length, GalagaGrid.GetRandomPointsDistance());
                }
                else if (points.Length == 0)
                {
                    points = new Vector3[Random.Range(4, 8)];

                    points = GalagaSpawner.GetRandomPoints(lastEnterance, new Vector2(windowSize.x, windowSize.y), points.Length, GalagaGrid.GetRandomPointsDistance());
                }

                Go(lastEnterance, lastExit);
            }

            Play();
        }
        /*
        Vector2 _Scramble(Vector2 value)
        {
            if (Random.Range(0, 2) == 1) value.x = -value.x;
            if (Random.Range(0, 2) == 1) value.y = -value.y;

            return value;
        }
        */

        float _RandomFloat(string str, float defaultValue = 0)
        {
            if (str.Trim() == "") return defaultValue;
            string[] r = str.Split(',');
            if (r.Length == 1) return float.Parse(str);
            float min = float.Parse(r[0]);
            float max = float.Parse(r[1]);
            return Random.Range(min, max);
        }

        static float _rotationSpeed = 250;
        static Vector3 _startPosition;
        static Vector3 _targetPosition;

        Vector3[] _points;
        float _position;
        float _rotation;
        int _state;
        Vector3 _tumblePosition;
    }
}