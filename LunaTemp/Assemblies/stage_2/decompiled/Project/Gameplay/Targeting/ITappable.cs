using Project.Domain;
using UnityEngine;

namespace Project.Gameplay.Targeting
{
	public interface ITappable
	{
		UnitId Id { get; }

		UnitKind Kind { get; }

		Transform Anchor { get; }

		void SetHighlighted(bool value);
	}
}
