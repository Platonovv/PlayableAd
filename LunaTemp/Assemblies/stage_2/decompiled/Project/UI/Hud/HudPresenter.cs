using Project.Core;
using UnityEngine;

namespace Project.UI.Hud
{
	public sealed class HudPresenter : MonoBehaviour
	{
		[SerializeField]
		private GameRoot _root;

		[SerializeField]
		private HudView _view;

		private SignalBus _signals;

		private void Start()
		{
			if (!(_root == null))
			{
				_signals = _root.Signals;
				_signals.Subscribe<PlayerPowerChangedSignal>(OnPower);
				if (_view != null)
				{
					_view.SetPower(_root.Level.Player.Power);
				}
			}
		}

		private void OnDestroy()
		{
			if (_signals != null)
			{
				_signals.Unsubscribe<PlayerPowerChangedSignal>(OnPower);
			}
		}

		private void OnPower(PlayerPowerChangedSignal s)
		{
			if (_view != null)
			{
				_view.SetPower(s.NewValue);
			}
		}
	}
}
