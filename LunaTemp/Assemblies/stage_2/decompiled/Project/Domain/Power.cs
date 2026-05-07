using System;
using System.Runtime.CompilerServices;

namespace Project.Domain
{
	
	public struct Power : IEquatable<Power>, IComparable<Power>
	{
		public static readonly Power Zero = new Power(0);

		public int Value { get; }

		public Power(int value)
		{
			Value = ((value >= 0) ? value : 0);
		}

		public static Power operator +(Power a, Power b)
		{
			return new Power(a.Value + b.Value);
		}

		public static bool operator >=(Power a, Power b)
		{
			return a.Value >= b.Value;
		}

		public static bool operator <=(Power a, Power b)
		{
			return a.Value <= b.Value;
		}

		public static bool operator >(Power a, Power b)
		{
			return a.Value > b.Value;
		}

		public static bool operator <(Power a, Power b)
		{
			return a.Value < b.Value;
		}

		public static bool operator ==(Power a, Power b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(Power a, Power b)
		{
			return a.Value != b.Value;
		}

		public bool Equals(Power other)
		{
			return Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			return obj is Power other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Value;
		}

		public int CompareTo(Power other)
		{
			return Value.CompareTo(other.Value);
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
