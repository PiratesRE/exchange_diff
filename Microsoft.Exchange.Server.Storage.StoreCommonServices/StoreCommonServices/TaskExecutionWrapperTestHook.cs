using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal static class TaskExecutionWrapperTestHook
	{
		public static IDisposable Set(Action<TaskTypeId> testDelegate)
		{
			return TaskExecutionWrapperTestHook.testHook.SetTestHook(testDelegate);
		}

		public static void Invoke(TaskTypeId taskTypeId)
		{
			if (TaskExecutionWrapperTestHook.testHook.Value != null)
			{
				TaskExecutionWrapperTestHook.testHook.Value(taskTypeId);
			}
		}

		private static readonly Hookable<Action<TaskTypeId>> testHook = Hookable<Action<TaskTypeId>>.Create(true, null);
	}
}
