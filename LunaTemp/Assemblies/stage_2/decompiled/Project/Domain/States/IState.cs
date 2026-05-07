namespace Project.Domain.States
{
	public interface IState<TContext>
	{
		void Enter(TContext context);

		void Exit(TContext context);
	}
}
