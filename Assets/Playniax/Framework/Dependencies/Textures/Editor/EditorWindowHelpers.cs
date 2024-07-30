// https://docs.unity3d.com/ScriptReference/MenuItem.html
// https://answers.unity.com/questions/22947/adding-to-the-context-menu-of-the-hierarchy-tab.html

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Framework.Editor
{
    public class EditorWindowHelpers : EditorWindow
    {
        public const int TAB_SPACE = 20;

        public static Canvas AddCanvas()
        {
            var canvas = new GameObject("Canvas").AddComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<CanvasScaler>();
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            Undo.RegisterCreatedObjectUndo(canvas.gameObject, "Create object");

            Selection.activeGameObject = canvas.gameObject;

            return canvas;
        }

        public static GUIContent Buttons(GUIContent[] content, float buttonWidth = 64, float buttonHeight = 64)
        {
            int count = (int)(EditorGUIUtility.currentViewWidth / buttonWidth);

            int counter = 0;

            GUILayout.BeginHorizontal();

            for (int i = 0; i < content.Length; i++)
            {
                if (GUILayout.Button(content[i], GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
                {
                    GUILayout.EndHorizontal();

                    return content[i];
                }

                counter += 1;

                if (counter >= count)
                {
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    counter = 0;
                }
            }

            GUILayout.EndHorizontal();

            return null;
        }

        public static Canvas GetCanvas()
        {
            Canvas canvas;

            var all = FindObjectsOfType<Canvas>();
            if (all.Length == 0) return null;

            var active = Selection.activeGameObject;

            if (active)
            {
                canvas = active.GetComponent<Canvas>();
                if (canvas) return canvas;
            }

            if (all[0]) return all[0];

            return null;
        }

        public static GameObject GetAssetAtPath(string path, bool allowAsChild = false)
        {
            GameObject gameObject;

            var active = Selection.activeGameObject;

            if (allowAsChild && active)
            {
                gameObject = Instantiate(AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject, active.transform);
            }
            else
            {
                gameObject = Instantiate(AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject);
            }

            if (gameObject)
            {
                gameObject.name = gameObject.name.Replace("(Clone)", "");

                Undo.RegisterCreatedObjectUndo(gameObject, "Create object");

                Selection.activeGameObject = gameObject;
            }

            return gameObject;
        }

        public static Texture2D GetPreview(string path, int width = 32, int height = 32)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var editor = UnityEditor.Editor.CreateEditor(prefab);
            var texture = editor.RenderStaticPreview(path, null, width, height);
            EditorWindow.DestroyImmediate(editor);
            return texture;
        }
    }
}
