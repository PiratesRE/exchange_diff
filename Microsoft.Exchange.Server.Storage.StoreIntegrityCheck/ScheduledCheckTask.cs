using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class ScheduledCheckTask : IntegrityCheckTaskBase
	{
		public ScheduledCheckTask(IJobExecutionTracker tracker) : base(new ScheduledCheckTask.AggregateJobExecutionTracker(tracker))
		{
			ScheduledCheckTaskConfiguration configuration = ScheduledCheckTaskConfiguration.GetConfiguration();
			this.isEnabled = configuration.IsEnabled;
			this.aggregateTracker = (ScheduledCheckTask.AggregateJobExecutionTracker)base.JobExecutionTracker;
			this.tasksDetectOnly = new List<IIntegrityCheckTask>(configuration.TaskIdsDetectOnly.Count);
			foreach (TaskId taskId in configuration.TaskIdsDetectOnly)
			{
				this.tasksDetectOnly.Add(TaskBuilder.Create(taskId).TrackedBy(this.aggregateTracker).Build());
			}
			this.tasksDetectAndFix = new List<IIntegrityCheckTask>(configuration.TaskIdsDetectAndFix.Count);
			foreach (TaskId taskId2 in configuration.TaskIdsDetectAndFix)
			{
				this.tasksDetectAndFix.Add(TaskBuilder.Create(taskId2).TrackedBy(this.aggregateTracker).Build());
			}
		}

		public override string TaskName
		{
			get
			{
				return "ScheduledCheckTask";
			}
		}

		public override ErrorCode Execute(Context context, MailboxEntry mailboxEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "Integrity check task \"{0}\" invoked on mailbox {1}, detect only={2}, enabled={3}", new object[]
				{
					this.TaskName,
					mailboxEntry.MailboxGuid,
					detectOnly,
					this.isEnabled
				});
			}
			foreach (IIntegrityCheckTask integrityCheckTask in this.tasksDetectOnly)
			{
				ErrorCode errorCode2 = integrityCheckTask.Execute(context, mailboxEntry.MailboxGuid, true, base.IsScheduled, shouldContinue);
				if (errorCode2 != ErrorCode.NoError && errorCode == ErrorCode.NoError)
				{
					errorCode = errorCode2;
				}
			}
			foreach (IIntegrityCheckTask integrityCheckTask2 in this.tasksDetectAndFix)
			{
				ErrorCode first = integrityCheckTask2.Execute(context, mailboxEntry.MailboxGuid, !base.IsScheduled && detectOnly, base.IsScheduled, shouldContinue);
				if (first != ErrorCode.NoError && errorCode == ErrorCode.NoError)
				{
					errorCode = first.Propagate((LID)37660U);
				}
			}
			stopwatch.Stop();
			StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(context.Database);
			if (base.IsScheduled && databaseInstance != null)
			{
				databaseInstance.ScheduledISIntegMailboxRate.Increment();
			}
			if (errorCode == ErrorCode.NoError && base.IsScheduled && !detectOnly)
			{
				errorCode = IntegrityCheckTaskBase.LockMailboxForOperation(context, mailboxEntry.MailboxNumber, delegate(MailboxState mailboxState)
				{
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, mailboxState))
					{
						mailbox.SetISIntegScheduledLast(context, mailboxState.UtcNow, new int?((int)stopwatch.ElapsedMilliseconds), new int?(this.aggregateTracker.CorruptionCount));
						mailbox.Save(context);
					}
					mailboxState.CleanupAsNonActive(context);
					return ErrorCode.NoError;
				});
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)54044U);
				}
			}
			return errorCode;
		}

		private readonly bool isEnabled;

		private readonly List<IIntegrityCheckTask> tasksDetectOnly;

		private readonly List<IIntegrityCheckTask> tasksDetectAndFix;

		private ScheduledCheckTask.AggregateJobExecutionTracker aggregateTracker;

		internal class AggregateJobExecutionTracker : IJobExecutionTracker, IProgress<short>
		{
			public AggregateJobExecutionTracker(IJobExecutionTracker tracker)
			{
				this.baseTracker = tracker;
			}

			public int CorruptionCount
			{
				get
				{
					return this.corruptionCount;
				}
			}

			void IJobExecutionTracker.OnCorruptionDetected(Corruption corruption)
			{
				this.corruptionCount++;
				this.baseTracker.OnCorruptionDetected(corruption);
			}

			public void Report(short progress)
			{
				this.baseTracker.Report(progress);
			}

			private int corruptionCount;

			private IJobExecutionTracker baseTracker;
		}
	}
}
