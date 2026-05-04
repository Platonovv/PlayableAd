using System;

namespace Project.Domain
{
	public readonly struct Power : IEquatable<Power>, IComparable<Power>
	{
		public static readonly Power Zero = new(0);

		public int Value { get; }

		public Power(int value) => Value = value < 0 ? 0 : value;

		public static Power operator +(Power a, Power b) => new(a.Value + b.Value);
		public static bool operator >=(Power a, Power b) => a.Value >= b.Value;
		public static bool operator <=(Power a, Power b) => a.Value <= b.Value;
		public static bool operator >(Power a, Power b) => a.Value > b.Value;
		public static bool operator <(Power a, Power b) => a.Value < b.Value;
		public static bool operator ==(Power a, Power b) => a.Value == b.Value;
		public static bool operator !=(Power a, Power b) => a.Value != b.Value;

		public bool Equals(Power other) => Value == other.Value;
		public override bool Equals(object obj) => obj is Power other && Equals(other);
		public override int GetHashCode() => Value;
		public int CompareTo(Power other) => Value.CompareTo(other.Value);
		public override string ToString() => Value.ToString();
	}
}