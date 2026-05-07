using Project.Core;
using Project.Domain.States;

namespace Project.Gameplay.Flow.States
{
	public sealed class WonState : IState<BattleFlowContext>
	{
		public void Enter(BattleFlowContext ctx)
		{
			ctx.Input.SetEnabled(false);
			ctx.Indicator.Hide();
			ctx.Player.PlayVictory();
			ctx.Signals.Fire(default(BattleWonSignal));
		}

		public void Exit(BattleFlowContext ctx)
		{
		}
	}
}
