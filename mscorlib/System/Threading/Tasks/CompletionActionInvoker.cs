using System;
using System.Security;

namespace System.Threading.Tasks
{
	internal sealed class CompletionActionInvoker : IThreadPoolWorkItem
	{
		internal CompletionActionInvoker(ITaskCompletionAction action, Task completingTask)
		{
			this.m_action = action;
			this.m_completingTask = completingTask;
		}

		[SecurityCritical]
		public void ExecuteWorkItem()
		{
			this.m_action.Invoke(this.m_completingTask);
		}

		[SecurityCritical]
		public void MarkAborted(ThreadAbortException tae)
		{
		}

		private readonly ITaskCompletionAction m_action;

		private readonly Task m_completingTask;
	}
}
