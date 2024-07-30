using UnityEditor;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Playniax.Framework.Editor;

namespace Playniax.Ignition.UI.Editor
{
    public class Helpers : EditorWindowHelpers
    {
        [MenuItem("Tools/Playniax/Ignition Framework/UI/ScrollBox", false, 51)]
        public static void Add_ScrollBox()
        {
            string file = SceneManager.GetActiveScene().path;
            string path = Path.GetDirectoryName(file);

            //Debug.Log(path);

            if (File.Exists(path + "/script.txt") == false) FileUtil.CopyFileOrDirectory("Assets/Playniax/Framework/Ignition/Framework/UI/Data/script.txt", path + "/script.txt");
            if (File.Exists(path + "/how to play.txt") == false) FileUtil.CopyFileOrDirectory("Assets/Playniax/Framework/Ignition/Framework/UI/Data/how to play.txt", path + "/how to play.txt");
            if (File.Exists(path + "/prologue.txt") == false) FileUtil.CopyFileOrDirectory("Assets/Playniax/Framework/Ignition/Framework/UI/Data/prologue.txt", path + "/prologue.txt");

            AssetDatabase.Refresh();

            Canvas canvas = GetCanvas();

            if (canvas == null) canvas = AddCanvas();

            RectTransform rectTransform = new GameObject("ScrollBox", typeof(RectTransform)).GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(640, 320);
            rectTransform.transform.SetParent(canvas.transform);
            rectTransform.transform.localPosition = Vector3.zero;

            Image image = rectTransform.gameObject.AddComponent<Image>();
            image.color = new Color(.25f, .25f, .25f, .25f);
            image.maskable = true;

            rectTransform.gameObject.AddComponent<Mask>();

            TextAsset script = AssetDatabase.LoadAssetAtPath(path + "/script.txt", typeof(TextAsset)) as TextAsset;

            ScrollBox scrollBox = rectTransform.gameObject.AddComponent<ScrollBox>();
            scrollBox.externalScript = script;
            scrollBox.useExternalScript = true;

            scrollBox.assetBank = new AssetBank();

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            TextAsset howtoplay = AssetDatabase.LoadAssetAtPath(path + "/how to play.txt", typeof(TextAsset)) as TextAsset;
            TextAsset prologue = AssetDatabase.LoadAssetAtPath(path + "/prologue.txt", typeof(TextAsset)) as TextAsset;
            Sprite playniax = AssetDatabase.LoadAssetAtPath("Assets/Playniax/Framework/Dependencies/Textures/Playniax/playniax.png", typeof(Sprite)) as Sprite;

            scrollBox.assetBank.assets = new Object[4] { font, howtoplay, prologue, playniax };

            scrollBox.gameObject.AddComponent<ScrollBoxAutoScroll>();

            Undo.RegisterCreatedObjectUndo(scrollBox.gameObject, "Create object");

            Selection.activeGameObject = scrollBox.gameObject;
        }
    }
}
