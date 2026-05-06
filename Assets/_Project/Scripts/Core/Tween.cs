using System;
using System.Collections;
using UnityEngine;

namespace Project.Core
{
    /// <summary>
    /// Tween-хелперы на корутинах. Возвращают IEnumerator — caller делает StartCoroutine.
    /// Устойчивы к уничтожению цели во время анимации. Все методы — non-generic,
    /// чтобы Luna транспилировала их без pattern-matching костылей.
    /// </summary>
    public static class Tween
    {
        public static IEnumerator Move(Transform t, Vector3 to, float duration, Func<float, float> ease = null)
        {
            if (t == null) yield break;
            if (ease == null) ease = Ease.Linear;
            duration = Mathf.Max(0.0001f, duration);
            var from = t.position;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                if (t == null) yield break;
                elapsed += Time.deltaTime;
                var k = ease(Mathf.Clamp01(elapsed / duration));
                t.position = Vector3.LerpUnclamped(from, to, k);
                yield return null;
            }
            if (t != null) t.position = to;
        }

        public static IEnumerator LocalMove(Transform t, Vector3 to, float duration, Func<float, float> ease = null)
        {
            if (t == null) yield break;
            if (ease == null) ease = Ease.Linear;
            duration = Mathf.Max(0.0001f, duration);
            var from = t.localPosition;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                if (t == null) yield break;
                elapsed += Time.deltaTime;
                var k = ease(Mathf.Clamp01(elapsed / duration));
                t.localPosition = Vector3.LerpUnclamped(from, to, k);
                yield return null;
            }
            if (t != null) t.localPosition = to;
        }

        public static IEnumerator Scale(Transform t, Vector3 to, float duration, Func<float, float> ease = null)
        {
            if (t == null) yield break;
            if (ease == null) ease = Ease.Linear;
            duration = Mathf.Max(0.0001f, duration);
            var from = t.localScale;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                if (t == null) yield break;
                elapsed += Time.deltaTime;
                var k = ease(Mathf.Clamp01(elapsed / duration));
                t.localScale = Vector3.LerpUnclamped(from, to, k);
                yield return null;
            }
            if (t != null) t.localScale = to;
        }

        public static IEnumerator Fade(CanvasGroup g, float to, float duration)
        {
            if (g == null) yield break;
            duration = Mathf.Max(0.0001f, duration);
            var from = g.alpha;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                if (g == null) yield break;
                elapsed += Time.deltaTime;
                var k = Mathf.Clamp01(elapsed / duration);
                g.alpha = Mathf.LerpUnclamped(from, to, k);
                yield return null;
            }
            if (g != null) g.alpha = to;
        }

        public static IEnumerator Punch(Transform t, float amplitude, float duration)
        {
            if (t == null) yield break;
            var origin = t.localScale;
            duration = Mathf.Max(0.0001f, duration);
            var elapsed = 0f;
            while (elapsed < duration && t != null)
            {
                elapsed += Time.deltaTime;
                var k = elapsed / duration;
                var bump = Mathf.Sin(k * Mathf.PI) * amplitude;
                t.localScale = origin * (1f + bump);
                yield return null;
            }
            if (t != null) t.localScale = origin;
        }
    }

    /// <summary>
    /// Easing-кривые для <see cref="Tween"/>.
    /// </summary>
    public static class Ease
    {
        public static readonly Func<float, float> Linear = k => k;
        public static readonly Func<float, float> OutQuad = k => 1f - (1f - k) * (1f - k);
        public static readonly Func<float, float> InQuad = k => k * k;
        public static readonly Func<float, float> InOutQuad = k => k < 0.5f ? 2f * k * k : 1f - Mathf.Pow(-2f * k + 2f, 2f) * 0.5f;
        public static readonly Func<float, float> OutBack = k => { const float c1 = 1.70158f; const float c3 = c1 + 1f; var x = k - 1f; return 1f + c3 * x * x * x + c1 * x * x; };
        public static readonly Func<float, float> OutSine = k => Mathf.Sin(k * Mathf.PI * 0.5f);
    }
}
