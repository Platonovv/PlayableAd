using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project.Gameplay.CameraFx
{
    /// <summary>
    /// Шейк камеры со случайным смещением и затухающей амплитудой.
    /// </summary>
    public sealed class ScreenShake : MonoBehaviour
    {
        private Vector3 _origin;
        private CancellationTokenSource _cts;

        private void Awake() => _origin = transform.localPosition;

        public void Shake(float amplitude, float duration)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _ = Run(amplitude, duration, _cts.Token);
        }

        private async UniTask Run(float amplitude, float duration, CancellationToken ct)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var fade = 1f - Mathf.Clamp01(elapsed / duration);
                var offset = (Vector3)Random.insideUnitCircle * amplitude * fade;
                transform.localPosition = _origin + offset;
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);
            }
            transform.localPosition = _origin;
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            transform.localPosition = _origin;
        }
    }
}
