using System.Collections;
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
		[SerializeField] private Animator _animator;

		private BalanceConfig _balance;

		public void Configure(BalanceConfig balance) => _balance = balance;

		protected override void Awake()
		{
			base.Awake();
			if (_animator != null)
			{
				// Luna Playworks ругается на cullingMode setter — оставляем дефолт (AlwaysAnimate
				// нужен только если объект уезжает за границы фрустума; у нас все юниты в кадре).
				_animator.applyRootMotion = false;
			}
		}

		public IEnumerator PlayDeath()
		{
			Cross("Death");
			// Дефолты на случай отсутствия Configure (Luna строит билд иначе чем Editor — параноим).
			var deathDuration = _balance != null ? _balance.DeathAnimDuration : 0.4f;
			var fadeDuration = _balance != null ? _balance.DeathFadeDuration : 0.2f;
			yield return new WaitForSeconds(deathDuration);
			yield return Tween.Scale(transform, Vector3.zero, fadeDuration, Ease.InQuad);
			if (this != null) gameObject.SetActive(false);
		}

		public void PlayHit() => Cross("Hit");

		public IEnumerator PlayAttack()
		{
			Cross("Attack");
			var windup = _balance != null ? _balance.AttackWindup : 0.2f;
			var impact = _balance != null ? _balance.AttackImpactDelay : 0.15f;
			yield return new WaitForSeconds(windup + impact);
		}

		public void PlayIdle() => Cross("Idle");

		private void Cross(string state) => PlayAnim(state);
	}
}
