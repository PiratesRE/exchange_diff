using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class SingleExecutionTask<T> : Task<T>
	{
		public static SingleExecutionTask<T> CreateSingleExecutionTask(TaskList taskList, Task<T>.TaskCallback callback, T context, ThreadPriority threadPriority, int stackSize, TaskFlags taskFlags)
		{
			if (taskList == null)
			{
				return null;
			}
			LinkedListNode<Task> linkedListNode = null;
			bool flag = (byte)(taskFlags & TaskFlags.AutoStart) != 0;
			taskFlags &= ~TaskFlags.AutoStart;
			SingleExecutionTask<T> singleExecutionTask = new SingleExecutionTask<T>(taskList, callback, context, threadPriority, stackSize, taskFlags);
			try
			{
				linkedListNode = taskList.Add(singleExecutionTask, false);
			}
			catch (ObjectDisposedException exception)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
			}
			if (linkedListNode != null)
			{
				singleExecutionTask.taskListItem = linkedListNode;
				if (flag)
				{
					singleExecutionTask.Start();
				}
			}
			else
			{
				singleExecutionTask.Dispose();
				singleExecutionTask = null;
			}
			return singleExecutionTask;
		}

		public static Task<T> CreateSingleExecutionTask(TaskList taskList, Task<T>.TaskCallback callback, T context, bool autoStart)
		{
			return SingleExecutionTask<T>.CreateSingleExecutionTask(taskList, callback, context, ThreadPriority.Normal, 0, (TaskFlags)(6 | (autoStart ? 1 : 0)));
		}

		private SingleExecutionTask(TaskList taskList, Task<T>.TaskCallback callback, T context, ThreadPriority threadPriority, int stackSize, TaskFlags taskFlags) : base(callback, context, threadPriority, stackSize, taskFlags)
		{
			this.taskList = taskList;
		}

		public override void Start()
		{
			base.Start();
		}

		protected override void Invoke()
		{
			try
			{
				base.Invoke();
			}
			finally
			{
				LinkedListNode<Task> linkedListNode = null;
				using (LockManager.Lock(base.StateLock))
				{
					if (this.taskListItem != null)
					{
						linkedListNode = this.taskListItem;
						this.taskListItem = null;
					}
				}
				if (linkedListNode != null && this.taskList.Remove(linkedListNode))
				{
					base.Dispose();
				}
			}
		}

		private LinkedListNode<Task> taskListItem;

		private TaskList taskList;
	}
}
