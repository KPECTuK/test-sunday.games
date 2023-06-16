namespace Assets.Sun.Base
{
	//? collect all the branch to enable\disable
	public interface IWidgetController
	{
		void OnWidgetEnable();
		void OnWidgetDisable();

		IScheduler Scheduler { get; }
		void SetScheduler<T>() where T : IScheduler, new();
	}
}
