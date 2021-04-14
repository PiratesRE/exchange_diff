using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class JobRunner : IJobExecutionTracker, IProgress<short>
	{
		public JobRunner(IIntegrityCheckJob job, IJobStateTracker jobStateReporter, IJobProgressTracker jobProgressReporter)
		{
			this.job = job;
			this.jobStateReporter = jobStateReporter;
			this.jobProgressReporter = jobProgressReporter;
		}

		public JobRunner AssignJob(IIntegrityCheckJob job)
		{
			this.job = job;
			return this;
		}

		public void Run(Context context)
		{
			this.timeWatcher = new Stopwatch();
			this.timeWatcher.Start();
			this.InternalExecute(context, () => true);
		}

		public void Report(short progress)
		{
			this.progress = progress;
			this.jobProgressReporter.Report(new ProgressInfo
			{
				Progress = progress,
				LastExecutionTime = new DateTime?(DateTime.UtcNow),
				CorruptionsDetected = this.corruptionsDetected,
				CorruptionsFixed = this.corruptionsFixed,
				TimeInServer = this.timeWatcher.Elapsed
			});
		}

		void IJobExecutionTracker.OnCorruptionDetected(Corruption corruption)
		{
			if (this.corruptions == null)
			{
				this.corruptions = new List<Corruption>();
			}
			this.corruptions.Add(corruption);
			this.corruptionsDetected++;
			if (corruption.IsFixed)
			{
				this.corruptionsFixed++;
			}
			this.jobProgressReporter.Report(new ProgressInfo
			{
				Progress = this.progress,
				LastExecutionTime = new DateTime?(DateTime.UtcNow),
				CorruptionsDetected = this.corruptionsDetected,
				CorruptionsFixed = this.corruptionsFixed,
				TimeInServer = this.timeWatcher.Elapsed
			});
		}

		private void InternalExecute(Context context, Func<bool> shouldTaskContinue)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			IIntegrityCheckTask integrityCheckTask = TaskBuilder.Create(this.job.TaskId).TrackedBy(this).Build();
			bool isScheduled = this.job.Source == JobSource.Maintenance;
			bool flag = true;
			try
			{
				errorCode = integrityCheckTask.Execute(context, this.job.MailboxGuid, this.job.DetectOnly, isScheduled, shouldTaskContinue).Propagate((LID)36956U);
				flag = false;
			}
			finally
			{
				if (flag)
				{
					errorCode = ErrorCode.CreateErrorCanNotComplete((LID)60508U);
				}
				JobState state = (errorCode == ErrorCode.NoError) ? JobState.Completed : JobState.Failed;
				this.jobProgressReporter.Report(new ProgressInfo
				{
					Progress = 100,
					LastExecutionTime = new DateTime?(DateTime.UtcNow),
					CompletedTime = new DateTime?(DateTime.UtcNow),
					CorruptionsDetected = this.corruptionsDetected,
					CorruptionsFixed = this.corruptionsFixed,
					TimeInServer = this.timeWatcher.Elapsed,
					Corruptions = this.corruptions,
					Error = errorCode
				});
				this.jobStateReporter.MoveToState(state);
			}
		}

		private IIntegrityCheckJob job;

		private IJobStateTracker jobStateReporter;

		private IJobProgressTracker jobProgressReporter;

		private short progress;

		private int corruptionsDetected;

		private int corruptionsFixed;

		private List<Corruption> corruptions;

		private Stopwatch timeWatcher;
	}
}
