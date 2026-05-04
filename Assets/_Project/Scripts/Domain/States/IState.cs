namespace Project.Domain.States
{
    /// <summary>
    /// Состояние FSM: Enter/Exit с контекстом-владельцем.
    /// </summary>
    public interface IState<TContext>
    {
        void Enter(TContext context);
        void Exit(TContext context);
    }
}
