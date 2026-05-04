using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Core;
using Project.Domain;
using UnityEngine;

namespace Project.Gameplay.Units
{
    /// <summary>
    /// View сундука: idle, анимация открытия с VFX, опциональный idle-бобинг (выключен по дефолту).
    /// </summary>
    public sealed class ChestView : UnitView
    {
        private static readonly int OpenHash = Animator.StringToHash("Open");

        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _openVfx;
        [SerializeField] private float _idleBobAmplitude = 0f;
        [SerializeField] private float _idleBobSpeed = 2f;

        private Vector3 _basePosition;
        private bool _basePositionCached;

        public override void Bind(Unit unit, Camera camera)
        {
            base.Bind(unit, camera);
            _basePosition = transform.localPosition;
            _basePositionCached = true;
        }

        public async UniTask PlayOpen(CancellationToken ct)
        {
            if (_animator != null) _animator.CrossFadeInFixedTime(OpenHash, 0.05f);
            if (_openVfx != null) _openVfx.Play();
            await Tween.Punch(transform, 0.2f, 0.45f, ct);
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.2f), cancellationToken: ct);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_basePositionCached) return;
            if (Unit == null || !Unit.IsAlive) return;
            if (_idleBobAmplitude <= 0f) return;
            var bob = Mathf.Sin(Time.time * _idleBobSpeed) * _idleBobAmplitude;
            transform.localPosition = _basePosition + new Vector3(0f, bob, 0f);
        }
    }
}
