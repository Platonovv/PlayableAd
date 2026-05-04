using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Configs;
using Project.Core;
using UnityEngine;

namespace Project.Gameplay.Units
{
    /// <summary>
    /// View игрока: анимации, движение к цели и апгрейд (смена материала на gold после сундука).
    /// </summary>
    public sealed class PlayerView : UnitView
    {
        private static readonly int IdleHash   = Animator.StringToHash("Idle");
        private static readonly int RunHash    = Animator.StringToHash("Run");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int HitHash    = Animator.StringToHash("Hit");
        private static readonly int DeathHash  = Animator.StringToHash("Death");
        private static readonly int VictoryHash= Animator.StringToHash("Victory");

        [SerializeField] private Animator _animator;
        [SerializeField] private Renderer _bodyRenderer;
        [SerializeField] private Material _defaultMaterial;
        [SerializeField] private Material _upgradedMaterial;
        [SerializeField] private ParticleSystem _upgradeBurst;

        private BalanceConfig _balance;

        public void Configure(BalanceConfig balance) => _balance = balance;

        public async UniTask MoveTo(Vector3 destination, CancellationToken ct)
        {
            // Игнорируем Y назначения — все юниты живут в одной плоскости, заданной стартовой высотой игрока.
            destination.y = transform.position.y;

            PlayState(RunHash);
            FaceTowards(destination);
            var stopSqr = _balance.StopDistance * _balance.StopDistance;
            while ((transform.position - destination).sqrMagnitude > stopSqr)
            {
                if (this == null) return;
                ct.ThrowIfCancellationRequested();
                var step = _balance.MoveSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, destination, step);
                FaceTowards(destination);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
            PlayState(IdleHash);
        }

        public async UniTask PlayAttack(CancellationToken ct)
        {
            PlayState(AttackHash);
            await UniTask.Delay(System.TimeSpan.FromSeconds(_balance.AttackWindup + _balance.AttackImpactDelay), cancellationToken: ct);
        }

        public async UniTask PlayRecover(CancellationToken ct)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(_balance.AttackRecover), cancellationToken: ct);
            PlayState(IdleHash);
        }

        public async UniTask PlayBounceBack(Vector3 origin, CancellationToken ct)
        {
            PlayState(HitHash);
            await Tween.Move(transform, origin, _balance.FailedAttackBounce, Ease.OutBack, ct);
            PlayState(IdleHash);
        }

        public async UniTask PlayUpgrade(CancellationToken ct)
        {
            if (_upgradeBurst != null) _upgradeBurst.Play();
            if (_bodyRenderer != null && _upgradedMaterial != null)
                _bodyRenderer.sharedMaterial = _upgradedMaterial;
            await Tween.Punch(transform, 0.18f, _balance.UpgradeDuration, ct);
        }

        public void PlayVictory() => PlayState(VictoryHash);

        public void PlayHurt() => PlayState(HitHash);

        public async UniTask PlayDeath(CancellationToken ct)
        {
            PlayState(DeathHash);
            // Сначала полная анимация смерти, потом плавный уход в ноль.
            await UniTask.Delay(System.TimeSpan.FromSeconds(_balance.DeathAnimDuration), cancellationToken: ct);
            await Tween.Scale(transform, Vector3.zero, _balance.DeathFadeDuration, Ease.InQuad, ct);
        }

        private void PlayState(int hash)
        {
            if (_animator != null) _animator.CrossFadeInFixedTime(hash, 0.08f);
        }

        public override void FaceTowards(Vector3 worldTarget)
        {
            var dir = worldTarget - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(dir), 18f * Time.deltaTime);
        }
    }
}
