using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class Job
	{
		protected Job(long id, ThrottlingContext context, QueuedRecipientsByAgeToken token, IList<StageInfo> stages)
		{
			this.Id = id;
			this.context = context;
			this.token = token;
			this.stages = stages;
			ExTraceGlobals.SchedulerTracer.TraceDebug<Job>((long)this.GetHashCode(), "Job({0}) started", this);
		}

		public IList<StageInfo> Stages
		{
			get
			{
				return this.stages;
			}
		}

		public bool RootMailItemDeferred
		{
			get
			{
				return this.rootMailItemDeferred;
			}
			set
			{
				this.rootMailItemDeferred = value;
			}
		}

		public static void ReleaseItem(TransportMailItem mailItem)
		{
			mailItem.ReleaseFromActiveMaterializedLazy();
		}

		public void RunningTaskCompleted(TaskContext completedTask, bool async)
		{
			TaskContext taskContext = (TaskContext)Interlocked.CompareExchange(ref this.executingTask, null, completedTask);
			if (taskContext != completedTask)
			{
				throw new InvalidOperationException("The task that completed was not running previously");
			}
			if (this.pendingTasksListHead == null)
			{
				this.CompletedInternal(taskContext.SubjectMailItem);
			}
			else if (async)
			{
				this.PendingInternal();
			}
			if (async)
			{
				this.GoneAsyncInternal();
			}
		}

		public void EnqueuePendingTask(int stage, TransportMailItem mailItem, AcceptedDomainCollection acceptedDomains)
		{
			this.EnqueuePendingTask(stage, mailItem, 0, null, null, null, acceptedDomains);
		}

		public bool ExecutePendingTasks()
		{
			bool result = false;
			while (!this.IsRetired)
			{
				TaskContext nextPendingTask = this.GetNextPendingTask();
				if (nextPendingTask != null)
				{
					TaskCompletion taskCompletion = nextPendingTask.Invoke();
					result = true;
					if (taskCompletion == TaskCompletion.Completed)
					{
						continue;
					}
				}
				return result;
			}
			this.Retire();
			return result;
		}

		public bool IsEmpty
		{
			get
			{
				return this.pendingTasksListHead == null;
			}
		}

		public override string ToString()
		{
			return this.Id.ToString();
		}

		public abstract bool TryGetDeferToken(TransportMailItem mailItem, out AcquireToken deferToken);

		public abstract void MarkDeferred(TransportMailItem mailItem);

		public void Retire()
		{
			ExTraceGlobals.SchedulerTracer.TraceDebug<Job>((long)this.GetHashCode(), "Abandon Job ({0}) as the scheduler is retired", this);
			TransportMailItem transportMailItem = null;
			lock (this)
			{
				for (TaskContext friendNextTaskContext = this.pendingTasksListHead; friendNextTaskContext != null; friendNextTaskContext = friendNextTaskContext.FriendNextTaskContext)
				{
					friendNextTaskContext.TaskRetired();
					if (transportMailItem == null)
					{
						transportMailItem = friendNextTaskContext.SubjectMailItem;
					}
				}
			}
			this.RetireInternal(transportMailItem);
		}

		protected abstract bool IsRetired { get; }

		protected abstract void CompletedInternal(TransportMailItem mailItem);

		protected abstract void PendingInternal();

		protected abstract void GoneAsyncInternal();

		protected abstract void RetireInternal(TransportMailItem mailItem);

		internal void EnqueuePendingTask(int stage, TransportMailItem mailItem, int latestMimeVersion, WeakReference lastKnownMimeDocument, IMExSession mexSession, AgentLatencyTracker agentLatencyTracker, AcceptedDomainCollection acceptedDomains)
		{
			TaskContext taskContext = new TaskContext(stage, mailItem, latestMimeVersion, lastKnownMimeDocument, this, mexSession, agentLatencyTracker, acceptedDomains);
			lock (this)
			{
				if (this.pendingTasksListHead == null || this.pendingTasksListHead.Stage <= stage)
				{
					taskContext.FriendNextTaskContext = this.pendingTasksListHead;
					this.pendingTasksListHead = taskContext;
				}
				else
				{
					TaskContext friendNextTaskContext = this.pendingTasksListHead;
					while (friendNextTaskContext.FriendNextTaskContext != null && friendNextTaskContext.FriendNextTaskContext.Stage > stage)
					{
						friendNextTaskContext = friendNextTaskContext.FriendNextTaskContext;
					}
					taskContext.FriendNextTaskContext = friendNextTaskContext.FriendNextTaskContext;
					friendNextTaskContext.FriendNextTaskContext = taskContext;
				}
			}
		}

		internal CategorizerItem GetCategorizerItemById(long mailItemId)
		{
			lock (this)
			{
				TaskContext taskContext;
				for (taskContext = this.pendingTasksListHead; taskContext != null; taskContext = taskContext.FriendNextTaskContext)
				{
					if (taskContext.SubjectMailItem.RecordId == mailItemId)
					{
						return new CategorizerItem(taskContext.SubjectMailItem, taskContext.Stage);
					}
				}
				taskContext = (TaskContext)this.executingTask;
				if (taskContext != null && taskContext.SubjectMailItem.RecordId == mailItemId)
				{
					return new CategorizerItem(taskContext.SubjectMailItem, taskContext.Stage);
				}
			}
			return null;
		}

		internal void VisitCategorizerItems(Func<CategorizerItem, bool> visitor)
		{
			lock (this)
			{
				TaskContext taskContext;
				for (taskContext = this.pendingTasksListHead; taskContext != null; taskContext = taskContext.FriendNextTaskContext)
				{
					visitor(new CategorizerItem(taskContext.SubjectMailItem, taskContext.Stage));
				}
				taskContext = (TaskContext)this.executingTask;
				if (taskContext != null)
				{
					visitor(new CategorizerItem(taskContext.SubjectMailItem, taskContext.Stage));
				}
			}
		}

		internal int GetMailItemCount()
		{
			int num = 0;
			lock (this)
			{
				for (TaskContext friendNextTaskContext = this.pendingTasksListHead; friendNextTaskContext != null; friendNextTaskContext = friendNextTaskContext.FriendNextTaskContext)
				{
					num++;
				}
				if (this.executingTask != null && num == 0)
				{
					num++;
				}
			}
			return num;
		}

		internal ThrottlingContext GetThrottlingContext()
		{
			return this.context;
		}

		internal QueuedRecipientsByAgeToken GetQueuedRecipientsByAgeToken()
		{
			return this.token;
		}

		private TaskContext GetNextPendingTask()
		{
			TaskContext taskContext = this.pendingTasksListHead;
			if (taskContext != null)
			{
				lock (this)
				{
					this.pendingTasksListHead = taskContext.FriendNextTaskContext;
					taskContext.FriendNextTaskContext = null;
					TaskContext taskContext2 = (TaskContext)Interlocked.CompareExchange(ref this.executingTask, taskContext, null);
					if (taskContext2 != null)
					{
						throw new InvalidOperationException("don't allow concurrent tasks");
					}
				}
			}
			return taskContext;
		}

		public readonly long Id;

		protected static long nextJobId = 1000L;

		private TaskContext pendingTasksListHead;

		private object executingTask;

		private ThrottlingContext context;

		private QueuedRecipientsByAgeToken token;

		private readonly IList<StageInfo> stages;

		private bool rootMailItemDeferred;
	}
}
