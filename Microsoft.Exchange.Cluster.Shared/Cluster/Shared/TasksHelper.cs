using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Shared
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class TasksHelper
	{
		public static IEnumerable<TTask> GetSuccessful<TTask>(this IEnumerable<TTask> tasks) where TTask : Task
		{
			return from task in tasks
			where task.Status == TaskStatus.RanToCompletion
			select task;
		}

		public static IEnumerable<TTask> GetFailed<TTask>(this IEnumerable<TTask> tasks) where TTask : Task
		{
			return from task in tasks
			where task.IsFaulted
			select task;
		}

		public static IEnumerable<TTask> GetTimedOut<TTask>(this IEnumerable<TTask> tasks) where TTask : Task
		{
			return from task in tasks
			where !task.IsCanceled && !task.IsCompleted && !task.IsFaulted
			select task;
		}

		public static IEnumerable<string> GetServerNamesFromTasks<T>(this IEnumerable<Task<Tuple<string, T>>> tasks)
		{
			return from task in tasks
			select task.Result.Item1;
		}

		public static string GetServerNamesStringFromTasks<T>(this IEnumerable<Task<Tuple<string, T>>> tasks, Func<IEnumerable<Task<Tuple<string, T>>>, IEnumerable<Task<Tuple<string, T>>>> tasksSelector = null)
		{
			IEnumerable<Task<Tuple<string, T>>> tasks2 = tasks;
			if (tasksSelector != null)
			{
				tasks2 = tasksSelector(tasks);
			}
			return string.Join(", ", tasks2.GetServerNamesFromTasks<T>());
		}
	}
}
