using Project.Core;
using Project.Domain.States;

namespace Project.Gameplay.Flow.States
{
	public sealed class LostState : IState<BattleFlowContext>
	{
		public void Enter(BattleFlowContext ctx)
		{
			ctx.Input.SetEnabled(false);
			ctx.Indicator.Hide();
			ctx.Signals.Fire(default(BattleLostSignal));
		}

		public void Exit(BattleFlowContext ctx)
		{
		}
	}
}
