using System;
using System.Collections;

namespace Assets.Sun.Base
{
	public sealed class SchedulerTaskDefault : IScheduler
	{
		public bool IsInProgress => false;

		public IScheduler PassThrough => throw new NotSupportedException("nothing to pass through by default");

		public void Schedule(Func<IEnumerator> taskFactory)
		{
			var name = taskFactory?.Method.Name;
			$"pass task '{name}' by 'default' scheduler with no context".Log();
		}

		public void Schedule<T>(T parameter, Func<T, IEnumerator> taskFactory)
		{
			var name = taskFactory?.Method.Name;
			$"pass task '{name}' by 'default' scheduler for context '{parameter}'".Log();
		}

		public IEnumerator Wait()
		{
			throw new NotSupportedException("nothing to wait by default");
		}

		public void Clear() { }
	}
}
