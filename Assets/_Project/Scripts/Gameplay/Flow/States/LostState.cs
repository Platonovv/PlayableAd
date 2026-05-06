using Project.Core;
using Project.Domain.States;

namespace Project.Gameplay.Flow.States
{
    /// <summary>
    /// Терминальное «поражение»: <see cref="BattleLostSignal"/>, инпут off.
    /// </summary>
    public sealed class LostState : IState<BattleFlowContext>
    {
        public void Enter(BattleFlowContext ctx)
        {
            UnityEngine.Debug.Log("[flow] LostState.Enter — firing BattleLostSignal");
            ctx.Input.SetEnabled(false);
            ctx.Indicator.Hide();
            ctx.Signals.Fire(new BattleLostSignal());
        }

        public void Exit(BattleFlowContext ctx) { }
    }
}
