using System.Collections;
using Project.Core;
using Project.Domain;
using Project.Domain.Actions;
using Project.Domain.States;
using Project.Gameplay.Units;
using UnityEngine;

namespace Project.Gameplay.Flow.States
{
	/// <summary>
	/// Атака врага: решение по силе принимает <see cref="Battle"/>, визуальные реакции исполняются здесь.
	/// </summary>
	public sealed class AttackState : IState<BattleFlowContext>
	{
		private readonly BattleFlow _flow;
		private Coroutine _co;
		private Coroutine _refreshCo;

		public AttackState(BattleFlow flow) => _flow = flow;

		public void Enter(BattleFlowContext ctx)
		{
			_co = _flow.StartCoroutine(Run(ctx));
		}

		public void Exit(BattleFlowContext ctx)
		{
			if (_co != null) { _flow.StopCoroutine(_co); _co = null; }
			if (_refreshCo != null) { _flow.StopCoroutine(_refreshCo); _refreshCo = null; }
		}

		private IEnumerator Run(BattleFlowContext ctx)
		{
			if (!ctx.Views.TryGetValue(ctx.PendingTarget, out var view) || !(view is EnemyView enemy))
			{
				_flow.GoIdle();
				yield break;
			}

			var enemyPower = enemy.Unit.Power.Value;
			var willWin = ctx.Battle.Player.Power >= enemy.Unit.Power;

			enemy.FaceTowards(ctx.Player.transform.position);

			if (willWin)
			{
				var useSuper = ctx.Battle.Player.Power.Value >= ctx.Balance.SuperAttackThreshold;
				yield return useSuper ? ctx.Player.PlaySuperAttack() : ctx.Player.PlayAttack();
				ctx.Battle.Apply(new AttackAction(enemy.Id));

				enemy.PlayHit();
				ctx.Shake.Shake(ctx.Balance.HitShakeAmplitude, ctx.Balance.HitShakeDuration);
				ctx.Vfx.Play(useSuper ? "super_hit" : "hit", enemy.Vfx.position);
				ctx.Signals.Fire(new AttackResolvedSignal(ctx.Battle.Player.Id, enemy.Id));

				if (ctx.Numbers != null)
				{
					var number = ctx.Numbers.Rent();
					var startPos = enemy.PowerLabel != null
						               ? enemy.PowerLabel.transform.position
						               : enemy.Anchor.position;
					var targetTransform = ctx.Player.PowerLabel != null
						                      ? ctx.Player.PowerLabel.transform
						                      : ctx.Player.Anchor;
					if (enemy.PowerLabel != null)
						enemy.PowerLabel.SetVisible(false);
					number.PlayFlying("+" + enemyPower,
					                  startPos,
					                  targetTransform,
					                  ctx.Balance.FloatingNumberDuration,
					                  ctx.Numbers);
				}

				yield return new WaitForSeconds(ctx.Balance.HitReactionDelay);

				ctx.Player.PlayIdle();

				ctx.Vfx.Play("death", enemy.Vfx.position);
				_refreshCo = _flow.StartCoroutine(RefreshPlayerOnNumberLand(ctx));
				yield return enemy.PlayDeath();

				_co = null;
				if (ctx.Battle.IsOver)
					_flow.GoWon();
				else
					_flow.GoIdle();
			}
			else
			{
				ctx.Signals.Fire(new AttackFailedSignal(ctx.Battle.Player.Id, enemy.Id));

				yield return enemy.PlayAttack();

				ctx.Player.PlayHurt();
				ctx.Vfx.Play("block", ctx.Player.Vfx.position);
				ctx.Shake.Shake(ctx.Balance.HitShakeAmplitude, ctx.Balance.HitShakeDuration);

				yield return new WaitForSeconds(ctx.Balance.HitReactionDelay);
				yield return ctx.Player.PlayDeath();
				_co = null;
				_flow.GoLost();
			}
		}

		private IEnumerator RefreshPlayerOnNumberLand(BattleFlowContext ctx)
		{
			yield return new WaitForSeconds(ctx.Balance.FloatingNumberDuration);
			ctx.Player.RefreshPower();
			if (ctx.Player.PowerLabel != null)
				ctx.Player.PowerLabel.Pop();
			ctx.Vfx.Play("power_gain", ctx.Player.Vfx.position);
			ctx.Signals.Fire(new PlayerPowerChangedSignal(ctx.Battle.Player.Power.Value));
			ctx.Player.PlayPowerGain();
			_refreshCo = null;
		}
	}
}
