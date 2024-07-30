using UnityEngine;
using UnityEditor;
using Playniax.Ignition;
public class GameDataWindow : EditorWindow
{

    [MenuItem("Tools/Playniax/Monitor/Ignition/Game Data", false, 201)]

    static void Init()
    {
        var window = (GameDataWindow)GetWindow(typeof(GameDataWindow), true, "Game Data");
    }

    public void OnGUI()
    {
        string line;

        line = "spawned : " + GameData.spawned.ToString(); EditorGUILayout.LabelField(line);
        line = "bodyCount : " + GameData.bodyCount.ToString(); EditorGUILayout.LabelField(line);
        line = "completed : " + GameData.completed.ToString(); EditorGUILayout.LabelField(line);
        line = "progress : " + GameData.progress.ToString(); EditorGUILayout.LabelField(line);
        line = "progressScale : " + GameData.progressScale.ToString(); EditorGUILayout.LabelField(line);
    }

    public void Update()
    {
        Repaint();
    }
}