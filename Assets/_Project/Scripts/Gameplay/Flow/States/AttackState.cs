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
				await ctx.Player.PlayAttack(ct);
				ctx.Battle.Apply(new AttackAction(enemy.Id));

				enemy.PlayHit();
				ctx.Shake.Shake(ctx.Balance.HitShakeAmplitude, ctx.Balance.HitShakeDuration);
				ctx.Vfx.Play("hit", enemy.Anchor.position);
				ctx.Signals.Fire(new AttackResolvedSignal(ctx.Battle.Player.Id, enemy.Id));

				if (ctx.Numbers != null)
				{
					var number = ctx.Numbers.Rent();
					number.PlayFlying("+" + enemyPower,
					                  enemy.Anchor.position,
					                  ctx.Player.Anchor,
					                  ctx.Balance.FloatingNumberDuration,
					                  ctx.Numbers,
					                  ct).Forget();
				}

				// Дать видимую реакцию на удар, потом запустить смерть.
				await UniTask.Delay(System.TimeSpan.FromSeconds(ctx.Balance.HitReactionDelay), cancellationToken: ct);

				// Параллельно: число долетает до игрока и обновляет лейбл, враг проигрывает анимацию смерти.
				var refreshTask = RefreshPlayerOnNumberLand(ctx, ct);
				await enemy.PlayDeath(ct);
				await refreshTask;

				await ctx.Player.PlayRecover(ct);

				if (ctx.Battle.IsOver) _flow.GoWon();
				else                   _flow.GoIdle();
			}
			else
			{
				// Враг сильнее → он бьёт первым → игрок получает урон → анимация смерти → исчезает.
				ctx.Signals.Fire(new AttackFailedSignal(ctx.Battle.Player.Id, enemy.Id));

				await enemy.PlayAttack(ct);

				ctx.Player.PlayHurt();
				ctx.Vfx.Play("block", ctx.Player.Anchor.position);
				ctx.Shake.Shake(ctx.Balance.HitShakeAmplitude, ctx.Balance.HitShakeDuration);

				await UniTask.Delay(System.TimeSpan.FromSeconds(ctx.Balance.HitReactionDelay), cancellationToken: ct);
				await ctx.Player.PlayDeath(ct);
				_flow.GoLost();
			}

			_ = preAttackPower; // подавляем предупреждение, если в дальнейшем не используется
		}

		private async UniTask RefreshPlayerOnNumberLand(BattleFlowContext ctx, CancellationToken ct)
		{
			// Когда +N "впитывается" в игрока — обновляем PowerLabel и эмитим сигнал.
			await UniTask.Delay(System.TimeSpan.FromSeconds(ctx.Balance.FloatingNumberDuration * 0.85f), cancellationToken: ct);
			ctx.Player.RefreshPower();
			ctx.Signals.Fire(new PlayerPowerChangedSignal(ctx.Battle.Player.Power.Value));
		}
	}
}