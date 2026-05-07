using System.Runtime.CompilerServices;
using Project.Domain;

namespace Project.Core
{
	
	public struct ChestCollectedSignal
	{
		public readonly UnitId ChestId;

		public readonly int PowerGain;

		public ChestCollectedSignal(UnitId id, int gain)
		{
			ChestId = id;
			PowerGain = gain;
		}
	}
}
