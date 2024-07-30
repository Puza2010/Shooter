using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

using Playniax.Framework.Editor;
using Playniax.Ignition;
using System;

public class ProductNavigator : EditorWindowHelpers
{
    const string MAIN_ASSET_PATH = "Assets/Playniax/Framework/Ignition";
    const string MAIN_ASSET_TITLE = "Ignition";
    const string MENU = "Tools/Playniax/Product Navigator";
    const string ONLINE_DOCUMENTATION_URL = "https://docs.playniax.com/Home/Introduction/index.html";
    const string PLAYNIAX_LOGO_PATH = "Assets/Playniax/Framework/Dependencies/Textures/Playniax/Logos/playniax.png";
    class Content : GUIContent
    {
        public string assetPath;

        public Content(string assetPath, string text, string tooltip)
        {
            this.assetPath = assetPath;
            this.text = text;
            this.tooltip = tooltip;
        }
    }

    public class Record
    {
        public string assetPath = "";
        public string package = "";
        public string title = "";

        public GUIContent content;

        public static Record[] Add(Record[] records, string[] folders, string package)
        {
            folders = FilterFolders(folders);

            if (folders.Length == 0) return records;

            var guids = AssetDatabase.FindAssets("t:Scene", folders);

            for (int i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);

                var title = Path.GetFileNameWithoutExtension(assetPath);

                var record = new Record();

                record.assetPath = assetPath;
                record.package = package;
                record.title = title;

                records = ArrayHelpers.Add(records, record);
            }

            return records;
        }

        public static string[] FilterFolders(string[] folders)
        {
            var filterFolders = new string[0];

            for (int i = 0; i < folders.Length; i++)
                if (AssetDatabase.IsValidFolder(folders[i])) filterFolders = ArrayHelpers.Add(filterFolders, folders[i]);

            return filterFolders;
        }
    }

    [InitializeOnLoadMethod]
    static void Init()
    {
        // For testing:
        //EditorPrefs.DeleteKey("Playniax.Navigator.showed");
        //EditorPrefs.DeleteKey("Playniax.Navigator.dontShowThisAgain");

        if (EditorPrefs.GetBool("Playniax.Navigator.dontShowThisAgain") == false && EditorApplication.isPlaying == false) EditorApplication.hierarchyChanged += AutoStart;
    }

    static void AutoStart()
    {
        EditorApplication.hierarchyChanged -= AutoStart;
        EditorApplication.hierarchyChanged -= AutoStart;

        OpenWindow();
    }

    [MenuItem(MENU, false, 500)]
    static void FromMenu()
    {
        OpenWindow();
   }

    static void OpenWindow()
    {
        var window = GetWindow(typeof(ProductNavigator), false, "Product Navigator");
        window.minSize = new Vector2(352, 256);

        if (EditorPrefs.GetBool("Playniax.Navigator.showed") == false)
        {
            var position = window.position;
            position.center = EditorGUIUtility.GetMainWindowPosition().center;
            window.position = position;

            EditorPrefs.SetBool("Playniax.Navigator.showed", true);
        }
    }

    void OnDestroy()
    {
        EditorPrefs.SetBool("Playniax.Navigator.dontShowThisAgain", _dontShowThisAgain);
    }

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            _OnGUIPlaying();
        }
        else
        {
            _Config();

            _OnGUIEditor();
        }
    }

    void _Config()
    {
        if (_configPath == "") _configPath = _GetConfigPath();

        if (_config == "" && File.Exists(_configPath))
        {
            _config = AssetDatabase.LoadAssetAtPath<TextAsset>(_configPath).text;

            _mainAssetPath = _GetString(_config, "mainAssetPath", MAIN_ASSET_PATH);
            _mainAssetTitle = _GetString(_config, "mainAssetTitle", MAIN_ASSET_TITLE);
        }
    }

    string _GetString(string data, string key, string defaultValue = "")
    {
        var lines = data.Split('\n');

        foreach (string line in lines)
        {
            // Check if the line contains the parameter name
            if (line.StartsWith(key))
            {
                // Extract the value using '=' and trim the result
                var startIndex = line.IndexOf('=') + 1;
                var value = line.Substring(startIndex).Trim();

                // Remove the quotes around the value
                value = value.Trim('\"');

                return value;
            }
        }

        return defaultValue;
    }

    string _GetFolderPath()
    {
        if (_folderPath != null && _folderPath != "") return _folderPath;

        var guids = AssetDatabase.FindAssets("ProductNavigator t:MonoScript", null);
        if (guids.Length == 1)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _folderPath = Path.GetDirectoryName(path);
            return _folderPath;
        }

        return "";
    }

    void _DontShowThisAgainUI()
    {
        GUILayout.Space(8);

        _dontShowThisAgain = GUILayout.Toggle(_dontShowThisAgain, "Don't show this again");

        GUILayout.Space(8);

        if (_dontShowThisAgain)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Use the Unity menu to open this window again.");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("(" + Convert.ToChar(34) + MENU + Convert.ToChar(34) + ")");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(8);
    }

    string _GetConfigPath()
    {
        // Search for all text assets in the project
        string[] guids = AssetDatabase.FindAssets("t:TextAsset");

        foreach (string guid in guids)
        {
            // Get the asset path
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Check if the file name matches "Product Navigator.txt"
            if (path.EndsWith("Product Navigator.txt")) return path;
        }

        // If no file is found, return null
        return null;
    }

    string _GetDocumentationPath()
    {
        // Search for all folders named "Documentation"
        string[] documentationFolders = AssetDatabase.FindAssets("Documentation", new[] { "Assets" });

        foreach (string guid in documentationFolders)
        {
            // Get the folder path
            string folderPath = AssetDatabase.GUIDToAssetPath(guid);

            // Check if the folder contains an index.html file
            string indexPath = Path.Combine(folderPath, "index.html");
            if (File.Exists(indexPath)) return indexPath;
        }

        // If no documentation folder with index.html is found, return null
        return null;
    }

    Record[] _GetMainRecords(Record[] records)
    {
        if (records == null)
        {
            records = Record.Add(records, new[] { _mainAssetPath }, _mainAssetTitle);
        }

        return records;
    }

    GUIContent[] _GetMainContent(Record[] records)
    {
        var content = new GUIContent[0];

        for (int i = 0; i < records.Length; i++)
        {
            var tooltip = "File path is " + records[i].assetPath;

            records[i].content = new Content(records[i].assetPath, records[i].title, tooltip);

            content = ArrayHelpers.Add(content, records[i].content);
        }

        return content;
    }

    GUIContent[] _GetPlayniaxContent(Record[] records)
    {
        var content = new GUIContent[0];

        for (int i = 0; i < records.Length; i++)
        {
            var tooltip = "File path is " + records[i].assetPath;

            records[i].content = new Content(records[i].assetPath, records[i].title, tooltip);

            if (records[i].package == _package[_packageState] || _package[_packageState] == "Show All")
            {
                content = ArrayHelpers.Add(content, records[i].content);
            }
        }

        return content;
    }

    Record[] _GetPlayniaxRecords(Record[] records)
    {
        if (records == null)
        {
            records = Record.Add(records, new[] { "Assets/Playniax/Framework/Ignition/Scenes" }, "Ignition/Framework");
            records = Record.Add(records, new[] { "Assets/Playniax/Framework/Ignition/Value Pack" }, "Ignition/Value Pack");

            records = Record.Add(records, new[] { "Assets/Playniax/Framework/EasyGameUI" }, "EasyGameUI/Framework");
            records = Record.Add(records, new[] { "Assets/Playniax/EasyGameUI (Themes)" }, "EasyGameUI/Themes");

            records = Record.Add(records, new[] { "Assets/Playniax/Framework/Pyro/Engine" }, "Pyro/Engine");
            records = Record.Add(records, new[] { "Assets/Playniax/Framework/Pyro/Scenes" }, "Pyro/Framework");
            records = Record.Add(records, new[] { "Assets/Playniax/Framework/Pyro/Prototyping" }, "Pyro/Prototyping");

            records = Record.Add(records, new[] { "Assets/Playniax/Space Shooter Art Pack 01/Scenes" }, "Space Shooter Art Pack 01");

            records = Record.Add(records, new[] { "Assets/Playniax/Space Shooter Art Pack 02/Scenes" }, "Space Shooter Art Pack 02");

            records = Record.Add(records, new[] { "Assets/Playniax/Galaga Essentials/Scenes" }, "Galaga Essentials");

            records = Record.Add(records, new[] { "Assets/Playniax/Framework/Extensions/Infinite Scroller" }, "Infinite Scroller");

            records = Record.Add(records, new[] { "Assets/Playniax/Space Shooter Kit" }, "Space Shooter Kit");
        }

        return records;
    }

    void _PackageUI(Record[] records)
    {
        if (_package == null || _package != null && _package.Length == 0)
        {
            _package = ArrayHelpers.Add(_package, "Show All");

            for (int i = 0; i < records.Length; i++)
            {
                var package = records[i].package;

                if (Array.IndexOf(_package, package) == -1) _package = ArrayHelpers.Add(_package, package);
            }
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Selection", GUILayout.ExpandWidth(false));
        _packageState = EditorGUILayout.Popup(_packageState, _package, new[] { GUILayout.ExpandWidth(false) });
        GUILayout.EndHorizontal();
    }

    void _MainUI()
    {
        _mainRecords = _GetMainRecords(_mainRecords);

        GUILayout.Space(2);

        _mainScrollPosition = GUILayout.BeginScrollView(_mainScrollPosition, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);

        var content = _GetMainContent(_mainRecords);

        var buttons = Buttons(content, 320, 32) as Content;

        if (buttons != null)
        {
            if (Event.current.button == 0) EditorSceneManager.OpenScene(buttons.assetPath);

            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(buttons.assetPath);
        }

        GUILayout.EndScrollView();
    }

    void _OnGUIEditor()
    {
        _mainState = GUILayout.Toolbar(_mainState, new GUIContent[] {
                new GUIContent(_mainAssetTitle, null, "Main product."),
                new GUIContent("Playniax Showreel", null, "Playniax showreel shortcuts."),
                }, GUILayout.Height(28));

        if (_mainState == 0)
        {
            _MainUI();
        }
        else if (_mainState == 1)
        {
            _ShowreelUI();
        }

        _PlayniaxLinksUI();

        _DontShowThisAgainUI();
    }

    void _OnGUIPlaying()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Scene navigation only works in Editor Mode");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        _PlayniaxLinksUI();

        _DontShowThisAgainUI();
    }

    void _PlayniaxLinksUI()
    {
        var guiStyle = new GUIStyle(GUI.skin.button);
        guiStyle.imagePosition = ImagePosition.ImageLeft;
        guiStyle.padding = new RectOffset(0, 0, 8, 8);
        GUI.skin.button = guiStyle;

        GUILayout.Space(2);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        var folder = _GetFolderPath();

        var moreContent = new GUIContent(" More from Playniax", AssetDatabase.LoadAssetAtPath<Texture2D>(PLAYNIAX_LOGO_PATH), "Visit the Playniax page on the Unity Asset Store.");
        var more = GUILayout.Button(moreContent, GUILayout.Width(160), GUILayout.Height(40));
        if (more) Application.OpenURL("https://assetstore.unity.com/publishers/30574");

        var docsContent = new GUIContent(" Doumentation", AssetDatabase.LoadAssetAtPath<Texture2D>(folder + "/Icons/documentation.png"), "Need help? Click here to see the documentation.");
        var docs = GUILayout.Button(docsContent, GUILayout.Width(160), GUILayout.Height(40));
        if (docs)
        {
            if (_documentationPath == "") _documentationPath = _GetDocumentationPath();

            if (File.Exists(_documentationPath) == true)
            {
                Application.OpenURL(_documentationPath);
            }
            else
            {
                Application.OpenURL(ONLINE_DOCUMENTATION_URL);
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void _ShowreelUI()
    {
        _playniaxRecords = _GetPlayniaxRecords(_playniaxRecords);

        _PackageUI(_playniaxRecords);

        GUILayout.Space(2);

        _showreelscrollPosition = GUILayout.BeginScrollView(_showreelscrollPosition, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);

        var content = _GetPlayniaxContent(_playniaxRecords);

        var buttons = Buttons(content, 320, 32) as Content;

        if (buttons != null)
        {
            if (Event.current.button == 0) EditorSceneManager.OpenScene(buttons.assetPath);

            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(buttons.assetPath);
        }

        GUILayout.EndScrollView();
    }

    string _config = "";
    string _configPath = "";
    string _documentationPath = "";
    bool _dontShowThisAgain;
    string _folderPath = "";
    Vector2 _mainScrollPosition;
    int _mainState;
    string[] _package;
    int _packageState;
    Record[] _playniaxRecords;
    string _mainAssetTitle = MAIN_ASSET_TITLE;
    string _mainAssetPath = MAIN_ASSET_PATH;
    Record[] _mainRecords;
    Vector2 _showreelscrollPosition;
}
