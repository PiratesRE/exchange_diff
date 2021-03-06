using System;

namespace System.Threading.Tasks
{
	internal class SystemThreadingTasks_TaskDebugView
	{
		public SystemThreadingTasks_TaskDebugView(Task task)
		{
			this.m_task = task;
		}

		public object AsyncState
		{
			get
			{
				return this.m_task.AsyncState;
			}
		}

		public TaskCreationOptions CreationOptions
		{
			get
			{
				return this.m_task.CreationOptions;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.m_task.Exception;
			}
		}

		public int Id
		{
			get
			{
				return this.m_task.Id;
			}
		}

		public bool CancellationPending
		{
			get
			{
				return this.m_task.Status == TaskStatus.WaitingToRun && this.m_task.CancellationToken.IsCancellationRequested;
			}
		}

		public TaskStatus Status
		{
			get
			{
				return this.m_task.Status;
			}
		}

		private Task m_task;
	}
}
