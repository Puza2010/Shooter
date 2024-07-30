using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition
{
    public class Effects : MonoBehaviour
    {
        public Text messenger;

        public bool debugKeys = false;

        public GameObject flash;
        public Image flashImage;
        public float flashOffset;
        public float flashTarget;

        public GameObject pauseFader;
        public Image pauseFaderImage;
        public float pauseFaderTarget;
        public float pauseFaderOffset;

        public GameObject gameFader;
        public Image gameFaderImage;
        public float gameFaderOffset;
        public float gameFaderTarget;

        public GameObject screenFader;
        public Image screenFaderImage;
        public float screenFaderOffset;
        public float screenFaderTarget;

        public bool isBusy
        {
            get
            {
                return flash.activeInHierarchy || pauseFader.activeInHierarchy || gameFader.activeInHierarchy || screenFader.activeInHierarchy || messenger.isActiveAndEnabled;
            }
        }

        public void Flash(float speed = .5f)
        {
            Flash(new Color(1, 1, 1, 1), speed = .5f);
        }

        public void Flash(Color color, float speed = .5f)
        {
            flashImage.color = color;

            flashTarget = 0;

            flashOffset = -Mathf.Abs(speed);

            flash.SetActive(true);
        }

        public void Message(Font font, int fontSize, Color color, string text, float sustain = 1, float scaleStep = 0)
        {
            if (messenger == null) return;

            messenger.text = text;
            messenger.font = font;
            messenger.fontSize = fontSize;
            messenger.color = color;

            messenger.transform.localScale = new Vector3(1, 1, 1);

            messenger.gameObject.SetActive(true);

            _messengerTimer = sustain;
            _messengerScaleStep = scaleStep;
        }

        public void SetGameFader(float offset)
        {
            if (offset > 0)
            {
                gameFaderTarget = 1;
            }
            else if (offset < 0)
            {
                gameFaderTarget = 0;
            }

            gameFaderOffset = offset;

            gameFader.SetActive(true);
        }

        public void SetGameFader(float offset, Color color)
        {
            gameFaderImage.color = color;

            SetGameFader(offset);
        }

        public void SetPauseFader(float offset, float target = .5f)
        {
            if (offset > 0)
            {
                if (pauseFader.activeInHierarchy == false && pauseFaderImage.color.a <= target) pauseFaderImage.color = new Color(pauseFaderImage.color.r, pauseFaderImage.color.g, pauseFaderImage.color.b, 0);

                pauseFaderTarget = Mathf.Abs(target);
            }
            else if (offset < 0)
            {
                pauseFaderTarget = 0;
            }

            pauseFaderOffset = offset;

            pauseFader.SetActive(true);
        }

        public void SetScreenFader(float offset)
        {
            if (offset > 0)
            {
                screenFaderTarget = 1;
            }
            else if (offset < 0)
            {
                screenFaderTarget = 0;
            }

            screenFaderOffset = offset;

            screenFader.SetActive(true);
        }

        void Awake()
        {
            messenger.gameObject.SetActive(false);

            flash.SetActive(false);

            gameFaderImage.color = new Color(0, 0, 0, 0);
            gameFader.SetActive(false);

            pauseFaderImage.color = new Color(0, 0, 0, 0);
            pauseFader.SetActive(false);

            screenFaderImage.color = new Color(0, 0, 0, 0);
            screenFader.SetActive(false);
        }

        void Update()
        {
            _UpdateMessenger();

            flashOffset = _UpdateFader(flash, flashImage, flashOffset, flashTarget);
            gameFaderOffset = _UpdateFader(gameFader, gameFaderImage, gameFaderOffset, gameFaderTarget);

            pauseFaderOffset = _UpdateFader(pauseFader, pauseFaderImage, pauseFaderOffset, pauseFaderTarget);
            screenFaderOffset = _UpdateFader(screenFader, screenFaderImage, screenFaderOffset, screenFaderTarget);

#if UNITY_EDITOR
            if (debugKeys) _DebugKeys();
#endif
        }

#if UNITY_EDITOR
        void _DebugKeys()
        {
            if (Input.GetKey(KeyCode.W)) SetGameFader(1);
            if (Input.GetKey(KeyCode.E)) SetGameFader(-1);

            if (Input.GetKey(KeyCode.A)) SetPauseFader(1);
            if (Input.GetKey(KeyCode.S)) SetPauseFader(-1);

            if (Input.GetKey(KeyCode.Z)) Flash();

            if (Input.GetKey(KeyCode.Y)) SetScreenFader(1);
            if (Input.GetKey(KeyCode.U)) SetScreenFader(-1);
        }
#endif

        float _UpdateFader(GameObject fader, Image image, float offset, float target)
        {
            if (offset == 0) return 0;

            Color color = new Color(image.color.r, image.color.g, image.color.b, image.color.a);

            if (offset > 0 && color.a < target)
            {
                color.a += offset * Time.unscaledDeltaTime;

                if (color.a > target)
                {
                    offset = 0;
                    color.a = target;
                }
            }
            else if (offset < 0 && color.a > target)
            {
                color.a += offset * Time.unscaledDeltaTime;

                if (color.a < target)
                {
                    offset = 0;
                    color.a = target;
                }

                if (color.a == 0) fader.SetActive(false);
            }

            image.color = color;

            return offset;
        }

        void _UpdateMessenger()
        {
            if (messenger == null) return;

            if (messenger.gameObject.activeInHierarchy == false) return;

            if (_messengerTimer > 0)
            {
                _messengerTimer -= 1 * Time.deltaTime;
            }
            else
            {
                Color color = messenger.color;

                color.a -= 1 * Time.deltaTime;

                if (color.a < 0) messenger.gameObject.SetActive(false);

                messenger.color = color;
            }

            messenger.transform.localScale += _messengerScaleStep * new Vector3(1, 1, 0) * Time.deltaTime;
        }

        float _messengerTimer;
        float _messengerScaleStep;
    }
}