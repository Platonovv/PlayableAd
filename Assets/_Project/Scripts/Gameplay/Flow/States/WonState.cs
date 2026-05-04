using Project.Core;
using Project.Domain.States;

namespace Project.Gameplay.Flow.States
{
    /// <summary>
    /// Терминальное «уровень пройден»: анимация победы, <see cref="BattleWonSignal"/>, инпут off.
    /// </summary>
    public sealed class WonState : IState<BattleFlowContext>
    {
        public void Enter(BattleFlowContext ctx)
        {
            ctx.Input.SetEnabled(false);
            ctx.Indicator.Hide();
            ctx.Player.PlayVictory();
            ctx.Signals.Fire(new BattleWonSignal());
        }

        public void Exit(BattleFlowContext ctx) { }
    }
}
