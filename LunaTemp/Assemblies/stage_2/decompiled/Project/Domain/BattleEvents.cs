using System;

namespace Project.Domain
{
	public sealed class BattleEvents
	{
		public event Action<Unit, Unit> AttackResolved;

		public event Action<Unit, Unit> AttackFailed;

		public event Action<Unit, Unit> ChestCollected;

		public event Action<Unit> PlayerPowerChanged;

		public event Action BattleWon;

		public event Action BattleLost;

		internal void RaiseAttackResolved(Unit attacker, Unit target)
		{
			this.AttackResolved?.Invoke(attacker, target);
		}

		internal void RaiseAttackFailed(Unit attacker, Unit target)
		{
			this.AttackFailed?.Invoke(attacker, target);
		}

		internal void RaiseChestCollected(Unit player, Unit chest)
		{
			this.ChestCollected?.Invoke(player, chest);
		}

		internal void RaisePlayerPowerChanged(Unit player)
		{
			this.PlayerPowerChanged?.Invoke(player);
		}

		internal void RaiseBattleWon()
		{
			this.BattleWon?.Invoke();
		}

		internal void RaiseBattleLost()
		{
			this.BattleLost?.Invoke();
		}
	}
}
