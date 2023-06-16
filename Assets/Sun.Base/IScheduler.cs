using System;
using System.Collections;

namespace Assets.Sun.Base
{
	/// <summary>
	/// it might be solved by the commands, but it aligned to view, and, more over,
	/// it could serve as any type of filter\provider, not the type only
	/// </summary>
	public interface IScheduler
	{
		bool IsInProgress { get; }

		IScheduler PassThrough { get; }

		void Schedule(Func<IEnumerator> taskFactory);

		/// <remarks> NOTE: first arg is a task context </remarks>
		void Schedule<T>(T parameter, Func<T, IEnumerator> taskFactory);

		IEnumerator Wait();

		void Clear();
	}
}
