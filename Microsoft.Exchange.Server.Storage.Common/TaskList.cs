using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class TaskList : DisposableBase
	{
		public TaskList()
		{
			this.taskList = new LinkedList<Task>();
			this.taskListToDispose = this.taskList;
		}

		internal Task Find(Predicate<Task> match)
		{
			using (LockManager.Lock(this.taskListLock, LockManager.LockType.TaskList))
			{
				if (this.taskList != null)
				{
					base.CheckDisposed();
					foreach (Task task in this.taskList)
					{
						if (match(task))
						{
							return task;
						}
					}
				}
			}
			return null;
		}

		public LinkedListNode<Task> Add(Task task, bool autoStart)
		{
			LinkedListNode<Task> result = null;
			Action action = null;
			using (LockManager.Lock(this.taskListLock, LockManager.LockType.TaskList))
			{
				if (this.taskList != null)
				{
					base.CheckDisposed();
					result = this.taskList.AddLast(task);
					action = new Action(task.Start);
					task = null;
				}
			}
			if (task != null)
			{
				task.Stop();
				task.Dispose();
			}
			else if (autoStart)
			{
				action();
			}
			return result;
		}

		public bool Remove(LinkedListNode<Task> taskNode)
		{
			bool result;
			using (LockManager.Lock(this.taskListLock, LockManager.LockType.TaskList))
			{
				if (this.taskList != null && taskNode.List != null)
				{
					base.CheckDisposed();
					taskNode.List.Remove(taskNode);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool Remove(Task t)
		{
			bool result;
			using (LockManager.Lock(this.taskListLock, LockManager.LockType.TaskList))
			{
				if (this.taskList != null)
				{
					base.CheckDisposed();
					result = this.taskList.Remove(t);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public void WaitAllForTestOnly()
		{
			using (LockManager.Lock(this.taskListLock, LockManager.LockType.TaskList))
			{
				if (this.taskList != null)
				{
					base.CheckDisposed();
					foreach (Task task in this.taskList)
					{
						task.WaitForCompletion(TimeSpan.FromSeconds(1.0));
						task.Stop();
					}
				}
			}
		}

		public void StartAll()
		{
			using (LockManager.Lock(this.taskListLock, LockManager.LockType.TaskList))
			{
				if (this.taskList != null)
				{
					base.CheckDisposed();
					foreach (Task task in this.taskList)
					{
						task.Start();
					}
				}
			}
		}

		public void StopAll()
		{
			this.InternalStopAll(false);
		}

		public void StopAllAndPreventFurtherRegistration()
		{
			this.InternalStopAll(true);
		}

		private void InternalStopAll(bool preventFurtherRegistration)
		{
			using (LockManager.Lock(this.taskListLock, LockManager.LockType.TaskList))
			{
				if (this.taskList != null)
				{
					base.CheckDisposed();
					foreach (Task task in this.taskList)
					{
						task.Stop();
					}
					if (preventFurtherRegistration)
					{
						this.taskList = null;
					}
				}
			}
		}

		public void StopAndShutdown()
		{
			this.Shutdown(false);
		}

		public void WaitAndShutdown()
		{
			this.Shutdown(true);
		}

		private void Shutdown(bool waitForCompletion)
		{
			using (LockManager.Lock(this.taskListLock, LockManager.LockType.TaskList))
			{
				if (this.taskList != null)
				{
					base.CheckDisposed();
					this.taskList = null;
				}
			}
			if (this.taskListToDispose != null)
			{
				for (;;)
				{
					Task value;
					using (LockManager.Lock(this.taskListLock, LockManager.LockType.TaskList))
					{
						if (this.taskListToDispose.Count == 0)
						{
							break;
						}
						value = this.taskListToDispose.First.Value;
						this.taskListToDispose.Remove(this.taskListToDispose.First);
					}
					if (waitForCompletion)
					{
						value.WaitForCompletion();
					}
					else
					{
						value.Stop();
					}
					value.Dispose();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TaskList>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.StopAndShutdown();
			}
		}

		internal const int CleanupIntervalInSeconds = 15;

		private LinkedList<Task> taskList;

		private LinkedList<Task> taskListToDispose;

		private object taskListLock = new object();
	}
}
