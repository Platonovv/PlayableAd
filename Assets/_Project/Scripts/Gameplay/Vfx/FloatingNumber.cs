using System.Collections;
using Project.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Gameplay.Vfx
{
    /// <summary>
    /// Всплывающее число фидбэка: <see cref="Play"/> поднимается на месте, <see cref="PlayFlying"/> летит дугой к цели.
    /// </summary>
    public sealed class FloatingNumber : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private float _arcHeight = 1.6f;

        private Camera _camera;

        private void OnEnable()
        {
            if (_camera == null)
                _camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_camera != null)
                transform.forward = _camera.transform.forward;
        }

        public void Play(string value,
                         Vector3 worldPosition,
                         float rise,
                         float duration,
                         Pool<FloatingNumber> pool)
        {
            transform.position = worldPosition;
            _text.text = value;
            _group.alpha = 1f;
            StartCoroutine(PlayRoutine(worldPosition, rise, duration, pool));
        }

        private IEnumerator PlayRoutine(Vector3 worldPosition, float rise, float duration, Pool<FloatingNumber> pool)
        {
            var target = worldPosition + Vector3.up * rise;
            var moveCo = StartCoroutine(Tween.Move(transform, target, duration, Ease.OutQuad));
            var fadeCo = StartCoroutine(Tween.Fade(_group, 0f, duration));
            yield return moveCo;
            yield return fadeCo;
            pool.Release(this);
        }

        public void PlayFlying(string value,
                               Vector3 from,
                               Transform target,
                               float duration,
                               Pool<FloatingNumber> pool)
        {
            transform.position = from;
            _text.text = value;
            _group.alpha = 1f;
            StartCoroutine(FlyingRoutine(from, target, duration, pool));
        }

        private IEnumerator FlyingRoutine(Vector3 from, Transform target, float duration, Pool<FloatingNumber> pool)
        {
            var elapsed = 0f;
            duration = Mathf.Max(0.0001f, duration);
            var baseScale = transform.localScale;
            while (elapsed < duration)
            {
                if (this == null || target == null)
                    break;
                elapsed += Time.deltaTime;
                var k = Mathf.Clamp01(elapsed / duration);
                var pos = Vector3.LerpUnclamped(from, target.position, Ease.InQuad(k));
                pos.y += Mathf.Sin(k * Mathf.PI) * _arcHeight;
                transform.position = pos;
                var pop = k < 0.15f ? Mathf.SmoothStep(1.4f, 1f, k / 0.15f) : 1f;
                transform.localScale = baseScale * pop;
                yield return null;
            }
            if (this != null) transform.localScale = baseScale;
            pool.Release(this);
        }
    }
}
