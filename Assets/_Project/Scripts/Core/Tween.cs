using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project.Core
{
    /// <summary>
    /// Tween-хелперы поверх UniTask. Устойчивы к уничтожению цели во время анимации.
    /// </summary>
    public static class Tween
    {
        public static UniTask Move(Transform t, Vector3 to, float duration, Func<float, float> ease = null, CancellationToken ct = default)
            => Run(t, duration, ease, t.position, to, v => t.position = v, ct);

        public static UniTask LocalMove(Transform t, Vector3 to, float duration, Func<float, float> ease = null, CancellationToken ct = default)
            => Run(t, duration, ease, t.localPosition, to, v => t.localPosition = v, ct);

        public static UniTask Scale(Transform t, Vector3 to, float duration, Func<float, float> ease = null, CancellationToken ct = default)
            => Run(t, duration, ease, t.localScale, to, v => t.localScale = v, ct);

        public static UniTask Punch(Transform t, float amplitude, float duration, CancellationToken ct = default)
            => RunPunch(t, amplitude, duration, ct);

        public static UniTask Fade(CanvasGroup g, float to, float duration, CancellationToken ct = default)
            => Run(g, duration, Ease.Linear, g.alpha, to, v => g.alpha = v, ct);

        private static async UniTask Run<TVal>(UnityEngine.Object owner, float duration, Func<float, float> ease, TVal from, TVal to, Action<TVal> apply, CancellationToken ct)
            where TVal : struct
        {
            ease ??= Ease.Linear;
            var elapsed = 0f;
            duration = Mathf.Max(0.0001f, duration);
            while (elapsed < duration)
            {
                if (owner == null) return;            // объект уничтожен — выходим тихо
                if (ct.IsCancellationRequested) return;
                elapsed += Time.deltaTime;
                var k = ease(Mathf.Clamp01(elapsed / duration));
                apply(Lerp(from, to, k));
                await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
            }
            if (owner == null) return;
            apply(to);
        }

        private static async UniTask RunPunch(Transform t, float amplitude, float duration, CancellationToken ct)
        {
            if (t == null) return;
            var origin = t.localScale;
            var elapsed = 0f;
            duration = Mathf.Max(0.0001f, duration);
            while (elapsed < duration)
            {
                if (t == null) return;
                if (ct.IsCancellationRequested) return;
                elapsed += Time.deltaTime;
                var k = elapsed / duration;
                var bump = Mathf.Sin(k * Mathf.PI) * amplitude;
                t.localScale = origin * (1f + bump);
                await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
            }
            if (t == null) return;
            t.localScale = origin;
        }

        private static TVal Lerp<TVal>(TVal a, TVal b, float k) where TVal : struct
        {
            return a switch
            {
                Vector3 va when b is Vector3 vb => (TVal)(object)Vector3.LerpUnclamped(va, vb, k),
                Vector2 va when b is Vector2 vb => (TVal)(object)Vector2.LerpUnclamped(va, vb, k),
                float fa when b is float fb => (TVal)(object)Mathf.LerpUnclamped(fa, fb, k),
                Color ca when b is Color cb => (TVal)(object)Color.LerpUnclamped(ca, cb, k),
                _ => b
            };
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
