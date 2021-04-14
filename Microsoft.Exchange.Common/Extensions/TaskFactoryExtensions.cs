using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Extensions
{
	internal static class TaskFactoryExtensions
	{
		internal static TaskScheduler GetTargetScheduler(this TaskFactory factory)
		{
			ArgumentValidator.ThrowIfNull("factory", factory);
			return factory.Scheduler ?? TaskScheduler.Current;
		}

		internal static Task Iterate<T>(this TaskFactory factory, IEnumerable<T> source) where T : Task
		{
			return factory.Iterate(source, null, factory.CancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
		}

		internal static Task Iterate<T>(this TaskFactory factory, IEnumerable<T> source, CancellationToken cancellationToken) where T : Task
		{
			return factory.Iterate(source, null, cancellationToken, factory.CreationOptions, factory.GetTargetScheduler());
		}

		internal static Task Iterate<T>(this TaskFactory factory, IEnumerable<T> source, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler) where T : Task
		{
			return factory.Iterate(source, null, cancellationToken, creationOptions, scheduler);
		}

		internal static Task Iterate<T>(this TaskFactory factory, IEnumerable<T> source, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler) where T : Task
		{
			ArgumentValidator.ThrowIfNull("factory", factory);
			ArgumentValidator.ThrowIfNull("source", source);
			ArgumentValidator.ThrowIfNull("scheduler", scheduler);
			ArgumentValidator.ThrowIfNull("cancellationToken", cancellationToken);
			IEnumerator<T> enumerator = source.GetEnumerator();
			if (enumerator == null)
			{
				throw new InvalidOperationException("Invalid enumerable - GetEnumerator returned null");
			}
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(state, creationOptions);
			tcs.Task.ContinueWith(delegate(Task<object> x)
			{
				enumerator.Dispose();
			}, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
			Action<Task> recursiveAsyncIterator = null;
			recursiveAsyncIterator = delegate(Task unusedAntecedentTask)
			{
				try
				{
					if (cancellationToken.IsCancellationRequested)
					{
						tcs.TrySetCanceled();
					}
					else if (enumerator.MoveNext())
					{
						T t = enumerator.Current;
						t.IgnoreExceptions();
						t.ContinueWith(recursiveAsyncIterator).IgnoreExceptions();
					}
					else
					{
						tcs.TrySetResult(null);
					}
				}
				catch (Exception ex)
				{
					OperationCanceledException ex2 = ex as OperationCanceledException;
					if (ex2 != null && ex2.CancellationToken == cancellationToken)
					{
						tcs.TrySetCanceled();
					}
					else
					{
						tcs.TrySetException(ex);
					}
				}
			};
			factory.StartNew(delegate()
			{
				recursiveAsyncIterator(null);
			}, CancellationToken.None, TaskCreationOptions.None, scheduler).IgnoreExceptions();
			return tcs.Task;
		}

		private static Task IgnoreExceptions(this Task task)
		{
			task.ContinueWith(delegate(Task t)
			{
				AggregateException exception = t.Exception;
			}, CancellationToken.None, TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
			return task;
		}
	}
}
