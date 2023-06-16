namespace Assets.Sun.Base
{
	public interface IWidgetController
	{
		void OnWidgetEnable();
		void OnWidgetDisable();

		IScheduler Scheduler { get; }
		void SetScheduler<T>() where T : IScheduler, new();
	}
}
