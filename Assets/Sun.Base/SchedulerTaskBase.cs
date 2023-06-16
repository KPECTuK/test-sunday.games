using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Sun.Base
{
	/// <summary> maintains execution queue </summary>
	public abstract class SchedulerTaskBase
	{
		public Queue<IEnumerator> QueueTasks;

		protected IScheduler Fallback;

		//? not all of them can be canceled
		public bool IsCanceled;

		public bool IsInProgress => IsCanceled || QueueTasks is { Count: > 0 };

		public void Clear()
		{
			QueueTasks?.Clear();
		}

		public IScheduler SetFallBack<T>() where T : IScheduler, new()
		{
			Fallback = QueueTasks.BuildScheduler<T>();
			return Fallback;
		}

		public IEnumerator Wait()
		{
			return new WaitWhile(() => IsInProgress);
		}
	}
}
