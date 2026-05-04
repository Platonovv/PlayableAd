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

            // +N стартует от лейбла сундука и летит к лейблу игрока. Лейбл сундука одновременно прячется.
            if (ctx.Numbers != null)
            {
                var number = ctx.Numbers.Rent();
                var startPos = chest.PowerLabel != null ? chest.PowerLabel.transform.position : chest.Anchor.position;
                var targetTransform = ctx.Player.PowerLabel != null
                    ? ctx.Player.PowerLabel.transform
                    : ctx.Player.Anchor;
                if (chest.PowerLabel != null) chest.PowerLabel.SetVisible(false);
                number.PlayFlying("+" + gain, startPos, targetTransform,
                    ctx.Balance.FloatingNumberDuration, ctx.Numbers, ct).Forget();
            }

            ctx.Vfx.Play("chest_open", chest.Vfx.position);

            // Игрок «радуется» открытию сундука, чтобы не стоять как столб.
            ctx.Player.PlayVictory();
            await chest.PlayOpen(ct);

            // Когда +N долетел: лейбл, pop, VFX-приёма, flair героя.
            await UniTask.Delay(System.TimeSpan.FromSeconds(ctx.Balance.FloatingNumberDuration), cancellationToken: ct);
            ctx.Player.RefreshPower();
            if (ctx.Player.PowerLabel != null) ctx.Player.PowerLabel.Pop();
            ctx.Vfx.Play("power_gain", ctx.Player.Vfx.position);
            ctx.Signals.Fire(new PlayerPowerChangedSignal(ctx.Battle.Player.Power.Value));
            ctx.Player.PlayPowerGain(ct).Forget();

            // Большой VFX-вспышка апгрейда вокруг героя.
            ctx.Vfx.Play("upgrade", ctx.Player.Vfx.position);
            await ctx.Player.PlayUpgrade(ct);
            ctx.Player.PlayIdle();

            _flow.GoIdle();
        }
    }
}
