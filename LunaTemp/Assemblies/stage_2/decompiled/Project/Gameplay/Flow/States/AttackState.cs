using System.Collections;
using Project.Core;
using Project.Domain.Actions;
using Project.Domain.States;
using Project.Gameplay.Units;
using Project.Gameplay.Vfx;
using UnityEngine;

namespace Project.Gameplay.Flow.States
{
	public sealed class AttackState : IState<BattleFlowContext>
	{
		private readonly BattleFlow _flow;

		private Coroutine _co;

		private Coroutine _refreshCo;

		public AttackState(BattleFlow flow)
		{
			_flow = flow;
		}

		public void Enter(BattleFlowContext ctx)
		{
			_co = _flow.StartCoroutine(Run(ctx));
		}

		public void Exit(BattleFlowContext ctx)
		{
			if (_co != null)
			{
				_flow.StopCoroutine(_co);
				_co = null;
			}
			if (_refreshCo != null)
			{
				_flow.StopCoroutine(_refreshCo);
				_refreshCo = null;
			}
		}

		private IEnumerator Run(BattleFlowContext ctx)
		{
			EnemyView enemy = default(EnemyView);
			int num;
			if (ctx.Views.TryGetValue(ctx.PendingTarget, out var view))
			{
				enemy = view as EnemyView;
				num = (((object)enemy == null) ? 1 : 0);
			}
			else
			{
				num = 1;
			}
			if (num != 0)
			{
				_flow.GoIdle();
				yield break;
			}
			int enemyPower = enemy.Unit.Power.Value;
			bool willWin = ctx.Battle.Player.Power >= enemy.Unit.Power;
			enemy.FaceTowards(ctx.Player.transform.position);
			if (willWin)
			{
				bool useSuper = ctx.Battle.Player.Power.Value >= ctx.Balance.SuperAttackThreshold;
				yield return useSuper ? ctx.Player.PlaySuperAttack() : ctx.Player.PlayAttack();
				ctx.Battle.Apply(new AttackAction(enemy.Id));
				enemy.PlayHit();
				ctx.Shake.Shake(ctx.Balance.HitShakeAmplitude, ctx.Balance.HitShakeDuration);
				ctx.Vfx.Play(useSuper ? "super_hit" : "hit", enemy.Vfx.position);
				ctx.Signals.Fire(new AttackResolvedSignal(ctx.Battle.Player.Id, enemy.Id));
				if (useSuper)
				{
					ctx.Shake.FovPulse(-2f, 0.18f);
				}
				yield return _flow.StartCoroutine(HitStop(0.06f));
				Vector3 pushDir = enemy.Vfx.position - ctx.Player.Vfx.position;
				pushDir.y = 0f;
				if (pushDir.sqrMagnitude > 0.0001f)
				{
					ctx.Shake.Push(pushDir.normalized * 0.6f, 0.35f);
				}
				if (ctx.Numbers != null)
				{
					FloatingNumber number = ctx.Numbers.Rent();
					Vector3 startPos = ((enemy.PowerLabel != null) ? enemy.PowerLabel.transform.position : enemy.Anchor.position);
					Transform targetTransform = ((ctx.Player.PowerLabel != null) ? ctx.Player.PowerLabel.transform : ctx.Player.Anchor);
					if (enemy.PowerLabel != null)
					{
						enemy.PowerLabel.SetVisible(false);
					}
					number.PlayFlying("+" + enemyPower, startPos, targetTransform, ctx.Balance.FloatingNumberDuration, ctx.Numbers);
				}
				yield return new WaitForSeconds(ctx.Balance.HitReactionDelay);
				ctx.Player.PlayIdle();
				ctx.Vfx.Play("death", enemy.Vfx.position);
				_refreshCo = _flow.StartCoroutine(RefreshPlayerOnNumberLand(ctx));
				yield return _flow.StartCoroutine(SafePlayDeath(enemy));
				_co = null;
				if (ctx.Battle.IsOver)
				{
					_flow.GoWon();
				}
				else
				{
					_flow.GoIdle();
				}
				yield break;
			}
			ctx.Signals.Fire(new AttackFailedSignal(ctx.Battle.Player.Id, enemy.Id));
			yield return enemy.PlayAttack();
			ctx.Player.PlayHurt();
			ctx.Vfx.Play("block", ctx.Player.Vfx.position);
			ctx.Shake.Shake(ctx.Balance.HitShakeAmplitude, ctx.Balance.HitShakeDuration);
			yield return new WaitForSeconds(ctx.Balance.HitReactionDelay);
			_flow.StartCoroutine(ctx.Player.PlayDeath());
			float elapsed = 0f;
			while (elapsed < 1.5f)
			{
				elapsed += Time.deltaTime;
				yield return null;
				if (ctx.Player == null)
				{
					break;
				}
			}
			_co = null;
			_flow.GoLost();
		}

		private IEnumerator HitStop(float duration)
		{
			float prev = Time.timeScale;
			Time.timeScale = 0.05f;
			float t = 0f;
			while (t < duration)
			{
				t += Time.unscaledDeltaTime;
				yield return null;
			}
			Time.timeScale = prev;
		}

		private IEnumerator SafePlayDeath(EnemyView enemy)
		{
			_flow.StartCoroutine(enemy.PlayDeath());
			float elapsed = 0f;
			while (elapsed < 1.5f)
			{
				elapsed += Time.deltaTime;
				yield return null;
				if (enemy == null || !enemy.gameObject.activeInHierarchy)
				{
					yield break;
				}
			}
			if (enemy != null)
			{
				enemy.gameObject.SetActive(false);
			}
		}

		private IEnumerator RefreshPlayerOnNumberLand(BattleFlowContext ctx)
		{
			yield return new WaitForSeconds(ctx.Balance.FloatingNumberDuration);
			ctx.Player.RefreshPower();
			if (ctx.Player.PowerLabel != null)
			{
				ctx.Player.PowerLabel.Pop();
			}
			ctx.Vfx.Play("power_gain", ctx.Player.Vfx.position);
			ctx.Signals.Fire(new PlayerPowerChangedSignal(ctx.Battle.Player.Power.Value));
			ctx.Player.PlayPowerGain();
			_refreshCo = null;
		}
	}
}
