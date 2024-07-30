using UnityEditor;
using UnityEngine;

namespace Playniax.Ignition.Menu
{
    public class Helpers : EditorWindow
    {
        [MenuItem("Tools/Playniax/EasyGameUI/Music Player", false, 71)]
        public static void Add_Music_Player()
        {
            var musicPlayer = new GameObject("Music Player").AddComponent<MusicPlayer>();

            Undo.RegisterCreatedObjectUndo(musicPlayer.gameObject, "Create object");

            Selection.activeGameObject = musicPlayer.gameObject;
        }
    }
}