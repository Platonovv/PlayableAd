using System.Collections;
using UnityEngine;

namespace Project.Gameplay.CameraFx
{
    /// <summary>
    /// Камера-эффекты: шейк со случайным смещением, FOV-пульс (для импактов) и
    /// короткий push-offset к точке (для кинематических наездов).
    /// </summary>
    public sealed class ScreenShake : MonoBehaviour
    {
        private Vector3 _origin;
        private float _baseFov;
        private Camera _camera;
        private Coroutine _shakeCo;
        private Coroutine _fovCo;
        private Coroutine _pushCo;

        private void Awake()
        {
            _origin = transform.localPosition;
            _camera = GetComponent<Camera>();
            if (_camera != null) _baseFov = _camera.fieldOfView;
        }

        public void Shake(float amplitude, float duration)
        {
            if (_shakeCo != null) StopCoroutine(_shakeCo);
            _shakeCo = StartCoroutine(RunShake(amplitude, duration));
        }

        public void FovPulse(float deltaFov, float duration)
        {
            if (_camera == null) return;
            if (_fovCo != null) StopCoroutine(_fovCo);
            _fovCo = StartCoroutine(RunFov(deltaFov, duration));
        }

        public void Push(Vector3 worldOffset, float duration)
        {
            if (_pushCo != null) StopCoroutine(_pushCo);
            _pushCo = StartCoroutine(RunPush(worldOffset, duration));
        }

        private IEnumerator RunShake(float amplitude, float duration)
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
            _shakeCo = null;
        }

        private IEnumerator RunFov(float deltaFov, float duration)
        {
            duration = Mathf.Max(0.0001f, duration);
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var k = Mathf.Clamp01(elapsed / duration);
                var bump = Mathf.Sin(k * Mathf.PI);
                _camera.fieldOfView = _baseFov + deltaFov * bump;
                yield return null;
            }
            _camera.fieldOfView = _baseFov;
            _fovCo = null;
        }

        private IEnumerator RunPush(Vector3 worldOffset, float duration)
        {
            duration = Mathf.Max(0.0001f, duration);
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var k = Mathf.Clamp01(elapsed / duration);
                var bump = Mathf.Sin(k * Mathf.PI);
                transform.localPosition = _origin + worldOffset * bump;
                yield return null;
            }
            transform.localPosition = _origin;
            _pushCo = null;
        }

        private void OnDisable()
        {
            if (_shakeCo != null) { StopCoroutine(_shakeCo); _shakeCo = null; }
            if (_fovCo != null) { StopCoroutine(_fovCo); _fovCo = null; }
            if (_pushCo != null) { StopCoroutine(_pushCo); _pushCo = null; }
            transform.localPosition = _origin;
            if (_camera != null) _camera.fieldOfView = _baseFov;
        }
    }
}
