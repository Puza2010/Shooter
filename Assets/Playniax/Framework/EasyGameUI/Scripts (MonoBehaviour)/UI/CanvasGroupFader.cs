using UnityEngine;

namespace Playniax.Ignition
{
    public class CanvasGroupFader : MonoBehaviour
    {
        public float fadeSpeed = 5;

        void Awake()
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup) _targetAlpha = _canvasGroup.alpha;
        }

        void Start()
        {
            if (EasyGameUI.instance && _canvasGroup) _canvasGroup.alpha = 0;
        }

        void Update()
        {
            if (EasyGameUI.instance && _canvasGroup)
            {
                if (EasyGameUI.instance.isBusy)
                {
                    _canvasGroup.alpha -= fadeSpeed * Time.unscaledDeltaTime;

                    if (_canvasGroup.alpha < 0) _canvasGroup.alpha = 0;
                }
                else
                {
                    _canvasGroup.alpha += fadeSpeed * Time.unscaledDeltaTime;

                    if (_canvasGroup.alpha > _targetAlpha) _canvasGroup.alpha = _targetAlpha;
                }
            }
        }

        CanvasGroup _canvasGroup;
        float _targetAlpha;
    }
}