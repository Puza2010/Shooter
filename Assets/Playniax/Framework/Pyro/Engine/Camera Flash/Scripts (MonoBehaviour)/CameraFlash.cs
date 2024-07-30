using UnityEngine;

namespace Playniax.Pyro
{

    public class CameraFlash : MonoBehaviour
    {
        public static void Flash(Camera camera, float duration, float r = 1, float g = 1, float b = 1)
        {
            if (_instance == null)
            {
                _instance = camera.gameObject.AddComponent<CameraFlash>();
                _instance._Init(duration, r, g, b);
            }
            else
            {
                _instance._Flash(duration, r, g, b);
            }
        }

        void _Init(float duration, float r, float g, float b)
        {
            var shader = Shader.Find("Custom/CameraFlashShader");

            _material = new Material(shader);

            _Flash(duration, r, g, b);
        }

        void _Flash(float duration, float r, float g, float b)
        {
            _material.color = new Color(r, g, b, 1f);
            _timer = 0;
            _duration = duration;
            _state = 0;
        }

        void Update()
        {
            if (_state == 0)
            {
                if (_timer < _duration)
                {
                    _alpha = 1f - (_timer / _duration);

                    _material.color = new Color(_material.color.r, _material.color.g, _material.color.b, _alpha);

                    _timer += Time.deltaTime;
                }
                else
                {
                    _state = -1;
                }
            }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_state == -1)
            {
                Graphics.Blit(src, dest);
            }
            else if (_state == 0)
            {
                Graphics.Blit(src, dest);
                Graphics.Blit(src, dest, _material);
            }
        }

        static CameraFlash _instance;

        float _alpha;
        Color _color;
        float _duration;
        Material _material;
        int _state = -1;
        float _timer = 0f;
    }
}