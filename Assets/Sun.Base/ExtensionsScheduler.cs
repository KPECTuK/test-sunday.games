using System.Collections;
using System.Collections.Generic;

namespace Assets.Sun.Base
{
	public static class ExtensionsScheduler
	{
		public static IScheduler BuildScheduler<T>(this Queue<IEnumerator> tasks) where T : IScheduler, new()
		{
			var result = new T();
			if(result is SchedulerTaskBase cast)
			{
				cast.QueueTasks = tasks;
			}

			return result;
		}
	}
}
