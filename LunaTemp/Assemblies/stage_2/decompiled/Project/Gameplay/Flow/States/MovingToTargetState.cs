using System.Collections;
using Project.Domain;
using Project.Domain.States;
using UnityEngine;

namespace Project.Gameplay.Flow.States
{
	public sealed class MovingToTargetState : IState<BattleFlowContext>
	{
		private readonly BattleFlow _flow;

		private Coroutine _co;

		public MovingToTargetState(BattleFlow flow)
		{
			_flow = flow;
		}

		public void Enter(BattleFlowContext ctx)
		{
			_co = _flow.StartCoroutine(Run(ctx));
		}

		public void Exit(BattleFlowContext ctx)
		{
			if (_co != null)
			{
				_flow.StopCoroutine(_co);
				_co = null;
			}
		}

		private IEnumerator Run(BattleFlowContext ctx)
		{
			if (!ctx.Views.TryGetValue(ctx.PendingTarget, out var view))
			{
				_flow.GoIdle();
				yield break;
			}
			ctx.Indicator.Show(ctx.Player.transform, view.Stop, ctx.ColorFor(view));
			yield return ctx.Player.MoveTo(view.Stop.position);
			ctx.Indicator.Hide();
			_co = null;
			switch (view.Kind)
			{
			case UnitKind.Chest:
				_flow.GoChestOpen();
				break;
			case UnitKind.Enemy:
				_flow.GoAttack();
				break;
			default:
				_flow.GoIdle();
				break;
			}
		}
	}
}
