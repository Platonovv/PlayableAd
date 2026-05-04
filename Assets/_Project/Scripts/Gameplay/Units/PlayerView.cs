using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Configs;
using Project.Core;
using UnityEngine;

namespace Project.Gameplay.Units
{
	/// <summary>
	/// View игрока: анимации, движение к цели и апгрейд (получение меча из сундука).
	/// </summary>
	public sealed class PlayerView : UnitView
	{
		private static readonly int IdleHash = Animator.StringToHash("Idle");
		private static readonly int RunHash = Animator.StringToHash("Run");
		private static readonly int AttackHash = Animator.StringToHash("Attack");
		private static readonly int HitHash = Animator.StringToHash("Hit");
		private static readonly int DeathHash = Animator.StringToHash("Death");
		private static readonly int VictoryHash = Animator.StringToHash("Victory");
		private static readonly int UpgradeHash = Animator.StringToHash("Upgrade");
		private static readonly int SuperAttackHash = Animator.StringToHash("SuperAttack");

		[SerializeField] private Animator _animator;
		[SerializeField] private Transform _sword;

		private BalanceConfig _balance;

		public Vector3 SwordTarget => _sword != null ? _sword.position : Anchor.position;

		public void Configure(BalanceConfig balance) => _balance = balance;

		protected override void Awake()
		{
			base.Awake();
			if (_sword != null)
				_sword.localScale = Vector3.zero;
			if (_animator != null)
				_animator.applyRootMotion = false;
		}

		public async UniTask MoveTo(Vector3 destination, CancellationToken ct)
		{
			destination.y = transform.position.y;

			PlayState(RunHash);
			FaceTowards(destination);
			var stopSqr = _balance.StopDistance * _balance.StopDistance;
			while ((transform.position - destination).sqrMagnitude > stopSqr)
			{
				if (this == null)
					return;

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
			await UniTask.Delay(System.TimeSpan.FromSeconds(_balance.AttackWindup + _balance.AttackImpactDelay),
			                    cancellationToken: ct);
		}

		public async UniTask PlaySuperAttack(CancellationToken ct)
		{
			PlayState(SuperAttackHash);
			await UniTask.Delay(System.TimeSpan.FromSeconds(_balance.AttackWindup + _balance.AttackImpactDelay),
			                    cancellationToken: ct);
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
			PlayState(UpgradeHash);
			if (_sword != null)
				_sword.localScale = Vector3.one;

			await RandomFlair(_balance.UpgradeDuration, ct);
			await UniTask.Delay(System.TimeSpan.FromSeconds(_balance.UpgradeAnimTail), cancellationToken: ct);
		}

		public UniTask PlayPowerGain(CancellationToken ct) => RandomFlair(_balance.UpgradeDuration * 0.6f, ct);

		public void PlayIdle() => PlayState(IdleHash);

		public void PlayVictory() => PlayState(VictoryHash);

		public void PlayHurt() => PlayState(HitHash);

		public async UniTask PlayDeath(CancellationToken ct)
		{
			PlayState(DeathHash);
			await UniTask.Delay(System.TimeSpan.FromSeconds(_balance.DeathAnimDuration), cancellationToken: ct);
			await Tween.Scale(transform, Vector3.zero, _balance.DeathFadeDuration, Ease.InQuad, ct);
		}

		public override void FaceTowards(Vector3 worldTarget)
		{
			var localUp = transform.up;
			var dir = Vector3.ProjectOnPlane(worldTarget - transform.position, localUp);
			if (dir.sqrMagnitude < 0.0001f)
				return;

			transform.rotation = Quaternion.Slerp(transform.rotation,
			                                      Quaternion.LookRotation(dir, localUp),
			                                      18f * Time.deltaTime);
		}

		private UniTask RandomFlair(float duration, CancellationToken ct)
		{
			switch (Random.Range(0, 5))
			{
				case 0:
					return Tween.Punch(transform, 0.30f, duration, ct);
				case 1:
					return HopUp(duration, ct);
				case 2:
					return Squash(duration, ct);
				case 3:
					return DoubleHop(duration, ct);
				default:
					return SpeedBurst(duration, ct);
			}
		}

		private async UniTask SpeedBurst(float duration, CancellationToken ct)
		{
			if (_animator == null) return;
			var prev = _animator.speed;
			_animator.speed = 1.6f;

			var elapsed = 0f;
			while (elapsed < duration && this != null && !ct.IsCancellationRequested)
			{
				elapsed += Time.deltaTime;
				await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			}

			if (this != null && _animator != null)
				_animator.speed = prev;
		}

		private async UniTask HopUp(float duration, CancellationToken ct)
		{
			const float hopHeight = 0.7f;
			var startY = transform.position.y;
			var elapsed = 0f;
			while (elapsed < duration && this != null && !ct.IsCancellationRequested)
			{
				elapsed += Time.deltaTime;
				var k = Mathf.Clamp01(elapsed / duration);
				var y = startY + Mathf.Sin(k * Mathf.PI) * hopHeight;
				var p = transform.position;
				p.y = y;
				transform.position = p;
				await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			}

			if (this != null)
			{
				var end = transform.position;
				end.y = startY;
				transform.position = end;
			}
		}

		private async UniTask Squash(float duration, CancellationToken ct)
		{
			var origin = transform.localScale;
			var elapsed = 0f;
			while (elapsed < duration && this != null && !ct.IsCancellationRequested)
			{
				elapsed += Time.deltaTime;
				var k = Mathf.Clamp01(elapsed / duration);
				var s = Mathf.Sin(k * Mathf.PI * 2f);
				transform.localScale = new Vector3(origin.x * (1f + s * 0.18f),
				                                   origin.y * (1f - s * 0.18f),
				                                   origin.z * (1f + s * 0.18f));
				await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			}

			if (this != null)
				transform.localScale = origin;
		}

		private async UniTask DoubleHop(float duration, CancellationToken ct)
		{
			const float hopHeight = 0.4f;
			var startY = transform.position.y;
			var elapsed = 0f;
			while (elapsed < duration && this != null && !ct.IsCancellationRequested)
			{
				elapsed += Time.deltaTime;
				var k = Mathf.Clamp01(elapsed / duration);
				var y = startY + Mathf.Abs(Mathf.Sin(k * Mathf.PI * 2f)) * hopHeight;
				var p = transform.position;
				p.y = y;
				transform.position = p;
				await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			}

			if (this != null)
			{
				var end = transform.position;
				end.y = startY;
				transform.position = end;
			}
		}

		private void PlayState(int hash)
		{
			if (_animator != null)
				_animator.CrossFadeInFixedTime(hash, 0.08f);
		}
	}
}