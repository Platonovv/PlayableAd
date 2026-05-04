namespace Project.Domain
{
    /// <summary>
    /// Мутабельный юнит домена: id, тип, текущая сила, состояние жизни.
    /// </summary>
    public sealed class Unit
    {
        public UnitId Id { get; }
        public UnitKind Kind { get; }
        public Power Power { get; private set; }
        public bool IsAlive { get; private set; } = true;

        public Unit(UnitKind kind, Power power)
        {
            Id = UnitId.Next();
            Kind = kind;
            Power = power;
        }

        internal void AddPower(Power delta) => Power += delta;
        internal void Kill() => IsAlive = false;
    }
}
