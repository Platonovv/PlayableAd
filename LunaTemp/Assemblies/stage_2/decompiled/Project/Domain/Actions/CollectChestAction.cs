namespace Project.Domain.Actions
{
	public sealed class CollectChestAction : IBattleAction
	{
		public UnitId TargetId { get; }

		public CollectChestAction(UnitId targetId)
		{
			TargetId = targetId;
		}
	}
}
