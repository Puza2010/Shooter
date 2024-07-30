/*
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Playniax.Ignition;

public class EasyGameUIWindow : EditorWindow
{
    string template = "Home";

    [MenuItem("Tools/Playniax/Easy Game UI")]
    static void Init()
    {
        EasyGameUIWindow window = (EasyGameUIWindow)EditorWindow.GetWindow(typeof(EasyGameUIWindow), true, "EasyGameUI");
    }

    void OnGUI()
    {
        if (Application.isPlaying) return;

        if (_simpleGameUI == null)
        {
            _simpleGameUI = FindObjectOfType<EasyGameUI>();
        }

        if (_simpleGameUI != null)
        {
            EditorGUILayout.LabelField("Clone Actions");

            template = GUILayout.TextField(template, 32);

            if (GUILayout.Button("Clone '" + template + "' Screen Canvas Scaler"))
            {
                _CloneMainCanvasScaler();
            }

            if (GUILayout.Button("Clone '" + template + "' Screen Window Image"))
            {
                _CloneWindow();
            }
        }

        void _CloneMainCanvasScaler()
        {
            var mainCanvas = _GetCanvasScaler(template);
            if (mainCanvas == null) return;

            var canvasScalers = _simpleGameUI.GetComponentsInChildren(typeof(CanvasScaler), true);

            for (int i = 0; i < canvasScalers.Length; i++)
            {
                var canvasScaler = canvasScalers[i] as CanvasScaler;

                if (canvasScaler == null) continue;
                if (canvasScaler == mainCanvas) continue;

                Debug.Log(canvasScaler.name);

                canvasScaler.referenceResolution = mainCanvas.referenceResolution;
                canvasScaler.screenMatchMode = mainCanvas.screenMatchMode;
                canvasScaler.matchWidthOrHeight = mainCanvas.matchWidthOrHeight;
                canvasScaler.screenMatchMode = mainCanvas.screenMatchMode;
                canvasScaler.referencePixelsPerUnit = mainCanvas.referencePixelsPerUnit;
            }
        }

        void _CloneWindow()
        {
            var mainImage = _GetWindow(template);
            if (mainImage == null) return;

            for (int i = 1; i < _windows.Length; i++)
            {
                var screen = _simpleGameUI.transform.Find(_windows[i]);
                if (screen == null) continue;
                var window = screen.transform.Find("Window");
                if (window == null) continue;
                var image = window.GetComponent<Image>();
                if (image == null) continue;

                image.sprite = mainImage.sprite;
                image.color = mainImage.color;
                image.rectTransform.position = mainImage.rectTransform.position;
                image.rectTransform.anchorMin = mainImage.rectTransform.anchorMin;
                image.rectTransform.anchorMax = mainImage.rectTransform.anchorMax;
                image.rectTransform.anchoredPosition = mainImage.rectTransform.anchoredPosition;
                image.rectTransform.localScale = mainImage.rectTransform.localScale;
                image.rectTransform.rotation = mainImage.rectTransform.rotation;
                image.rectTransform.sizeDelta = mainImage.rectTransform.sizeDelta;
            }
        }

        CanvasScaler _GetCanvasScaler(string name)
        {
            var screen = _simpleGameUI.transform.Find(name);
            if (screen == null) return null;
            var canvasScaler = screen.GetComponent<CanvasScaler>();
            return canvasScaler;
        }

        Image _GetWindow(string name)
        {
            var screen = _simpleGameUI.transform.Find(name);
            if (screen == null) return null;
            var window = screen.transform.Find("Window");
            if (window == null) return null;
            var image = window.GetComponent<Image>();
            return image;
        }
    }

    string[] _windows = new string[] { "Home", "About", "GameOver", "Pause", "Settings", "Shop" };

    GameObject _gameObject;
    EasyGameUI _simpleGameUI;
}
*/