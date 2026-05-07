using System;
using System.Runtime.CompilerServices;

namespace Project.Domain
{
	
	public struct UnitId : IEquatable<UnitId>
	{
		private static int _counter;

		public int Value { get; }

		private UnitId(int value)
		{
			Value = value;
		}

		public static UnitId Next()
		{
			return new UnitId(++_counter);
		}

		public bool Equals(UnitId other)
		{
			return Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			return obj is UnitId other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Value;
		}

		public override string ToString()
		{
			return $"#{Value}";
		}

		public static bool operator ==(UnitId a, UnitId b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(UnitId a, UnitId b)
		{
			return a.Value != b.Value;
		}
	}
}
