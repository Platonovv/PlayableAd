using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Domain.States;

namespace Project.Gameplay.Flow.States
{
	/// <summary>
	/// Движение игрока к точке <see cref="Units.UnitView.Stop"/>; по прибытии — атака или сундук.
	/// </summary>
	public sealed class MovingToTargetState : IState<BattleFlowContext>
	{
		private readonly BattleFlow _flow;
		private CancellationTokenSource _cts;

		public MovingToTargetState(BattleFlow flow) => _flow = flow;

		public void Enter(BattleFlowContext ctx)
		{
			_cts = new CancellationTokenSource();
			_ = Run(ctx, _cts.Token);
		}

		public void Exit(BattleFlowContext ctx) => _cts?.Cancel();

		private async UniTask Run(BattleFlowContext ctx, CancellationToken ct)
		{
			if (!ctx.Views.TryGetValue(ctx.PendingTarget, out var view))
			{
				_flow.GoIdle();
				return;
			}

			// Бежим к точке остановки, заданной в инспекторе UnitView (Stop).
			// Если StopPoint не задан, fallback — корень префаба.
			ctx.Indicator.Show(ctx.Player.transform, view.Stop, ctx.ColorFor(view));
			await ctx.Player.MoveTo(view.Stop.position, ct);
			ctx.Indicator.Hide();

			switch (view.Kind)
			{
				case Domain.UnitKind.Chest:
					_flow.GoChestOpen();
					break;
				case Domain.UnitKind.Enemy:
					_flow.GoAttack();
					break;
				default:
					_flow.GoIdle();
					break;
			}
		}
	}
}