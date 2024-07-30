using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition
{
    public class KeyEnter : MonoBehaviour
    {
        public Button button;
        public KeyCode keyCode;

        void Awake()
        {
            if (button == null) button = GetComponent<Button>();
        }

        void Update()
        {
            if (EasyGameUI.instance && EasyGameUI.instance.effects.screenFader.activeInHierarchy == false && Input.GetKeyDown(keyCode)) button.onClick.Invoke();
        }
    }
}