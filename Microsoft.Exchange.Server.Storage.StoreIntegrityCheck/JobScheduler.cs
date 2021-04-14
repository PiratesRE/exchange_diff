using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class JobScheduler : IJobScheduler
	{
		public JobScheduler(StoreDatabase database)
		{
			this.database = database;
			this.readyQueue = new JobScheduler.PriorityQueue();
			this.runningQueue = new HashSet<Guid>();
			this.ageoutQueue = new Queue<IntegrityCheckJob>();
			this.perfCounters = PerformanceCounterFactory.GetDatabaseInstance(database);
		}

		public static TimeSpan ScheduleInterval
		{
			get
			{
				return JobScheduler.interval;
			}
		}

		public StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		public static JobScheduler Instance(StoreDatabase database)
		{
			return database.ComponentData[JobScheduler.jobSchedulerSlot] as JobScheduler;
		}

		public void ScheduleJob(IntegrityCheckJob job)
		{
			((IJobStateTracker)job).MoveToState(JobState.Pending);
			using (LockManager.Lock(this))
			{
				this.readyQueue.Enqueue(job);
			}
			if (this.perfCounters != null)
			{
				this.perfCounters.ISIntegStorePendingJobs.Increment();
			}
		}

		public void RemoveJob(IntegrityCheckJob job)
		{
			bool flag;
			using (LockManager.Lock(this))
			{
				flag = this.readyQueue.Remove(job.JobGuid);
			}
			if (flag && this.perfCounters != null)
			{
				this.perfCounters.ISIntegStorePendingJobs.Decrement();
			}
		}

		public void ExecuteJob(Context context, IntegrityCheckJob job)
		{
			using (LockManager.Lock(this))
			{
				if (job != null)
				{
					if (this.runningQueue.Contains(job.JobGuid) || this.ageoutQueue.Contains(job))
					{
						return;
					}
					this.readyQueue.Dequeue(job.JobGuid);
				}
				else
				{
					job = this.readyQueue.Dequeue(JobPriority.High, JobPriority.Low);
				}
				if (job == null)
				{
					return;
				}
			}
			if (this.perfCounters != null)
			{
				this.perfCounters.ISIntegStorePendingJobs.Decrement();
			}
			try
			{
				using (LockManager.Lock(this))
				{
					this.runningQueue.Add(job.JobGuid);
				}
				((IJobStateTracker)job).MoveToState(JobState.Running);
				JobRunner jobRunner = new JobRunner(job, job, job);
				jobRunner.Run(context);
			}
			finally
			{
				using (LockManager.Lock(this))
				{
					this.runningQueue.Remove(job.JobGuid);
					this.ageoutQueue.Enqueue(job);
				}
				if (job.State == JobState.Failed && this.perfCounters != null)
				{
					this.perfCounters.ISIntegStoreFailedJobs.Decrement();
				}
			}
		}

		public IEnumerable<IntegrityCheckJob> GetReadyJobs(JobPriority priority)
		{
			IEnumerable<IntegrityCheckJob> readyJobs;
			using (LockManager.Lock(this))
			{
				readyJobs = this.readyQueue.GetReadyJobs(priority);
			}
			return readyJobs;
		}

		internal static void Initialize()
		{
			if (JobScheduler.jobSchedulerSlot == -1)
			{
				JobScheduler.jobSchedulerSlot = StoreDatabase.AllocateComponentDataSlot();
			}
		}

		internal static void MountEventHandler(Context context, StoreDatabase database, bool readOnly)
		{
			JobScheduler jobScheduler = new JobScheduler(database);
			database.ComponentData[JobScheduler.jobSchedulerSlot] = jobScheduler;
			if (!readOnly)
			{
				Task<JobScheduler>.TaskCallback callback = TaskExecutionWrapper<JobScheduler>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.OnlineIntegrityCheck, ClientType.System, database.MdbGuid), new TaskExecutionWrapper<JobScheduler>.TaskCallback<Context>(jobScheduler.IntegrityCheckJobScheduleThread));
				RecurringTask<JobScheduler> task = new RecurringTask<JobScheduler>(callback, jobScheduler, JobScheduler.interval, false);
				database.TaskList.Add(task, true);
			}
		}

		internal static void DismountEventHandler(StoreDatabase database)
		{
			database.ComponentData[JobScheduler.jobSchedulerSlot] = null;
		}

		private void IntegrityCheckJobScheduleThread(Context context, JobScheduler jobScheduler, Func<bool> shouldCallbackContinue)
		{
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "Integrity check job manager thread start running on database \"" + jobScheduler.Database.MdbName + "\"");
			}
			for (int i = 0; i < JobScheduler.jobBatchSize; i++)
			{
				IntegrityCheckJob integrityCheckJob = null;
				using (LockManager.Lock(this))
				{
					integrityCheckJob = this.readyQueue.Peek(JobPriority.High, JobPriority.High);
				}
				if (integrityCheckJob == null)
				{
					break;
				}
				using (context.AssociateWithDatabase(this.Database))
				{
					this.ExecuteJob(context, integrityCheckJob);
				}
			}
			this.AgeoutOldJobs();
		}

		private void AgeoutOldJobs()
		{
			IntegrityCheckJob integrityCheckJob = null;
			do
			{
				using (LockManager.Lock(this))
				{
					integrityCheckJob = null;
					if (this.ageoutQueue.Count != 0)
					{
						integrityCheckJob = this.ageoutQueue.Peek();
						if (integrityCheckJob.CompletedTime == null || DateTime.UtcNow - integrityCheckJob.CompletedTime <= ConfigurationSchema.IntegrityCheckJobAgeoutTimeSpan.Value)
						{
							integrityCheckJob = null;
						}
						else
						{
							integrityCheckJob = this.ageoutQueue.Dequeue();
						}
					}
				}
				if (integrityCheckJob != null)
				{
					InMemoryJobStorage.Instance(this.database).RemoveJob(integrityCheckJob.JobGuid);
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid>(0L, "Integrity check job {0} aged out", integrityCheckJob.JobGuid);
					}
				}
			}
			while (integrityCheckJob != null);
		}

		private const int InitialCapacity = 500;

		private static int jobSchedulerSlot = -1;

		private static int jobBatchSize = 50;

		private static TimeSpan interval = TimeSpan.FromSeconds(30.0);

		private StoreDatabase database;

		private StorePerDatabasePerformanceCountersInstance perfCounters;

		private JobScheduler.PriorityQueue readyQueue;

		private HashSet<Guid> runningQueue;

		private Queue<IntegrityCheckJob> ageoutQueue;

		private class PriorityQueue
		{
			public PriorityQueue()
			{
				this.map = new Dictionary<Guid, LinkedListNode<IntegrityCheckJob>>();
				this.activeQueue = new List<LinkedList<IntegrityCheckJob>>();
				for (int i = 0; i <= 2; i++)
				{
					this.activeQueue.Add(new LinkedList<IntegrityCheckJob>());
				}
			}

			public bool IsEmpty
			{
				get
				{
					return this.map.Count == 0;
				}
			}

			public void Enqueue(IntegrityCheckJob job)
			{
				LinkedListNode<IntegrityCheckJob> linkedListNode = new LinkedListNode<IntegrityCheckJob>(job);
				this.map.Add(job.JobGuid, linkedListNode);
				this.activeQueue[(int)job.Priority].AddLast(linkedListNode);
			}

			public IntegrityCheckJob Dequeue(Guid jobGuid)
			{
				IntegrityCheckJob integrityCheckJob = this.Peek(jobGuid);
				if (integrityCheckJob != null)
				{
					this.Remove(integrityCheckJob);
				}
				return integrityCheckJob;
			}

			public IntegrityCheckJob Dequeue(JobPriority from, JobPriority to)
			{
				IntegrityCheckJob integrityCheckJob = this.Peek(from, to);
				if (integrityCheckJob != null)
				{
					this.Remove(integrityCheckJob);
				}
				return integrityCheckJob;
			}

			public bool Remove(Guid jobGuid)
			{
				bool result = false;
				LinkedListNode<IntegrityCheckJob> linkedListNode = null;
				if (!this.IsEmpty && this.map.TryGetValue(jobGuid, out linkedListNode) && linkedListNode != null)
				{
					this.map.Remove(jobGuid);
					this.activeQueue[(int)linkedListNode.Value.Priority].Remove(linkedListNode);
					result = true;
				}
				return result;
			}

			public void Remove(IntegrityCheckJob job)
			{
				this.Remove(job.JobGuid);
			}

			public IEnumerable<IntegrityCheckJob> GetReadyJobs(JobPriority priority)
			{
				if (!this.IsEmpty && this.activeQueue[(int)priority].Count != 0)
				{
					List<IntegrityCheckJob> list = new List<IntegrityCheckJob>(JobScheduler.jobBatchSize);
					LinkedListNode<IntegrityCheckJob> linkedListNode = this.activeQueue[(int)priority].First;
					int num = 0;
					do
					{
						list.Add(linkedListNode.Value);
						linkedListNode = linkedListNode.Next;
						num++;
					}
					while (num < JobScheduler.jobBatchSize && linkedListNode != null);
					return list;
				}
				return null;
			}

			public IntegrityCheckJob Peek(Guid jobGuid)
			{
				if (!this.IsEmpty)
				{
					LinkedListNode<IntegrityCheckJob> linkedListNode = null;
					if (this.map.TryGetValue(jobGuid, out linkedListNode))
					{
						return linkedListNode.Value;
					}
				}
				return null;
			}

			public IntegrityCheckJob Peek(JobPriority from, JobPriority to)
			{
				if (!this.IsEmpty)
				{
					LinkedList<IntegrityCheckJob> linkedList = null;
					for (int i = (int)from; i <= (int)to; i++)
					{
						if (this.activeQueue[i].Count != 0)
						{
							linkedList = this.activeQueue[i];
							break;
						}
					}
					if (linkedList != null)
					{
						return linkedList.First.Value;
					}
				}
				return null;
			}

			private Dictionary<Guid, LinkedListNode<IntegrityCheckJob>> map;

			private List<LinkedList<IntegrityCheckJob>> activeQueue;
		}
	}
}
