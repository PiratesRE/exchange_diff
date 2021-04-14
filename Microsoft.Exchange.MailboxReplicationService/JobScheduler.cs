using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class JobScheduler : SystemWorkloadBase
	{
		public static void ScheduleJob(IJob job)
		{
			CommonUtils.CheckForServiceStopping();
			job.ResetJob();
			lock (JobScheduler.staticLock)
			{
				JobScheduler.RunnableJobs.Add(job);
			}
		}

		private static ICollection<IJob> RunnableJobs
		{
			get
			{
				return JobScheduler.runnableJobsInstance.Value;
			}
		}

		public JobScheduler(WorkloadType workloadType)
		{
			this.JobStateChanged = null;
			this.wakeupJobsTimer = new Timer(new TimerCallback(this.WakeupJobs), null, 1000, 1000);
			this.workloadType = workloadType;
		}

		public override WorkloadType WorkloadType
		{
			get
			{
				return this.workloadType;
			}
		}

		public override string Id
		{
			get
			{
				return this.WorkloadType.ToString();
			}
		}

		public override int TaskCount
		{
			get
			{
				int result;
				lock (this.instanceLock)
				{
					lock (JobScheduler.staticLock)
					{
						result = this.waitingJobs.Count + JobScheduler.RunnableJobs.Count + this.runningJobs.Count;
					}
				}
				return result;
			}
		}

		public override int BlockedTaskCount
		{
			get
			{
				int result;
				lock (this.instanceLock)
				{
					lock (JobScheduler.staticLock)
					{
						result = JobScheduler.RunnableJobs.Count + this.waitingJobs.Count;
					}
				}
				return result;
			}
		}

		public event EventHandler<JobEventArgs> JobStateChanged;

		public void Start()
		{
			SystemWorkloadManager.RegisterWorkload(this);
		}

		public void Stop()
		{
			List<IJob> list;
			lock (this.instanceLock)
			{
				lock (JobScheduler.staticLock)
				{
					list = new List<IJob>(this.waitingJobs.Count + JobScheduler.RunnableJobs.Count);
					list.AddRange(this.waitingJobs);
					list.AddRange(JobScheduler.RunnableJobs);
				}
			}
			foreach (IJob job in list)
			{
				job.WaitForJobToBeDone();
			}
			this.wakeupJobsTimer.Dispose();
		}

		protected override SystemTaskBase GetTask(ResourceReservationContext context)
		{
			MrsSystemTask mrsSystemTask = null;
			SystemTaskBase result;
			lock (this.instanceLock)
			{
				lock (JobScheduler.staticLock)
				{
					if (JobScheduler.RunnableJobs.Count > 0)
					{
						List<IJob> list = new List<IJob>();
						List<IJob> list2 = new List<IJob>();
						foreach (IJob job in JobScheduler.RunnableJobs)
						{
							mrsSystemTask = job.GetTask(this, context);
							if (mrsSystemTask != null)
							{
								break;
							}
							switch (job.State)
							{
							case JobState.Waiting:
								list2.Add(job);
								break;
							case JobState.Finished:
								list.Add(job);
								break;
							}
						}
						if (mrsSystemTask != null)
						{
							JobScheduler.RunnableJobs.Remove(mrsSystemTask.Job);
							this.runningJobs.Add(mrsSystemTask.Job);
						}
						foreach (IJob job2 in list)
						{
							JobScheduler.RunnableJobs.Remove(job2);
							job2.RevertToPreviousUnthrottledState();
							this.OnJobStateChanged(job2, job2.State);
						}
						foreach (IJob item in list2)
						{
							JobScheduler.RunnableJobs.Remove(item);
							this.waitingJobs.Add(item);
						}
					}
					result = mrsSystemTask;
				}
			}
			return result;
		}

		protected override void CompleteTask(SystemTaskBase task)
		{
			MrsSystemTask mrsSystemTask = task as MrsSystemTask;
			IJob job = mrsSystemTask.Job;
			job.ProcessTaskExecutionResult(mrsSystemTask);
			switch (job.State)
			{
			case JobState.Runnable:
				lock (this.instanceLock)
				{
					lock (JobScheduler.staticLock)
					{
						this.runningJobs.Remove(job);
						JobScheduler.RunnableJobs.Add(job);
					}
				}
				return;
			case JobState.Failed:
				lock (this.instanceLock)
				{
					job.RevertToPreviousUnthrottledState();
					this.runningJobs.Remove(job);
				}
				this.OnJobStateChanged(job, job.State);
				return;
			default:
				return;
			}
		}

		protected virtual void OnJobStateChanged(IJob job, JobState state)
		{
			if (this.JobStateChanged != null)
			{
				JobEventArgs e = new JobEventArgs(job, state);
				this.JobStateChanged(this, e);
			}
		}

		private void WakeupJobs(object state)
		{
			if (this.waitingJobs.Count > 0)
			{
				lock (this.instanceLock)
				{
					if (this.waitingJobs.Count > 0)
					{
						for (int i = this.waitingJobs.Count - 1; i >= 0; i--)
						{
							IJob job = this.waitingJobs[i];
							lock (JobScheduler.staticLock)
							{
								if (job.ShouldWakeup)
								{
									this.waitingJobs.RemoveAt(i);
									job.ResetJob();
									JobScheduler.RunnableJobs.Add(job);
								}
							}
						}
					}
				}
			}
		}

		private const int WakeupJobsPeriodMilliseconds = 1000;

		private static readonly Lazy<ICollection<IJob>> runnableJobsInstance = new Lazy<ICollection<IJob>>(delegate()
		{
			if (MoveJob.CacheJobQueues)
			{
				return new SortedSet<IJob>(JobScheduler.JobsComparer.Instance);
			}
			return new LinkedList<IJob>();
		});

		private static readonly object staticLock = new object();

		protected WorkloadType workloadType;

		private List<IJob> waitingJobs = new List<IJob>();

		private HashSet<IJob> runningJobs = new HashSet<IJob>();

		private Timer wakeupJobsTimer;

		private object instanceLock = new object();

		private class JobsComparer : IComparer<IJob>
		{
			private JobsComparer()
			{
			}

			public static JobScheduler.JobsComparer Instance
			{
				get
				{
					return JobScheduler.JobsComparer.defaultInstance;
				}
			}

			public int Compare(IJob x, IJob y)
			{
				if (object.ReferenceEquals(x, y))
				{
					return 0;
				}
				return x.JobSortKey.CompareTo(y.JobSortKey);
			}

			private static readonly JobScheduler.JobsComparer defaultInstance = new JobScheduler.JobsComparer();
		}
	}
}
