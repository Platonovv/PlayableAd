using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Core;
using Project.Domain;
using Project.Domain.Actions;
using Project.Domain.States;
using Project.Gameplay.Units;

namespace Project.Gameplay.Flow.States
{
	/// <summary>
	/// Атака врага: решение по силе принимает <see cref="Battle"/>, визуальные реакции исполняются здесь.
	/// </summary>
	public sealed class AttackState : IState<BattleFlowContext>
	{
		private readonly BattleFlow _flow;
		private CancellationTokenSource _cts;

		public AttackState(BattleFlow flow) => _flow = flow;

		public void Enter(BattleFlowContext ctx)
		{
			_cts = new CancellationTokenSource();
			_ = Run(ctx, _cts.Token);
		}

		public void Exit(BattleFlowContext ctx) => _cts?.Cancel();

		private async UniTask Run(BattleFlowContext ctx, CancellationToken ct)
		{
			if (!ctx.Views.TryGetValue(ctx.PendingTarget, out var view) || view is not EnemyView enemy)
			{
				_flow.GoIdle();
				return;
			}

			var enemyOrigin = ctx.Player.transform.position;
			var preAttackPower = ctx.Battle.Player.Power.Value;
			var enemyPower = enemy.Unit.Power.Value;
			var willWin = ctx.Battle.Player.Power >= enemy.Unit.Power;

			// Противник поворачивается лицом к игроку перед самим боем.
			enemy.FaceTowards(ctx.Player.transform.position);

			if (willWin)
			{
				// Игрок бьёт → враг получает урон → анимация смерти → исчезает.
				// Если сила выше порога — играет супер-атака.
				var useSuper = ctx.Battle.Player.Power.Value >= ctx.Balance.SuperAttackThreshold;
				await (useSuper ? ctx.Player.PlaySuperAttack(ct) : ctx.Player.PlayAttack(ct));
				ctx.Battle.Apply(new AttackAction(enemy.Id));

				enemy.PlayHit();
				ctx.Shake.Shake(ctx.Balance.HitShakeAmplitude, ctx.Balance.HitShakeDuration);
				ctx.Vfx.Play(useSuper ? "super_hit" : "hit", enemy.Vfx.position);
				ctx.Signals.Fire(new AttackResolvedSignal(ctx.Battle.Player.Id, enemy.Id));

				// +N стартует от лейбла врага и летит к лейблу игрока — лейбл врага одновременно прячется,
				// чтобы создать иллюзию что число «вылетело» из него.
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
					                  ctx.Numbers,
					                  ct)
					      .Forget();
				}

				// Дать видимую реакцию на удар, потом запустить смерть.
				await UniTask.Delay(System.TimeSpan.FromSeconds(ctx.Balance.HitReactionDelay), cancellationToken: ct);

				// Игрок возвращается в idle сразу после удара — нет смысла ждать смерть врага в боевой стойке.
				ctx.Player.PlayIdle();

				// Параллельно: число долетает до игрока и обновляет лейбл, враг проигрывает анимацию смерти.
				ctx.Vfx.Play("death", enemy.Vfx.position);
				var refreshTask = RefreshPlayerOnNumberLand(ctx, ct);
				await enemy.PlayDeath(ct);
				await refreshTask;

				if (ctx.Battle.IsOver)
					_flow.GoWon();
				else
					_flow.GoIdle();
			}
			else
			{
				// Враг сильнее → он бьёт первым → игрок получает урон → анимация смерти → исчезает.
				ctx.Signals.Fire(new AttackFailedSignal(ctx.Battle.Player.Id, enemy.Id));

				await enemy.PlayAttack(ct);

				ctx.Player.PlayHurt();
				ctx.Vfx.Play("block", ctx.Player.Vfx.position);
				ctx.Shake.Shake(ctx.Balance.HitShakeAmplitude, ctx.Balance.HitShakeDuration);

				await UniTask.Delay(System.TimeSpan.FromSeconds(ctx.Balance.HitReactionDelay), cancellationToken: ct);
				await ctx.Player.PlayDeath(ct);
				_flow.GoLost();
			}

			_ = preAttackPower; // подавляем предупреждение, если в дальнейшем не используется
		}

		private async UniTask RefreshPlayerOnNumberLand(BattleFlowContext ctx, CancellationToken ct)
		{
			// Когда +N долетел до игрока: обновляем лейбл, pop, VFX-приёма и flair-эффект героя.
			await UniTask.Delay(System.TimeSpan.FromSeconds(ctx.Balance.FloatingNumberDuration),
			                    cancellationToken: ct);
			ctx.Player.RefreshPower();
			if (ctx.Player.PowerLabel != null)
				ctx.Player.PowerLabel.Pop();
			ctx.Vfx.Play("power_gain", ctx.Player.Vfx.position);
			ctx.Signals.Fire(new PlayerPowerChangedSignal(ctx.Battle.Player.Power.Value));
			ctx.Player.PlayPowerGain(ct).Forget();
		}
	}
}