#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Playniax.Pyro
{
    [AddComponentMenu("Playniax/Pyro/AlphaEffects")]
    // Sprite alpha effects with support for fade in, fade out and ping pong mode.
    public class AlphaEffects : MonoBehaviour
    {

#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(AlphaEffects))]

        public class Inspector : Editor
        {
            SerializedProperty fadeInSettings;
            SerializedProperty fadeOutSettings;
            SerializedProperty pingPongSettings;
            void OnEnable()
            {
                fadeInSettings = serializedObject.FindProperty("fadeInSettings");
                fadeOutSettings = serializedObject.FindProperty("fadeOutSettings");
                pingPongSettings = serializedObject.FindProperty("pingPongSettings");
            }

            override public void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();

                serializedObject.Update();

                var myScript = target as AlphaEffects;

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(myScript), typeof(AlphaEffects), false);
                EditorGUI.EndDisabledGroup();

                myScript.mode = (Mode)EditorGUILayout.EnumPopup("Mode", myScript.mode);

                if (myScript.mode == Mode.PingPong)
                {
                    EditorGUILayout.PropertyField(pingPongSettings, new GUIContent("PingPong Settings"));
                }
                else if (myScript.mode == Mode.FadeIn)
                {
                    EditorGUILayout.PropertyField(fadeInSettings, new GUIContent("Fade In Settings"));
                }
                else if (myScript.mode == Mode.FadeOut)
                {
                    EditorGUILayout.PropertyField(fadeOutSettings, new GUIContent("Fade Out Settings"));
                }

                myScript.spriteRenderer = (SpriteRenderer)EditorGUILayout.ObjectField("Sprite Renderer", myScript.spriteRenderer, typeof(SpriteRenderer), true);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(myScript);

                    serializedObject.ApplyModifiedProperties();
                }
            }

        }
#endif

        [System.Serializable]
        // Fade in datatype.
        public class FadeInSettings
        {
            [Tooltip("Fade speed.")]
            public float speed = 1;
        }

        [System.Serializable]
        // Fade out datatype.
        public class FadeOutSettings
        {
            [Tooltip("Fade speed.")]
            public float speed = 1;
            [Tooltip("Timer.")]
            public float timer;
        }

        [System.Serializable]
        // Ping pong datatype.
        public class PingPongSettings
        {
            [Tooltip("Fade speed.")]
            public float speed = 1;
            [Tooltip("Lowest possible value.")]
            public float min = .25f;
            [Tooltip("Highest possible value.")]
            public float max = 1;
            [Tooltip("Whether to start at random value or not.")]
            public bool startRandom;
        }

        [Tooltip("Mode of alpha effects.")]
        public enum Mode { PingPong, FadeOut, FadeIn }

        // Mode can be Mode.PingPong, Mode.FadeOut or Mode.FadeIn
        [Tooltip("Mode of the alpha effects.")]
        public Mode mode = Mode.PingPong;

        // Have a look at the fade in datatype for settings.
        [Tooltip("Settings for fading in.")]
        public FadeInSettings fadeInSettings = new FadeInSettings();

        // Have a look at the fade out datatype for settings.
        [Tooltip("Settings for fading out.")]
        public FadeOutSettings fadeOutSettings = new FadeOutSettings();

        // Have a look at the ping pong datatype for settings.
        [Tooltip("Settings for ping pong effect.")]
        public PingPongSettings pingPongSettings = new PingPongSettings();

        // spriteRenderer AlphaEffects is using.
        [Tooltip("SpriteRenderer component used for alpha effects.")]
        public SpriteRenderer spriteRenderer;

        void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null) _color = spriteRenderer.color;

            if (mode == Mode.PingPong)
            {
                if (pingPongSettings.startRandom)
                {
                    var color = spriteRenderer.color;

                    color.a = Random.Range(pingPongSettings.min, pingPongSettings.max);

                    if (color.a < pingPongSettings.min)
                    {
                        color.a = pingPongSettings.min;

                        pingPongSettings.speed = Mathf.Abs(pingPongSettings.speed);
                    }

                    if (color.a > pingPongSettings.max)
                    {
                        color.a = pingPongSettings.max;

                        pingPongSettings.speed = -Mathf.Abs(pingPongSettings.speed);
                    }

                    spriteRenderer.color = color;
                }
            }
            else if (mode == Mode.FadeIn)
            {
                var color = spriteRenderer.color;
                color.a = 0;
                spriteRenderer.color = color;
            }
        }

        void OnDisable()
        {
            if (spriteRenderer != null) spriteRenderer.color = _color;
        }

        void Update()
        {
            if (mode == Mode.PingPong)
            {
                _UpdatePingPong();
            }
            else if (mode == Mode.FadeOut)
            {
                _UpdateFadeOut();
            }
            else if (mode == Mode.FadeIn && spriteRenderer.color.a != 1)
            {
                _UpdateFadeIn();
            }
        }

        void _UpdateFadeIn()
        {
            var color = spriteRenderer.color;

            color.a += fadeOutSettings.speed * Time.deltaTime;

            if (color.a > 1)
            {
                color.a = 1;

                spriteRenderer.color = color;
            }
            else
            {
                spriteRenderer.color = color;
            }
        }

        void _UpdateFadeOut()
        {
            if (fadeOutSettings.timer > 0)
            {
                fadeOutSettings.timer -= Time.deltaTime;

                if (fadeOutSettings.timer <= 0) fadeOutSettings.timer = 0;
            }
            else
            {
                var color = spriteRenderer.color;

                color.a -= fadeOutSettings.speed * Time.deltaTime;

                if (color.a < 0)
                {
                    color.a = 0;

                    spriteRenderer.color = color;

                    Destroy(gameObject);
                }
                else
                {
                    spriteRenderer.color = color;
                }
            }
        }

        void _UpdatePingPong()
        {
            var color = spriteRenderer.color;

            color.a += pingPongSettings.speed * Time.deltaTime;

            if (color.a < pingPongSettings.min)
            {
                color.a = pingPongSettings.min;

                pingPongSettings.speed = Mathf.Abs(pingPongSettings.speed);
            }

            if (color.a > pingPongSettings.max)
            {
                color.a = pingPongSettings.max;

                pingPongSettings.speed = -Mathf.Abs(pingPongSettings.speed);
            }

            spriteRenderer.color = color;
        }

        Color _color;
    }
}
