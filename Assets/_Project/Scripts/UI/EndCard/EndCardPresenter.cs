using Project.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.UI.EndCard
{
    /// <summary>
    /// EndCard-presenter: Win — заголовок + CTA + Retry; Lose — только заголовок + Retry.
    /// </summary>
    public sealed class EndCardPresenter : MonoBehaviour
    {
        [SerializeField] private GameRoot _root;
        [SerializeField] private EndCardView _view;
        [SerializeField] private string _winTitle  = "YOU WIN!";
        [SerializeField] private string _loseTitle = "GAME OVER";

        private SignalBus _signals;

        private void Start()
        {
            Debug.Log($"[endcard] Presenter.Start root={(_root != null)} view={(_view != null)}");
            if (_root == null) { Debug.LogError("[endcard] _root is NULL — set GameRoot in inspector!"); return; }
            if (_view == null) { Debug.LogError("[endcard] _view is NULL — set EndCardView in inspector!"); return; }
            _signals = _root.Signals;
            _signals.Subscribe<BattleWonSignal>(OnWon);
            _signals.Subscribe<BattleLostSignal>(OnLost);
            _view.CtaClicked += OnCta;
            _view.RetryClicked += OnRetry;
            Debug.Log("[endcard] Presenter subscribed");
        }

        private void OnDestroy()
        {
            if (_signals != null)
            {
                _signals.Unsubscribe<BattleWonSignal>(OnWon);
                _signals.Unsubscribe<BattleLostSignal>(OnLost);
            }
            if (_view != null)
            {
                _view.CtaClicked -= OnCta;
                _view.RetryClicked -= OnRetry;
            }
        }

        private void OnWon(BattleWonSignal signal)
        {
            var delay = _root.Balance.EndCardDelay;
            Debug.Log($"[endcard] OnWon fired, delay={delay}");
            _view.Show(_winTitle, delay, showCta: true);
        }

        private void OnLost(BattleLostSignal signal)
        {
            var delay = _root.Balance.EndCardDelay;
            Debug.Log($"[endcard] OnLost fired, delay={delay}");
            _view.Show(_loseTitle, delay, showCta: false);
        }

        private void OnCta() => _signals.Fire(new CtaClickedSignal());

        private void OnRetry() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
