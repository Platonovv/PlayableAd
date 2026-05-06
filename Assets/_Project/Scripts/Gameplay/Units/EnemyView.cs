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
				_animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				_animator.applyRootMotion = false;
			}
		}

		public IEnumerator PlayDeath()
		{
			Cross("Death");
			yield return new WaitForSeconds(_balance.DeathAnimDuration);
			yield return Tween.Scale(transform, Vector3.zero, _balance.DeathFadeDuration, Ease.InQuad);
			if (this != null) gameObject.SetActive(false);
		}

		public void PlayHit() => Cross("Hit");

		public IEnumerator PlayAttack()
		{
			Cross("Attack");
			yield return new WaitForSeconds(_balance.AttackWindup + _balance.AttackImpactDelay);
		}

		public void PlayIdle() => Cross("Idle");

		private void Cross(string state) => PlayAnim(state);
	}
}
