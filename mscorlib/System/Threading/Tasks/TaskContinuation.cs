using System;
using System.Security;

namespace System.Threading.Tasks
{
	internal abstract class TaskContinuation
	{
		internal abstract void Run(Task completedTask, bool bCanInlineContinuationTask);

		[SecuritySafeCritical]
		protected static void InlineIfPossibleOrElseQueue(Task task, bool needsProtection)
		{
			if (needsProtection)
			{
				if (!task.MarkStarted())
				{
					return;
				}
			}
			else
			{
				task.m_stateFlags |= 65536;
			}
			try
			{
				if (!task.m_taskScheduler.TryRunInline(task, false))
				{
					task.m_taskScheduler.InternalQueueTask(task);
				}
			}
			catch (Exception ex)
			{
				if (!(ex is ThreadAbortException) || (task.m_stateFlags & 134217728) == 0)
				{
					TaskSchedulerException exceptionObject = new TaskSchedulerException(ex);
					task.AddException(exceptionObject);
					task.Finish(false);
				}
			}
		}

		internal abstract Delegate[] GetDelegateContinuationsForDebugger();
	}
}
