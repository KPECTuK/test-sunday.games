using System;
using System.Collections;

namespace Assets.Sun.Base
{
	public sealed class SchedulerTaskAll : SchedulerTaskBase, IScheduler
	{
		//! avoid loops
		public IScheduler PassThrough => this;

		public void Schedule(Func<IEnumerator> taskFactory)
		{
			var name = taskFactory.Method.Name;
			$"schedule task '{name}' by scheduler '{GetType().NameNice()}'".Log();

			QueueTasks.Enqueue(taskFactory.Invoke());
		}

		public void Schedule<T>(T parameter, Func<T, IEnumerator> taskFactory)
		{
			var name = taskFactory.Method.Name;
			$"schedule task '{name}' by scheduler '{GetType().NameNice()}' for context '{parameter}'".Log();

			QueueTasks.Enqueue(taskFactory.Invoke(parameter));
		}
	}
}
