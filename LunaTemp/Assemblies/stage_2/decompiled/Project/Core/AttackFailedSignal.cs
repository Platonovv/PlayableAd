using System.Runtime.CompilerServices;
using Project.Domain;

namespace Project.Core
{
	
	public struct AttackFailedSignal
	{
		public readonly UnitId AttackerId;

		public readonly UnitId TargetId;

		public AttackFailedSignal(UnitId a, UnitId t)
		{
			AttackerId = a;
			TargetId = t;
		}
	}
}
