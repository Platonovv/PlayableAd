using Project.Core;
using Project.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Common
{
	/// <summary>
	/// Счётчик оставшихся врагов: «MOBS 2/3». Слушает <see cref="AttackResolvedSignal"/>
	/// и при каждом успешном убийстве декрементирует. Скрывается на конце боя.
	/// </summary>
	public sealed class MobCounter : MonoBehaviour
	{
		[SerializeField] private GameRoot _root;
		[SerializeField] private Text _label;
		[SerializeField] private CanvasGroup _group;
		[SerializeField] private string _format = "MOBS {0}/{1}";

		private SignalBus _signals;
		private int _total;
		private int _killed;

		private void Start()
		{
			if (_root == null)
				return;

			_signals = _root.Signals;

			_total = CountEnemies(_root.Level);
			_killed = 0;
			UpdateLabel();
			SetVisible(_total > 0);

			_signals.Subscribe<AttackResolvedSignal>(OnAttackResolved);
			_signals.Subscribe<BattleWonSignal>(OnBattleEnded);
			_signals.Subscribe<BattleLostSignal>(OnBattleEnded);
		}

		private void OnDestroy()
		{
			if (_signals == null)
				return;

			_signals.Unsubscribe<AttackResolvedSignal>(OnAttackResolved);
			_signals.Unsubscribe<BattleWonSignal>(OnBattleEnded);
			_signals.Unsubscribe<BattleLostSignal>(OnBattleEnded);
		}

		private void OnAttackResolved(AttackResolvedSignal _)
		{
			_killed++;
			if (_killed > _total)
				_killed = _total;
			UpdateLabel();
		}

		private void OnBattleEnded<T>(T _) => SetVisible(false);

		private void UpdateLabel()
		{
			if (_label != null)
				_label.text = string.Format(_format, _total - _killed, _total);
		}

		private void SetVisible(bool visible)
		{
			if (_group != null)
				_group.alpha = visible ? 1f : 0f;
		}

		private static int CountEnemies(Project.Configs.LevelConfig level)
		{
			if (level == null || level.Targets == null)
				return 0;

			int n = 0;
			for (int i = 0; i < level.Targets.Count; i++)
				if (level.Targets[i].Kind == UnitKind.Enemy)
					n++;
			return n;
		}
	}
}