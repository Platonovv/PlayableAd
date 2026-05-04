using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Configs;
using Project.Core;
using UnityEngine;

namespace Project.Gameplay.Units
{
    /// <summary>
    /// View противника: idle, реакция на удар и анимация смерти.
    /// </summary>
    public sealed class EnemyView : UnitView
    {
        private static readonly int IdleHash   = Animator.StringToHash("Idle");
        private static readonly int HitHash    = Animator.StringToHash("Hit");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int DeathHash  = Animator.StringToHash("Death");

        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _vfxOnDeath;

        private BalanceConfig _balance;

        public void Configure(BalanceConfig balance) => _balance = balance;

        public async UniTask PlayDeath(CancellationToken ct)
        {
            if (_animator != null) _animator.CrossFadeInFixedTime(DeathHash, 0.05f);
            if (_vfxOnDeath != null) Instantiate(_vfxOnDeath, Anchor.position, Quaternion.identity);

            // Ждём полную анимацию смерти, затем плавно уменьшаем юнит до нуля.
            await UniTask.Delay(System.TimeSpan.FromSeconds(_balance.DeathAnimDuration), cancellationToken: ct);
            await Tween.Scale(transform, Vector3.zero, _balance.DeathFadeDuration, Ease.InQuad, ct);
            gameObject.SetActive(false);
        }

        public void PlayHit()
        {
            if (_animator != null) _animator.CrossFadeInFixedTime(HitHash, 0.05f);
        }

        public async UniTask PlayAttack(CancellationToken ct)
        {
            if (_animator != null) _animator.CrossFadeInFixedTime(AttackHash, 0.05f);
            await UniTask.Delay(System.TimeSpan.FromSeconds(_balance.AttackWindup + _balance.AttackImpactDelay), cancellationToken: ct);
        }

        public void PlayIdle()
        {
            if (_animator != null) _animator.CrossFadeInFixedTime(IdleHash, 0.05f);
        }
    }
}
