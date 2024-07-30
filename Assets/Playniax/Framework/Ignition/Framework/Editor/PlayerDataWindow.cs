using UnityEditor;
using Playniax.Ignition;

public class PlayerDataWindow : EditorWindow
{

    [MenuItem("Tools/Playniax/Monitor/Ignition/Player Data", false, 201)]

    static void Init()
    {
        var window = (PlayerDataWindow)GetWindow(typeof(PlayerDataWindow), true, "Player Data");
    }

    public void OnGUI()
    {
        if (PlayerData.data == null) return;
        if (PlayerData.data.Length == 0) return;

        string line;

        for (int i = 0; i < PlayerData.data.Length; i++)
        {
            var player = PlayerData.data[i];

            line = "PLAYER : " + i; EditorGUILayout.LabelField(line);
            line = "    name : " + player.name.ToString(); EditorGUILayout.LabelField(line);
            line = "    coins : " + player.coins.ToString(); EditorGUILayout.LabelField(line);
            line = "    collectables : " + player.collectables.ToString(); EditorGUILayout.LabelField(line);
            line = "    lives : " + player.lives.ToString(); EditorGUILayout.LabelField(line);
            line = "    scoreBoard : " + player.scoreboard.ToString(); EditorGUILayout.LabelField(line);
            line = ""; EditorGUILayout.LabelField(line);
        }
    }

    public void Update()
    {
        Repaint();
    }
}