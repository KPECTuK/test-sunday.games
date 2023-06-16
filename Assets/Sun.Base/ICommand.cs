namespace Assets.Sun.Base
{
	public interface ICommand<in T>
	{
		bool Assert(T context);
		void Execute(T context);
	}
}
