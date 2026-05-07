using System.Runtime.CompilerServices;
using Project.Domain;

namespace Project.Core
{
	
	public struct TargetSelectedSignal
	{
		public readonly UnitId TargetId;

		public TargetSelectedSignal(UnitId id)
		{
			TargetId = id;
		}
	}
}
