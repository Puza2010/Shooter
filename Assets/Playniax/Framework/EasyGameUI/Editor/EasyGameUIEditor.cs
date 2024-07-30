using UnityEditor;

using System.IO;

using UnityEngine;

namespace Playniax.Ignition
{
    [CustomEditor(typeof(EasyGameUI))]
    public class EasyGameUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _Buttons();
        }

        void _Buttons()
        {
            if (Application.isPlaying) return;

            var myScript = (EasyGameUI)target;

            EditorGUILayout.Separator();

            if (GUILayout.Button("Add Scenes To Build"))
            {
                myScript.ScenesInBuild();

                EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }
            if (GUILayout.Button("Open Unity Build Settings Window"))
            {
                EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }
            else if (GUILayout.Button("Set Current Scene Folder"))
            {
                myScript.searchInFolders = new string[] { Path.GetDirectoryName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().path) };
            }
        }
    }
}
