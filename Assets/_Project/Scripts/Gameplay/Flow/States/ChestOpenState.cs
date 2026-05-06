using System.Collections;
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
        private Coroutine _co;
        private Coroutine _landedCo;
        private Coroutine _swordCo;

        public ChestOpenState(BattleFlow flow) => _flow = flow;

        public void Enter(BattleFlowContext ctx)
        {
            _co = _flow.StartCoroutine(Run(ctx));
        }

        public void Exit(BattleFlowContext ctx)
        {
            if (_co != null)      { _flow.StopCoroutine(_co); _co = null; }
            if (_landedCo != null) { _flow.StopCoroutine(_landedCo); _landedCo = null; }
            if (_swordCo != null)  { _flow.StopCoroutine(_swordCo); _swordCo = null; }
        }

        private IEnumerator Run(BattleFlowContext ctx)
        {
            if (!ctx.Views.TryGetValue(ctx.PendingTarget, out var view) || !(view is ChestView chest))
            {
                _flow.GoIdle();
                yield break;
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
                    ctx.Balance.FloatingNumberDuration, ctx.Numbers);
            }

            ctx.Player.PlayIdle();
            yield return chest.PlayPreOpen();

            ctx.Vfx.Play("chest_open", chest.Vfx.position);
            _swordCo = _flow.StartCoroutine(FlySwordFromChest(chest, ctx.Player));
            _landedCo = _flow.StartCoroutine(NumberLanded(ctx));

            yield return chest.PlayOpen();
            while (_swordCo != null) yield return null;

            ctx.Vfx.Play("upgrade", ctx.Player.Vfx.position);
            yield return ctx.Player.PlayUpgrade();
            ctx.Player.PlayIdle();

            _co = null;
            _flow.GoIdle();
        }

        private IEnumerator NumberLanded(BattleFlowContext ctx)
        {
            yield return new WaitForSeconds(ctx.Balance.FloatingNumberDuration);
            ctx.Player.RefreshPower();
            if (ctx.Player.PowerLabel != null) ctx.Player.PowerLabel.Pop();
            ctx.Vfx.Play("power_gain", ctx.Player.Vfx.position);
            ctx.Signals.Fire(new PlayerPowerChangedSignal(ctx.Battle.Player.Power.Value));
            ctx.Player.PlayPowerGain();
            _landedCo = null;
        }

        private IEnumerator FlySwordFromChest(ChestView chest, PlayerView player)
        {
            var sword = chest.FlyingSword;
            if (sword == null) { player.RevealSword(); _swordCo = null; yield break; }

            var from = chest.Vfx.position;
            var to = player.SwordTarget;
            const float duration = 0.6f;
            const float arcHeight = 2.5f;
            var direction = (to - from).sqrMagnitude > 0.0001f
                ? (to - from).normalized
                : Vector3.forward;

            sword.SetParent(null, true);
            sword.position = from;
            sword.gameObject.SetActive(true);

            var sideAxis = Vector3.Cross(Vector3.up, direction);
            const float wobbleAmplitude = 0.15f;
            var elapsed = 0f;
            while (elapsed < duration && sword != null)
            {
                elapsed += Time.deltaTime;
                var k = Mathf.Clamp01(elapsed / duration);
                var eased = Ease.InOutQuad(k);
                var pos = Vector3.Lerp(from, to, eased);
                pos.y += Mathf.Sin(k * Mathf.PI) * arcHeight;
                var apex = Mathf.Sin(k * Mathf.PI);
                pos += sideAxis * (Mathf.Sin(k * Mathf.PI * 5f) * wobbleAmplitude * apex);
                sword.position = pos;
                sword.rotation = Quaternion.Euler(0f, 0f, k * 1080f) *
                                 Quaternion.LookRotation(direction, Vector3.up);
                yield return null;
            }

            if (sword != null) sword.gameObject.SetActive(false);
            player.RevealSword();
            _swordCo = null;
        }
    }
}
