using System.Runtime.CompilerServices;

namespace Project.Core
{
	
	public struct PlayerPowerChangedSignal
	{
		public readonly int NewValue;

		public PlayerPowerChangedSignal(int value)
		{
			NewValue = value;
		}
	}
}
