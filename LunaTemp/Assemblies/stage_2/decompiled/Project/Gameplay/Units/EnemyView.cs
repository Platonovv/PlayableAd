using System.Collections;
using Project.Configs;
using Project.Core;
using UnityEngine;

namespace Project.Gameplay.Units
{
	public sealed class EnemyView : UnitView
	{
		private BalanceConfig _balance;

		public void Configure(BalanceConfig balance)
		{
			_balance = balance;
		}

		public IEnumerator PlayDeath()
		{
			Cross("Death");
			float deathDuration = ((_balance != null) ? _balance.DeathAnimDuration : 0.4f);
			float fadeDuration = ((_balance != null) ? _balance.DeathFadeDuration : 0.2f);
			yield return new WaitForSeconds(deathDuration);
			yield return Tween.Scale(base.transform, Vector3.zero, fadeDuration, Ease.InQuad);
			if (this != null)
			{
				base.gameObject.SetActive(false);
			}
		}

		public void PlayHit()
		{
			Cross("Hit");
		}

		public IEnumerator PlayAttack()
		{
			Cross("Attack");
			float windup = ((_balance != null) ? _balance.AttackWindup : 0.2f);
			float impact = ((_balance != null) ? _balance.AttackImpactDelay : 0.15f);
			yield return new WaitForSeconds(windup + impact);
		}

		public void PlayIdle()
		{
			Cross("Idle");
		}

		private void Cross(string state)
		{
			PlayAnim(state);
		}
	}
}
