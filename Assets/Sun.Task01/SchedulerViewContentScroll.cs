using System;
using System.Collections;
using Assets.Sun.Base;
using UnityEngine;

namespace Assets.Sun.Task01
{
	public sealed class SchedulerViewContentScroll : SchedulerTaskBase, IScheduler
	{
		public IScheduler PassThrough => QueueTasks.BuildScheduler<SchedulerTaskAll>();

		public void Schedule(Func<IEnumerator> taskFactory)
		{
			throw new NotSupportedException("TL; TI;");
		}

		public void Schedule<T>(T parameter, Func<T, IEnumerator> taskFactory)
		{
			// var name = taskFactory.Method.Name;
			// $"schedule task '{name}' by scheduler '{GetType().NameNice()}' for context '{parameter}'".Log();

			if(parameter is float cast)
			{
				if(!Mathf.Approximately(0f, cast))
				{
					QueueTasks.Enqueue(taskFactory.Invoke(parameter));
				}
			}
		}
	}
}
