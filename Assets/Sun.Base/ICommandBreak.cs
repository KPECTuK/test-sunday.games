namespace Assets.Sun.Base
{
	/// <summary>
	/// brakes execution queue after that
	/// </summary>
	public interface ICommandBreak<in T> : ICommand<T> { }
}
