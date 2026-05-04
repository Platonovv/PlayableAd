using System;

namespace Project.Domain.States
{
    /// <summary>
    /// Generic FSM: одно активное состояние, без аллокаций при заранее созданных стейтах.
    /// </summary>
    public sealed class StateMachine<TContext>
    {
        private readonly TContext _context;

        public IState<TContext> Current { get; private set; }
        public event Action<IState<TContext>, IState<TContext>> Changed;

        public StateMachine(TContext context) => _context = context;

        public void Set(IState<TContext> next)
        {
            if (next == null || next == Current) return;
            var prev = Current;
            prev?.Exit(_context);
            Current = next;
            next.Enter(_context);
            Changed?.Invoke(prev, next);
        }
    }
}
