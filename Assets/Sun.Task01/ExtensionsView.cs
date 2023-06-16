using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Sun.Base;

namespace Assets.Sun.Task01
{
	public static class ExtensionsView
	{
		private readonly struct EnumerableGeneric<T> : IEnumerable<T>
		{
			private readonly IEnumerator<T> _enumerator;

			public EnumerableGeneric(IEnumerator<T> enumerator)
			{
				_enumerator = enumerator;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return _enumerator;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public static bool TryExecuteCommandQueue<T>(this Queue<ICommand<T>> source, T context)
		{
			var notionContext = context?.GetType().NameNice() ?? "undefined";

			// prevent execution of next command batch
			var size = source.Count;
			while(size > 0 && source.TryPeek(out var @event))
			{
				//size--;
				try
				{
					var notionEvent = @event?.GetType().NameNice() ?? "undefined";

					if(@event == null)
					{
						"skip command due conditions: command is NULL".LogError();
					}
					else
					{
						if(@event.Assert(context))
						{
							$"run command <color=white>{notionEvent}</color> in context <color=white>{notionContext}</color>".Log();

							@event.Execute(context);
						}
						else
						{
							$"skip command due conditions <color=white>{notionEvent}</color> in context <color=white>{notionContext}</color>".LogWarning();
						}
					}
				}
				catch(Exception exception)
				{
					exception.ToText().LogError();
				}

				source.Dequeue();

				// stop queue to wait screen transition for example (batching for particular screen)
				if(@event is ICommandBreak<CompScreens>)
				{
					$"stop command queue in context: <color=white>{notionContext}</color>".Log();

					break;
				}
			}

			return source.Count > 0;
		}

		public static bool ExecuteTasksSimultaneously(this Queue<IEnumerator> tasks)
		{
			var size = tasks.Count;
			for(var index = 0; index < size; index++)
			{
				var task = tasks.Dequeue();
				if(task.MoveNext())
				{
					tasks.Enqueue(task);
				}
			}

			return tasks.Count != 0;
		}

		public static bool ExecuteTasksSequentially(this Queue<IEnumerator> tasks)
		{
			var size = tasks.Count;
			for(var index = 0; index < size; index++)
			{
				var task = tasks.Dequeue();
				if(task.MoveNext())
				{
					tasks.Enqueue(task);
					break;
				}
			}

			return tasks.Count != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsIndexValid(this int index, int size)
		{
			return index > -1 && index < size;
		}

		public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> source)
		{
			return new EnumerableGeneric<T>(source);
		}
	}
}
