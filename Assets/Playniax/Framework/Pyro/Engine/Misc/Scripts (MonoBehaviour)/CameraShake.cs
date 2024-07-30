using System.Collections;
using UnityEngine;

namespace Playniax.Pyro
{
    // https://youtu.be/BQGTdRhGmE4

    public class CameraShake : MonoBehaviour
    {
        public float duration = 1;
        public int interval = 10;
        public Vector3 range = new Vector3(1, 1, 1);
        public AnimationCurve curve;
        public void Shake()
        {
            if (_shaking != null) return;

            _interval += 1;

            if (_interval >= interval)
            {
                _interval = 0;

                _shaking = StartCoroutine(_Shake());
            }
        }

        IEnumerator _Shake()
        {
            var position = Camera.main.transform.position;

            float timer = 0f;

            while (timer < duration)
            {
                var shake = Vector3.Scale(Random.insideUnitSphere, range);

                if (curve.length > 0) shake *= curve.Evaluate(timer / duration);

                Camera.main.transform.position = position + shake;

                timer += Time.deltaTime;

                yield return null;
            }

            Camera.main.transform.position = position;

            _shaking = null;
        }

        int _interval;
        Coroutine _shaking;
    }
}