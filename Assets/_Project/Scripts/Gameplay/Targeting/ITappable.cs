using Project.Domain;

namespace Project.Gameplay.Targeting
{
    /// <summary>
    /// Контракт объекта, по которому можно тапнуть; держит ссылку на Domain-юнит.
    /// </summary>
    public interface ITappable
    {
        UnitId Id { get; }
        UnitKind Kind { get; }
        UnityEngine.Transform Anchor { get; }
        void SetHighlighted(bool value);
    }
}
