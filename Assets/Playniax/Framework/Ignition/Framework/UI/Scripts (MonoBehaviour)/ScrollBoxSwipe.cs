﻿using UnityEngine;

namespace Playniax.Ignition.UI
{
    // Provides functionality for smooth scrolling of content within the ScrollBox based on swipe gestures.
    public class ScrollBoxSwipe : MonoBehaviour
    {
        [Tooltip("Reference to the ScrollBox component containing the scrollable content.")]
        public ScrollBox scrollBox;

        [Tooltip("Enable horizontal scrolling.")]
        public bool horizontal;

        [Tooltip("Enable vertical scrolling.")]
        public bool vertical = true;

        public void Stop()
        {
            _previous = default;
            _speed = default;
        }

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            if (scrollBox == null) scrollBox = GetComponent<ScrollBox>();
        }

        /*
        void OnEnable()
        {
            _previous = default;
            _speed = default;
        }
        */

        void Update()
        {
            if (_previous == default) _previous = Input.mousePosition;

            if (Input.GetMouseButton(0) && scrollBox.isMouseOver)
            {
                Vector3 mousePosition = Input.mousePosition;

                if (_previous == Vector3.zero) _previous = mousePosition;

                _speed -= (_previous - mousePosition) * 10 * Time.unscaledDeltaTime;

                _previous = mousePosition;
            }
            else
            {
                _previous = Vector3.zero;
            }

            scrollBox.content.transform.position += new Vector3(_speed.x *= horizontal ? 1 : 0, _speed.y *= vertical ? 1 : 0);

            if (scrollBox.content.transform.localPosition.y < 0)
            {
                scrollBox.content.transform.localPosition = new Vector3(scrollBox.content.transform.localPosition.x, 0, scrollBox.content.transform.localPosition.z);
                _speed = -_speed * .05f;
            }

            if (scrollBox.content.transform.localPosition.y > scrollBox.contentHeight - _rectTransform.sizeDelta.y)
            {
                scrollBox.content.transform.localPosition = new Vector3(scrollBox.content.transform.localPosition.x, scrollBox.contentHeight - _rectTransform.sizeDelta.y, scrollBox.content.transform.localPosition.z);
                _speed = -_speed * .05f;
            }

            _speed *= 1 / (1 + (Time.unscaledDeltaTime * .99f));
        }

        RectTransform _rectTransform;

        Vector3 _previous;
        Vector3 _speed;
    }
}