using UnityEngine;
using UnityEngine.UI;

namespace Playniax.Ignition
{
    public class CustomButton : Button
    {
        public Graphic[] Graphics
        {
            get
            {
                if (_graphics == null) _graphics = targetGraphic.transform.GetComponentsInChildren<Graphic>();

                return _graphics;
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy) return;

            Color tintColor;
            Sprite transitionSprite;
            string triggerName;

            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = colors.normalColor;
                    transitionSprite = null;
                    triggerName = animationTriggers.normalTrigger;
                    break;
                case SelectionState.Highlighted:
                    tintColor = colors.highlightedColor;
                    transitionSprite = spriteState.highlightedSprite;
                    triggerName = animationTriggers.highlightedTrigger;
                    break;
                case SelectionState.Pressed:
                    tintColor = colors.pressedColor;
                    transitionSprite = spriteState.pressedSprite;
                    triggerName = animationTriggers.pressedTrigger;
                    break;
                case SelectionState.Selected:
                    tintColor = colors.selectedColor;
                    transitionSprite = spriteState.selectedSprite;
                    triggerName = animationTriggers.selectedTrigger;
                    break;
                case SelectionState.Disabled:
                    tintColor = colors.disabledColor;
                    transitionSprite = spriteState.disabledSprite;
                    triggerName = animationTriggers.disabledTrigger;
                    break;
                default:
                    tintColor = Color.black;
                    transitionSprite = null;
                    triggerName = string.Empty;
                    break;
            }

            switch (transition)
            {
                case Transition.ColorTint:
                    _StartColorTween(tintColor * colors.colorMultiplier, instant);
                    break;
                case Transition.SpriteSwap:
                    _DoSpriteSwap(transitionSprite);
                    break;
                case Transition.Animation:
                    _TriggerAnimation(triggerName);
                    break;
            }

        }

        void _DoSpriteSwap(Sprite newSprite)
        {
            if (image == null) return;

            image.overrideSprite = newSprite;
        }

        void _StartColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null) return;

            foreach (Graphic g in Graphics)
            {
                g.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
            }
        }

        void _TriggerAnimation(string triggername)
        {
            if (transition != Transition.Animation || animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername)) return;

            animator.ResetTrigger(animationTriggers.normalTrigger);
            animator.ResetTrigger(animationTriggers.highlightedTrigger);
            animator.ResetTrigger(animationTriggers.pressedTrigger);
            animator.ResetTrigger(animationTriggers.selectedTrigger);
            animator.ResetTrigger(animationTriggers.disabledTrigger);

            animator.SetTrigger(triggername);
        }

        Graphic[] _graphics;
    }
}