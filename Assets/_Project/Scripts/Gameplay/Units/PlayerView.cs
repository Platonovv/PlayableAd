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
		private static readonly int IdleHash = Animator.StringToHash("Idle");
		private static readonly int RunHash = Animator.StringToHash("Run");
		private static readonly int AttackHash = Animator.StringToHash("Attack");
		private static readonly int HitHash = Animator.StringToHash("Hit");
		private static readonly int DeathHash = Animator.StringToHash("Death");
		private static readonly int VictoryHash = Animator.StringToHash("Victory");
		private static readonly int UpgradeHash = Animator.StringToHash("Upgrade");
		private static readonly int SuperAttackHash = Animator.StringToHash("SuperAttack");

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
			if (_upgradeBurst != null)
				_upgradeBurst.Play();
			if (_bodyRenderer != null && _upgradedMaterial != null)
				_bodyRenderer.sharedMaterial = _upgradedMaterial;

			await RandomFlair(_balance.UpgradeDuration, ct);
			// Хвост: дать анимации Upgrade доиграть, прежде чем стейт сменится.
			await UniTask.Delay(System.TimeSpan.FromSeconds(_balance.UpgradeAnimTail), cancellationToken: ct);
		}

		public UniTask PlayPowerGain(CancellationToken ct) => RandomFlair(_balance.UpgradeDuration * 0.6f, ct);

		private UniTask RandomFlair(float duration, CancellationToken ct)
		{
			switch (Random.Range(0, 3))
			{
				case 0:
					return Tween.Punch(transform, 0.30f, duration, ct);
				case 1:
					return HopUp(duration, ct);
				default:
					return SpinAroundY(duration, ct);
			}
		}

		private async UniTask HopUp(float duration, CancellationToken ct)
		{
			const float hopHeight = 0.7f;
			var startY = transform.position.y;
			var elapsed = 0f;
			while (elapsed < duration)
			{
				if (this == null || ct.IsCancellationRequested)
					return;

				elapsed += Time.deltaTime;
				var k = Mathf.Clamp01(elapsed / duration);
				var y = startY + Mathf.Sin(k * Mathf.PI) * hopHeight;
				var p = transform.position;
				p.y = y;
				transform.position = p;
				await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			}

			var end = transform.position;
			end.y = startY;
			transform.position = end;
		}

		private async UniTask SpinAroundY(float duration, CancellationToken ct)
		{
			var startEuler = transform.eulerAngles;
			var elapsed = 0f;
			while (elapsed < duration)
			{
				if (this == null || ct.IsCancellationRequested)
					return;

				elapsed += Time.deltaTime;
				var k = Mathf.Clamp01(elapsed / duration);
				transform.eulerAngles = new Vector3(startEuler.x, startEuler.y + 360f * k, startEuler.z);
				await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			}

			transform.eulerAngles = startEuler;
		}

		public void PlayIdle() => PlayState(IdleHash);

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
			if (_animator != null)
				_animator.CrossFadeInFixedTime(hash, 0.08f);
		}

		public override void FaceTowards(Vector3 worldTarget)
		{
			// Поворот вокруг локального UP — сохраняет наклон под BG-плоскость.
			var localUp = transform.up;
			var dir = Vector3.ProjectOnPlane(worldTarget - transform.position, localUp);
			if (dir.sqrMagnitude < 0.0001f)
				return;

			transform.rotation = Quaternion.Slerp(transform.rotation,
			                                      Quaternion.LookRotation(dir, localUp),
			                                      18f * Time.deltaTime);
		}
	}
}