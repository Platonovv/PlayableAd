using Project.Domain.States;

namespace Project.Gameplay.Flow.States
{
	public sealed class IdleState : IState<BattleFlowContext>
	{
		public void Enter(BattleFlowContext ctx)
		{
			ctx.Input.SetEnabled(true);
		}

		public void Exit(BattleFlowContext ctx)
		{
			ctx.Input.SetEnabled(false);
		}
	}
}
