using Project.Core;
using UnityEngine;

namespace Project.Integration
{
    /// <summary>
    /// Аналитика: слушает ключевые сигналы и пишет события в <c>console.log</c>.
    /// </summary>
    public sealed class AnalyticsService : MonoBehaviour
    {
        [SerializeField] private GameRoot _root;

        private SignalBus _signals;

        private void Start()
        {
            if (_root == null) return;
            _signals = _root.Signals;
            _signals.Subscribe<PlayableStartedSignal>(OnStart);
            _signals.Subscribe<TargetSelectedSignal>(OnTarget);
            _signals.Subscribe<AttackResolvedSignal>(OnAttack);
            _signals.Subscribe<AttackFailedSignal>(OnFail);
            _signals.Subscribe<ChestCollectedSignal>(OnChest);
            _signals.Subscribe<BattleWonSignal>(OnWon);
            _signals.Subscribe<CtaClickedSignal>(OnCta);
        }

        private void OnDestroy()
        {
            if (_signals == null) return;
            _signals.Unsubscribe<PlayableStartedSignal>(OnStart);
            _signals.Unsubscribe<TargetSelectedSignal>(OnTarget);
            _signals.Unsubscribe<AttackResolvedSignal>(OnAttack);
            _signals.Unsubscribe<AttackFailedSignal>(OnFail);
            _signals.Unsubscribe<ChestCollectedSignal>(OnChest);
            _signals.Unsubscribe<BattleWonSignal>(OnWon);
            _signals.Unsubscribe<CtaClickedSignal>(OnCta);
        }

        private void Log(string ev) => Debug.Log("[analytics] " + ev);

        private void OnStart(PlayableStartedSignal _)  => Log("start");
        private void OnTarget(TargetSelectedSignal s)  => Log("interaction:" + s.TargetId.Value);
        private void OnAttack(AttackResolvedSignal s)  => Log("attack_ok:" + s.TargetId.Value);
        private void OnFail(AttackFailedSignal s)      => Log("attack_fail:" + s.TargetId.Value);
        private void OnChest(ChestCollectedSignal s)   => Log("chest:" + s.PowerGain);
        private void OnWon(BattleWonSignal _)          => Log("won");
        private void OnCta(CtaClickedSignal _)         => Log("cta_click");
    }
}
