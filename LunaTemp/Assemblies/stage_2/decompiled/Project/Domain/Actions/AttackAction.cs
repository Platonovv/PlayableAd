namespace Project.Domain.Actions
{
	public sealed class AttackAction : IBattleAction
	{
		public UnitId TargetId { get; }

		public AttackAction(UnitId targetId)
		{
			TargetId = targetId;
		}
	}
}
