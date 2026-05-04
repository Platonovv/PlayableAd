using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Core;
using Project.Domain.Actions;
using Project.Domain.States;
using Project.Gameplay.Units;
using UnityEngine;

namespace Project.Gameplay.Flow.States
{
    /// <summary>
    /// Открытие сундука: <see cref="CollectChestAction"/>, анимация открытия, меч из сундука летит в руку игрока.
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
                var startPos = chest.PowerLabel != null ? chest.PowerLabel.transform.position : chest.Anchor.position;
                var targetTransform = ctx.Player.PowerLabel != null
                    ? ctx.Player.PowerLabel.transform
                    : ctx.Player.Anchor;
                if (chest.PowerLabel != null) chest.PowerLabel.SetVisible(false);
                number.PlayFlying("+" + gain, startPos, targetTransform,
                    ctx.Balance.FloatingNumberDuration, ctx.Numbers, ct).Forget();
            }

            ctx.Vfx.Play("chest_open", chest.Vfx.position);

            ctx.Player.PlayVictory();

            var openTask = chest.PlayOpen(ct);
            var swordTask = FlySwordFromChest(chest.Vfx.position, ctx.Player, ct);

            UniTask.Delay(System.TimeSpan.FromSeconds(ctx.Balance.FloatingNumberDuration), cancellationToken: ct)
                .ContinueWith(() =>
                {
                    if (ct.IsCancellationRequested) return;
                    ctx.Player.RefreshPower();
                    if (ctx.Player.PowerLabel != null) ctx.Player.PowerLabel.Pop();
                    ctx.Vfx.Play("power_gain", ctx.Player.Vfx.position);
                    ctx.Signals.Fire(new PlayerPowerChangedSignal(ctx.Battle.Player.Power.Value));
                    ctx.Player.PlayPowerGain(ct).Forget();
                }).Forget();

            await UniTask.WhenAll(openTask, swordTask);

            ctx.Vfx.Play("upgrade", ctx.Player.Vfx.position);
            await ctx.Player.PlayUpgrade(ct);
            ctx.Player.PlayIdle();

            _flow.GoIdle();
        }

        private async UniTask FlySwordFromChest(Vector3 from, PlayerView player, CancellationToken ct)
        {
            var to = player.SwordTarget;
            const float duration = 0.6f;
            const float arcHeight = 2.5f;
            var swordSize = new Vector3(0.12f, 0.12f, 0.9f);

            var ghost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ghost.name = "FlyingSword";
            if (ghost.TryGetComponent(out Collider col)) Object.Destroy(col);

            var t = ghost.transform;
            t.position = from;
            t.localScale = swordSize;

            var renderer = ghost.GetComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            var mat = new Material(Shader.Find("Mobile/Diffuse")) { color = new Color(1f, 0.82f, 0.2f) };
            renderer.sharedMaterial = mat;

            var elapsed = 0f;
            while (elapsed < duration && ghost != null && !ct.IsCancellationRequested)
            {
                elapsed += Time.deltaTime;
                var k = Mathf.Clamp01(elapsed / duration);
                var eased = Ease.InOutQuad(k);
                var pos = Vector3.Lerp(from, to, eased);
                pos.y += Mathf.Sin(k * Mathf.PI) * arcHeight;
                t.position = pos;

                t.rotation = Quaternion.Euler(0f, 0f, k * 1080f) *
                             Quaternion.LookRotation((to - from).normalized, Vector3.up);
                await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
            }

            if (ghost != null) Object.Destroy(ghost);
            if (mat != null) Object.Destroy(mat);
        }
    }
}
