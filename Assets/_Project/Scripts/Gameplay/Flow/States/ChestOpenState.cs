using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Core;
using Project.Domain.Actions;
using Project.Domain.States;
using Project.Gameplay.Units;

namespace Project.Gameplay.Flow.States
{
    /// <summary>
    /// Открытие сундука: <see cref="CollectChestAction"/>, анимация открытия, апгрейд героя на gold-материал.
    /// </summary>
    public sealed class ChestOpenState : IState<BattleFlowContext>
    {
        private readonly BattleFlow _flow;
        private CancellationTokenSource _cts;

        public ChestOpenState(BattleFlow flow) => _flow = flow;

        public void Enter(BattleFlowContext ctx)
        {
            _cts = new CancellationTokenSource();
            _ = Run(ctx, _cts.Token);
        }

        public void Exit(BattleFlowContext ctx) => _cts?.Cancel();

        private async UniTask Run(BattleFlowContext ctx, CancellationToken ct)
        {
            if (!ctx.Views.TryGetValue(ctx.PendingTarget, out var view) || view is not ChestView chest)
            {
                _flow.GoIdle();
                return;
            }

            var gain = chest.Unit.Power.Value;
            ctx.Battle.Apply(new CollectChestAction(chest.Id));
            ctx.Signals.Fire(new ChestCollectedSignal(chest.Id, gain));

            if (ctx.Numbers != null)
            {
                var number = ctx.Numbers.Rent();
                // +N из сундука летит к игроку — как при убийстве врага.
                number.PlayFlying("+" + gain, chest.Anchor.position, ctx.Player.Anchor,
                    ctx.Balance.FloatingNumberDuration, ctx.Numbers, ct).Forget();
            }

            ctx.Vfx.Play("chest_open", chest.Anchor.position);
            await chest.PlayOpen(ct);

            // На подлёте +N — обновляем PowerLabel и стартует апгрейд (gold-материал).
            await UniTask.Delay(System.TimeSpan.FromSeconds(ctx.Balance.FloatingNumberDuration * 0.85f), cancellationToken: ct);
            ctx.Player.RefreshPower();
            ctx.Signals.Fire(new PlayerPowerChangedSignal(ctx.Battle.Player.Power.Value));

            await ctx.Player.PlayUpgrade(ct);

            _flow.GoIdle();
        }
    }
}
