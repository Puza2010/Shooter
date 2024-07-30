using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition
{
    public class Rotator : MonoBehaviour
    {
        public Image targetGraphic;

        public int state;

        public Sprite[] states;

        public virtual void OnClick(int state)
        {
            print(state);
        }

        public void OnPointerClick()
        {
            state += 1;
            if (state >= states.Length) state = 0;

            UpdateState();

            OnClick(state);
        }

        public void UpdateState()
        {
            if (targetGraphic && states[state])
            {
                targetGraphic.sprite = states[state];
                targetGraphic.SetNativeSize();
            }
        }

        void Awake()
        {
            if (targetGraphic == null) targetGraphic = GetComponent<Image>();

            UpdateState();
        }

    }
}