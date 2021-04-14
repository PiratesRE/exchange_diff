using System;

namespace System.Threading.Tasks
{
	internal class SystemThreadingTasks_FutureDebugView<TResult>
	{
		public SystemThreadingTasks_FutureDebugView(Task<TResult> task)
		{
			this.m_task = task;
		}

		public TResult Result
		{
			get
			{
				if (this.m_task.Status != TaskStatus.RanToCompletion)
				{
					return default(TResult);
				}
				return this.m_task.Result;
			}
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

		private Task<TResult> m_task;
	}
}
