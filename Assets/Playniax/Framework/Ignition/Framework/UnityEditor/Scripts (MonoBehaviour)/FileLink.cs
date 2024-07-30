#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Playniax.Ignition.UnityEditor
{
    public class FileLink : MonoBehaviour, IPointerClickHandler
    {
        public string file = "GameData.cs";

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
            {
                if (assetPath.EndsWith(file))
                {
                    var lScript = (MonoScript)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));

                    if (lScript != null)
                    {
                        AssetDatabase.OpenAsset(lScript);
                        break;
                    }
                }
            }
        }
    }
}

#endif