using System;
using System.Collections;
using Assets.Sun.Base;
using UnityEngine;

namespace Assets.Sun.Task01
{
	public sealed class SchedulerViewContentScroll : SchedulerTaskBase, IScheduler
	{
		// to allow pic tap
		private double SCROLL_BRAKE_INTEVAL_D = .5d;

		private DateTime _lastScrollEvent;

		public bool IsScrolling => DateTime.UtcNow - _lastScrollEvent < TimeSpan.FromSeconds(SCROLL_BRAKE_INTEVAL_D); 

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
					_lastScrollEvent = DateTime.UtcNow;
					QueueTasks.Enqueue(taskFactory.Invoke(parameter));
				}
			}
		}
	}
}
