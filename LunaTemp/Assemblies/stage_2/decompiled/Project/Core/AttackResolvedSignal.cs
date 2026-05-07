using System.Runtime.CompilerServices;
using Project.Domain;

namespace Project.Core
{
	
	public struct AttackResolvedSignal
	{
		public readonly UnitId AttackerId;

		public readonly UnitId TargetId;

		public AttackResolvedSignal(UnitId a, UnitId t)
		{
			AttackerId = a;
			TargetId = t;
		}
	}
}
