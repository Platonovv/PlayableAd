using System;

namespace Project.Domain
{
    public readonly struct UnitId : IEquatable<UnitId>
    {
        private static int _counter;

        public int Value { get; }

        private UnitId(int value) => Value = value;

        // Single-threaded в playable, не нужен Interlocked.
        public static UnitId Next() => new UnitId(++_counter);

        public bool Equals(UnitId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is UnitId other && Equals(other);
        public override int GetHashCode() => Value;
        public override string ToString() => $"#{Value}";

        public static bool operator ==(UnitId a, UnitId b) => a.Value == b.Value;
        public static bool operator !=(UnitId a, UnitId b) => a.Value != b.Value;
    }
}
