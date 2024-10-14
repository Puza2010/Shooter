using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerProgression))]
public class PlayerProgressionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerProgression progression = (PlayerProgression)target;

        if (GUILayout.Button("Reset Progress"))
        {
            progression.ResetProgress();
            Debug.Log("Player progression reset to level 1.");
        }
    }
}