using Project.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.UI.EndCard
{
	public sealed class EndCardPresenter : MonoBehaviour
	{
		[SerializeField]
		private GameRoot _root;

		[SerializeField]
		private EndCardView _view;

		[SerializeField]
		private string _winTitle = "YOU WIN!";

		[SerializeField]
		private string _loseTitle = "GAME OVER";

		[SerializeField]
		private string _winSubtitleFormat = "DEFEATED {0} · POWER {1}";

		[SerializeField]
		private string _loseSubtitleFormat = "DEFEATED {0} · POWER {1}";

		private SignalBus _signals;

		private int _kills;

		private int _fails;

		private int _lastPower;

		private void Start()
		{
			if (!(_root == null) && !(_view == null))
			{
				_signals = _root.Signals;
				_signals.Subscribe<BattleWonSignal>(OnWon);
				_signals.Subscribe<BattleLostSignal>(OnLost);
				_signals.Subscribe<AttackResolvedSignal>(OnKill);
				_signals.Subscribe<AttackFailedSignal>(OnFail);
				_signals.Subscribe<PlayerPowerChangedSignal>(OnPowerChanged);
				_view.CtaClicked += OnCta;
				_view.RetryClicked += OnRetry;
				if (_root.Level != null)
				{
					_lastPower = _root.Level.Player.Power;
				}
			}
		}

		private void OnDestroy()
		{
			if (_signals != null)
			{
				_signals.Unsubscribe<BattleWonSignal>(OnWon);
				_signals.Unsubscribe<BattleLostSignal>(OnLost);
				_signals.Unsubscribe<AttackResolvedSignal>(OnKill);
				_signals.Unsubscribe<AttackFailedSignal>(OnFail);
				_signals.Unsubscribe<PlayerPowerChangedSignal>(OnPowerChanged);
			}
			if (_view != null)
			{
				_view.CtaClicked -= OnCta;
				_view.RetryClicked -= OnRetry;
			}
		}

		private void OnKill(AttackResolvedSignal _)
		{
			_kills++;
		}

		private void OnFail(AttackFailedSignal _)
		{
			_fails++;
		}

		private void OnPowerChanged(PlayerPowerChangedSignal s)
		{
			_lastPower = s.NewValue;
		}

		private void OnWon(BattleWonSignal signal)
		{
			_view.Show(_winTitle, FormatSubtitle(_winSubtitleFormat), _root.Balance.EndCardDelay, true, ComputeWinStars());
		}

		private void OnLost(BattleLostSignal signal)
		{
			_view.Show(_loseTitle, FormatSubtitle(_loseSubtitleFormat), _root.Balance.EndCardDelay, false);
		}

		private string FormatSubtitle(string fmt)
		{
			return string.Format(fmt, _kills, _lastPower);
		}

		private int ComputeWinStars()
		{
			if (_fails == 0)
			{
				return 3;
			}
			if (_fails <= 2)
			{
				return 2;
			}
			return 1;
		}

		private void OnCta()
		{
			_signals.Fire(default(CtaClickedSignal));
		}

		private void OnRetry()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
