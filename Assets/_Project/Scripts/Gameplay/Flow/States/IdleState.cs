using Project.Domain.States;

namespace Project.Gameplay.Flow.States
{
    /// <summary>
    /// Стартовое состояние FSM. Включает инпут и ждёт, пока игрок выберет цель.
    /// </summary>
    public sealed class IdleState : IState<BattleFlowContext>
    {
        public void Enter(BattleFlowContext ctx) => ctx.Input.SetEnabled(true);
        public void Exit(BattleFlowContext ctx) => ctx.Input.SetEnabled(false);
    }
}
