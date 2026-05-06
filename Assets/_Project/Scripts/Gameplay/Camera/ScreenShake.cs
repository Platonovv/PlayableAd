using System.Collections;
using UnityEngine;

namespace Project.Gameplay.CameraFx
{
    /// <summary>
    /// Шейк камеры со случайным смещением и затухающей амплитудой.
    /// </summary>
    public sealed class ScreenShake : MonoBehaviour
    {
        private Vector3 _origin;
        private Coroutine _co;

        private void Awake() => _origin = transform.localPosition;

        public void Shake(float amplitude, float duration)
        {
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Run(amplitude, duration));
        }

        private IEnumerator Run(float amplitude, float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var fade = 1f - Mathf.Clamp01(elapsed / duration);
                var offset = (Vector3)Random.insideUnitCircle * amplitude * fade;
                transform.localPosition = _origin + offset;
                yield return null;
            }
            transform.localPosition = _origin;
            _co = null;
        }

        private void OnDisable()
        {
            if (_co != null) { StopCoroutine(_co); _co = null; }
            transform.localPosition = _origin;
        }
    }
}
