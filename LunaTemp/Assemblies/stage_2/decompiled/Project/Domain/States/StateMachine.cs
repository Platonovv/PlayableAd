using System;

namespace Project.Domain.States
{
	public sealed class StateMachine<TContext>
	{
		private readonly TContext _context;

		public IState<TContext> Current { get; private set; }

		public event Action<IState<TContext>, IState<TContext>> Changed;

		public StateMachine(TContext context)
		{
			_context = context;
		}

		public void Set(IState<TContext> next)
		{
			if (next != null && next != Current)
			{
				IState<TContext> prev = Current;
				prev?.Exit(_context);
				Current = next;
				next.Enter(_context);
				this.Changed?.Invoke(prev, next);
			}
		}
	}
}
