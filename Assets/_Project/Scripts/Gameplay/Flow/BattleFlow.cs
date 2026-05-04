using System.Collections.Generic;
using Project.Configs;
using Project.Core;
using Project.Domain;
using Project.Domain.States;
using Project.Gameplay.CameraFx;
using Project.Gameplay.Flow.States;
using Project.Gameplay.Targeting;
using Project.Gameplay.Units;
using Project.Gameplay.Vfx;
using UnityEngine;

namespace Project.Gameplay.Flow
{
	/// <summary>
	/// Оркестратор боя: связывает Domain-модель, view-слой, инпут и FSM.
	/// </summary>
	public sealed class BattleFlow : MonoBehaviour
	{
		[SerializeField] private TapInput _input;
		[SerializeField] private TargetIndicator _indicator;
		[SerializeField] private VfxService _vfx;
		[SerializeField] private ScreenShake _shake;
		[SerializeField] private FloatingNumber _floatingNumberPrefab;
		[SerializeField] private Transform _floatingNumbersRoot;

		private BattleFlowContext _ctx;
		private StateMachine<BattleFlowContext> _fsm;
		private IdleState _idle;
		private MovingToTargetState _moving;
		private AttackState _attack;
		private ChestOpenState _chest;
		private WonState _won;
		private LostState _lost;
		private UnitView _highlighted;
		private UnitView _previewing;

		public Battle Battle { get; private set; }

		public void Init(BalanceConfig balance,
		                 SignalBus signals,
		                 PlayerView player,
		                 Battle battle,
		                 Dictionary<UnitId, UnitView> views)
		{
			Battle = battle;

			Pool<FloatingNumber> pool = null;
			if (_floatingNumberPrefab != null && _floatingNumbersRoot != null)
				pool = new Pool<FloatingNumber>(_floatingNumberPrefab, _floatingNumbersRoot, prewarm: 4);

			_ctx = new BattleFlowContext
			{
				Balance = balance,
				Signals = signals,
				Player = player,
				Battle = battle,
				Views = views,
				Indicator = _indicator,
				Input = _input,
				Vfx = _vfx,
				Shake = _shake,
				Numbers = pool
			};

			_fsm = new StateMachine<BattleFlowContext>(_ctx);
			_idle = new IdleState();
			_moving = new MovingToTargetState(this);
			_attack = new AttackState(this);
			_chest = new ChestOpenState(this);
			_won = new WonState();
			_lost = new LostState();

			_input.Init(signals);
			signals.Subscribe<TargetSelectedSignal>(OnTargetSelected);
			signals.Subscribe<TargetPreviewSignal>(OnTargetPreview);

			// PowerLabel над головой игрока обновляем вручную в стейтах после того, как
			// летящее +N "долетит" до игрока — иначе число у героя меняется раньше эффекта.

			_fsm.Set(_idle);
		}

		private void OnDestroy()
		{
			if (_ctx?.Signals == null) return;
			_ctx.Signals.Unsubscribe<TargetSelectedSignal>(OnTargetSelected);
			_ctx.Signals.Unsubscribe<TargetPreviewSignal>(OnTargetPreview);
		}

		private void OnTargetPreview(TargetPreviewSignal signal)
		{
			if (_fsm.Current != _idle) return;

			if (!signal.HasTarget)
			{
				EndPreview();
				return;
			}

			if (!_ctx.Views.TryGetValue(signal.TargetId, out var view) || !view.Unit.IsAlive)
			{
				EndPreview();
				return;
			}
			if (view.Kind == Project.Domain.UnitKind.Player) return;

			if (_previewing == view) return;

			if (_previewing != null) _previewing.SetPreview(false);
			_previewing = view;

			// Если враг сильнее — показываем «warning» preview (красное кольцо).
			var isWarning = view.Kind == Project.Domain.UnitKind.Enemy
			                && _ctx.Battle.Player.Power < view.Unit.Power;
			_previewing.SetPreview(true, isWarning);

			// Обновляем _highlighted для будущей очистки при переходе в Idle/Won.
			// SetHighlighted НЕ зовём — кольца уже управляются SetPreview.
			_highlighted = view;
			_ctx.Indicator.Show(_ctx.Player.transform, view.Stop);
		}

		private void EndPreview()
		{
			if (_previewing != null) _previewing.SetPreview(false);
			_previewing = null;
			ClearHighlight();
			_ctx.Indicator.Hide();
		}

		private void OnTargetSelected(TargetSelectedSignal signal)
		{
			if (_fsm.Current != _idle)
				return;
			if (!_ctx.Views.TryGetValue(signal.TargetId, out var view) || !view.Unit.IsAlive)
				return;

			// Тап по самому игроку — игнор: ходить к самому себе бессмысленно.
			if (view.Kind == Project.Domain.UnitKind.Player)
				return;

			// Коммит: пользователь действительно хочет атаковать или открыть сундук.
			// Если враг сильнее — игрок всё равно идёт, получает удар и проигрывает (см. AttackState).
			if (_previewing != null) _previewing.SetPreview(false);
			_previewing = null;
			Highlight(view);
			_ctx.PendingTarget = signal.TargetId;
			_fsm.Set(_moving);
		}

		private void Highlight(UnitView view)
		{
			if (_highlighted != null && _highlighted != view)
				_highlighted.SetHighlighted(false);
			_highlighted = view;
			if (_highlighted != null)
				_highlighted.SetHighlighted(true);
		}

		private void ClearHighlight()
		{
			if (_highlighted != null)
				_highlighted.SetHighlighted(false);
			_highlighted = null;
		}

		internal void GoIdle()
		{
			ClearHighlight();
			_fsm.Set(_idle);
		}

		internal void GoAttack() => _fsm.Set(_attack);
		internal void GoChestOpen() => _fsm.Set(_chest);

		internal void GoWon()
		{
			ClearHighlight();
			_fsm.Set(_won);
		}

		internal void GoLost()
		{
			ClearHighlight();
			_fsm.Set(_lost);
		}
	}
}