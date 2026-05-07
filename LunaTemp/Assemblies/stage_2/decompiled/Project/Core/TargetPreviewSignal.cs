using System.Runtime.CompilerServices;
using Project.Domain;

namespace Project.Core
{
	
	public struct TargetPreviewSignal
	{
		public readonly UnitId TargetId;

		public readonly bool HasTarget;

		public static TargetPreviewSignal None => default(TargetPreviewSignal);

		public TargetPreviewSignal(UnitId id)
		{
			TargetId = id;
			HasTarget = true;
		}
	}
}
