using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Assistants.Diagnostics;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Assistants.Logging;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class TimeBasedDatabaseJob : Base
	{
		protected TimeBasedDatabaseJob(TimeBasedDatabaseDriver driver, List<MailboxData> queue, PoisonMailboxControl poisonControl, PerformanceCountersPerDatabaseInstance databaseCounters)
		{
			this.Finished = false;
			if (driver == null)
			{
				throw new ArgumentNullException("driver", "Time based Database Job can not be started with a null driver");
			}
			this.Driver = driver;
			this.SetPendingQueue(queue);
			this.poisonControl = poisonControl;
			this.performanceCounters = databaseCounters;
			this.initialPendingQueueCount = this.pendingQueue.Count;
			this.OnDemandMailboxCount = 0;
			this.mailboxesProcessedFailureCount = 0;
			this.mailboxesProcessedSuccessfullyCount = 0;
			this.mailboxesFailedToOpenStoreSessionCount = 0;
			this.mailboxesRetriedCount = 0;
		}

		private protected TimeBasedDatabaseDriver Driver { protected get; private set; }

		protected int InterestingMailboxCount
		{
			get
			{
				return this.initialPendingQueueCount;
			}
		}

		private protected int OnDemandMailboxCount { protected get; private set; }

		public DateTime StartTime { get; protected set; }

		public DateTime EndTime { get; protected set; }

		public TimeSpan Duration
		{
			get
			{
				if (this.Finished)
				{
					return this.EndTime - this.StartTime;
				}
				return DateTime.UtcNow - this.StartTime;
			}
		}

		public bool Finished { get; private set; }

		public int MailboxesQueued
		{
			get
			{
				int result;
				lock (this.instanceLock)
				{
					result = this.activeQueue.Count + this.pendingQueue.Count;
				}
				return result;
			}
		}

		internal ITimeBasedAssistant Assistant
		{
			get
			{
				return this.Driver.Assistant;
			}
		}

		protected DatabaseInfo DatabaseInfo
		{
			get
			{
				return this.Driver.DatabaseInfo;
			}
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Concat(new string[]
				{
					base.GetType().Name,
					" for database '",
					this.Driver.DatabaseInfo.DisplayName,
					"', assistant ",
					this.Driver.Assistant.GetType().Name
				});
			}
			return this.toString;
		}

		public AssistantTaskContext ProcessNextMailbox(AssistantTaskContext context)
		{
			ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceDebug<TimeBasedDatabaseJob>((long)this.GetHashCode(), "{0}: ProcessNextMailbox", this);
			MailboxData mailboxData = context.MailboxData;
			lock (this.instanceLock)
			{
				if (mailboxData == null)
				{
					ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceDebug<TimeBasedDatabaseJob>((long)this.GetHashCode(), "{0}: No more mailboxes to process", this);
					return null;
				}
				if (!this.loggedBegin)
				{
					ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceDebug<TimeBasedDatabaseJob, int>((long)this.GetHashCode(), "{0}: Processing first mailbox out of {1}", this, this.initialPendingQueueCount);
					this.LogJobBegin(this.initialPendingQueueCount);
					this.loggedBegin = true;
				}
			}
			AssistantTaskContext assistantTaskContext = this.ProcessMailbox(context);
			AssistantTaskContext result;
			lock (this.instanceLock)
			{
				if (assistantTaskContext == null)
				{
					ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceDebug((long)this.GetHashCode(), "{0}: Finished processing of mailbox.  mailboxesProcessedSuccessfully: {1}, mailboxesProcessedError: {2}, mailboxesFailedToOpenStoreSession: {3}, mailboxesRetriedCount: {4},remaining: {5}", new object[]
					{
						this,
						this.mailboxesProcessedSuccessfullyCount,
						this.mailboxesProcessedFailureCount,
						this.mailboxesFailedToOpenStoreSessionCount,
						this.mailboxesRetriedCount,
						this.pendingQueue.Count
					});
				}
				result = assistantTaskContext;
			}
			return result;
		}

		public uint RequestStop()
		{
			uint num = 0U;
			ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceDebug<TimeBasedDatabaseJob>((long)this.GetHashCode(), "{0}: RequestingStop...", this);
			lock (this.instanceLock)
			{
				num = (uint)this.pendingQueue.Count;
				this.Driver.DecrementNumberOfMailboxes(this.pendingQueue.Count);
				this.pendingQueue.Clear();
				this.FinishIfNecessary();
			}
			base.TracePfd("PFD AIS {0} {1}: RequestedStop.  Skipping {2} mailboxes", new object[]
			{
				27223,
				this,
				num
			});
			return num;
		}

		public bool Remove(MailboxData mailbox)
		{
			lock (this.instanceLock)
			{
				if (this.pendingQueue.Remove(mailbox))
				{
					this.OnDemandMailboxCount++;
					this.Driver.DecrementNumberOfMailboxes();
					ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceDebug((long)this.GetHashCode(), "{0}: Removed mailbox.  total removed: {1}, mailboxesProcessedSuccessfully: {2}, mailboxesProcessedError: {3}, mailboxesFailedToOpenStoreSession: {4}, mailboxesRetriedCount: {5},remaining: {6}", new object[]
					{
						this,
						this.OnDemandMailboxCount,
						this.mailboxesProcessedSuccessfullyCount,
						this.mailboxesProcessedFailureCount,
						this.mailboxesFailedToOpenStoreSessionCount,
						this.mailboxesRetriedCount,
						this.pendingQueue.Count
					});
					this.FinishIfNecessary();
					return true;
				}
			}
			return false;
		}

		public bool IsMailboxActiveOrPending(MailboxData mailbox)
		{
			bool result;
			lock (this.instanceLock)
			{
				result = (this.activeQueue.Contains(mailbox) || this.pendingQueue.Contains(mailbox));
			}
			return result;
		}

		public bool HasTask()
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			bool spreadLoad = snapshot.MailboxAssistants.GetObject<IMailboxAssistantSettings>(this.Driver.AssistantType.Identifier, new object[0]).SpreadLoad;
			lock (this.instanceLock)
			{
				switch (this.Driver.Governor.GetHierarchyStatus())
				{
				case GovernorStatus.Retry:
					if (!this.retry)
					{
						return false;
					}
					this.retry = false;
					break;
				case GovernorStatus.Failure:
					return false;
				}
				if (this.pendingQueue == null || this.pendingQueue.Count == 0)
				{
					return false;
				}
				if (spreadLoad)
				{
					float num = (float)(this.InterestingMailboxCount - this.pendingQueue.Count - 1) / (float)this.InterestingMailboxCount;
					float num2 = (float)this.Duration.Ticks / (float)this.Driver.AssistantType.WorkCycleCheckpoint.Ticks;
					if (num > num2)
					{
						return false;
					}
				}
			}
			return true;
		}

		public DiagnosticsSummaryJob GetJobDiagnosticsSummary()
		{
			int processing;
			int queued;
			lock (this.instanceLock)
			{
				processing = ((this.activeQueue == null) ? 0 : this.activeQueue.Count);
				queued = ((this.pendingQueue == null) ? 0 : this.pendingQueue.Count);
			}
			return new DiagnosticsSummaryJob(processing, this.mailboxesProcessedSuccessfullyCount, this.mailboxesProcessedFailureCount, this.mailboxesFailedToOpenStoreSessionCount, this.mailboxesRetriedCount, queued);
		}

		public List<Guid> GetMailboxGuidList(bool isActive)
		{
			List<Guid> list = new List<Guid>();
			lock (this.instanceLock)
			{
				list.AddRange(from mbx in isActive ? this.activeQueue : this.pendingQueue
				select mbx.MailboxGuid);
			}
			return list;
		}

		protected abstract void LogJobBegin(int initialPendingQueueCount);

		protected abstract void LogJobEnd(int initialPendingQueueCount, int mailboxesProcessedSuccessfullyCount, int mailboxesProcessedFailureCount, int mailboxesFailedToOpenStoreSessionCount, int mailboxesProcessedSeparatelyCount, int mailboxesRetriedCount);

		private void Finish()
		{
			if (this.loggedBegin)
			{
				ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceDebug((long)this.GetHashCode(), "{0}: finishing.  initialPendingQueueCount: {1}, processed successfully: {2}, failure count: {3}, failed to open store session: {4}, separately: {5},retried: {6}", new object[]
				{
					this,
					this.initialPendingQueueCount,
					this.mailboxesProcessedSuccessfullyCount,
					this.mailboxesProcessedFailureCount,
					this.mailboxesFailedToOpenStoreSessionCount,
					this.OnDemandMailboxCount,
					this.mailboxesRetriedCount
				});
				this.LogJobEnd(this.initialPendingQueueCount, this.mailboxesProcessedSuccessfullyCount, this.mailboxesProcessedFailureCount, this.mailboxesFailedToOpenStoreSessionCount, this.OnDemandMailboxCount, this.mailboxesRetriedCount);
				this.LogSkippedMailboxes();
			}
			else
			{
				this.StartTime = DateTime.UtcNow;
				this.EndTime = DateTime.UtcNow;
			}
			this.Finished = true;
		}

		public MailboxData GetNextMailbox()
		{
			MailboxData result;
			lock (this.instanceLock)
			{
				if (this.pendingQueue.Count <= 0)
				{
					result = null;
				}
				else
				{
					MailboxData mailboxData = this.pendingQueue[0];
					this.pendingQueue.RemoveAt(0);
					this.activeQueue.Add(mailboxData);
					result = mailboxData;
				}
			}
			return result;
		}

		internal void LogAIException(MailboxData mailbox, AIException e)
		{
			ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceError<TimeBasedDatabaseJob, string, AIException>((long)this.GetHashCode(), "{0}: Exception on mailbox {1}: {2}", this, (mailbox == null) ? "No Mailbox Present" : mailbox.DisplayName, e);
			base.LogEvent(AssistantsEventLogConstants.Tuple_TimeBasedAssistantFailed, null, new object[]
			{
				this.Assistant.Name,
				(mailbox == null) ? "No Mailbox Present" : mailbox.MailboxGuid.ToString(),
				e
			});
			AssistantsLog.LogErrorProcessingMailboxEvent(this.Assistant.NonLocalizedName, mailbox, e, this.DatabaseInfo.DatabaseName, this.StartTime.ToString("O"), (this is TimeBasedDatabaseWindowJob) ? MailboxSlaRequestType.Scheduled : MailboxSlaRequestType.OnDemand);
		}

		private void SetPendingQueue(List<MailboxData> queue)
		{
			this.pendingQueue = queue;
			this.Driver.IncrementNumberOfMailboxes(queue.Count);
		}

		private AssistantTaskContext ProcessMailbox(AssistantTaskContext context)
		{
			TimeBasedDatabaseJob.<>c__DisplayClassd CS$<>8__locals1 = new TimeBasedDatabaseJob.<>c__DisplayClassd();
			CS$<>8__locals1.context = context;
			CS$<>8__locals1.<>4__this = this;
			Guid mailboxGuid = CS$<>8__locals1.context.MailboxData.MailboxGuid;
			CS$<>8__locals1.nextContext = null;
			if (this.poisonControl.IsPoisonMailbox(mailboxGuid))
			{
				ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceDebug<TimeBasedDatabaseJob, string, int>((long)this.GetHashCode(), "{0}: Poison mailbox detected and skipped. Mailbox: {1}, crashCount: {2}", this, (CS$<>8__locals1.context.MailboxData is StoreMailboxData) ? ((StoreMailboxData)CS$<>8__locals1.context.MailboxData).DisplayName : string.Empty, this.poisonControl.GetCrashCount(mailboxGuid));
				if (Test.NotifyPoisonMailboxSkipped != null)
				{
					Test.NotifyPoisonMailboxSkipped(this.DatabaseInfo, mailboxGuid);
				}
				this.mailboxesProcessedFailureCount++;
				return null;
			}
			CS$<>8__locals1.kit = new EmergencyKit(mailboxGuid);
			this.poisonControl.PoisonCall(CS$<>8__locals1.kit, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ProcessMailbox>b__c)));
			return CS$<>8__locals1.nextContext;
		}

		private AssistantTaskContext ProcessMailboxUnderPoisonControl(AssistantTaskContext context, EmergencyKit kit)
		{
			AIException exception = null;
			AssistantTaskContext nextContext = null;
			try
			{
				base.CatchMeIfYouCan(delegate
				{
					AdminRpcMailboxData adminRpcMailboxData = context.MailboxData as AdminRpcMailboxData;
					if (adminRpcMailboxData != null)
					{
						nextContext = this.ProcessAdminRpcMailboxUnderPoisonControl(context, kit);
						return;
					}
					StoreMailboxData storeMailboxData = context.MailboxData as StoreMailboxData;
					if (storeMailboxData != null)
					{
						nextContext = this.ProcessStoreMailboxUnderPoisonControl(context, kit);
					}
				}, this.Assistant.NonLocalizedName);
			}
			catch (AIException ex)
			{
				this.LogAIException(context.MailboxData, ex);
				exception = ex;
			}
			this.PostProcessMailbox(exception, nextContext, context.MailboxData);
			return nextContext;
		}

		private AssistantTaskContext ProcessAdminRpcMailboxUnderPoisonControl(AssistantTaskContext context, EmergencyKit kit)
		{
			TimeBasedDatabaseJob.processMailboxTestHook.Value();
			AssistantTaskContext result = null;
			Guid activityId = (ActivityContext.ActivityId != null) ? ActivityContext.ActivityId.Value : Guid.Empty;
			if (context.Args == null)
			{
				context.Args = InvokeArgs.Create(null, this.Driver.TimePerTask, context.MailboxData);
			}
			AssistantsLog.LogStartProcessingMailboxEvent(activityId, this.Assistant as AssistantBase, context.MailboxData.MailboxGuid, context.MailboxData.DisplayName, this);
			try
			{
				kit.SetContext(this.Assistant, context.MailboxData);
				result = context.Step(context);
			}
			finally
			{
				kit.UnsetContext();
			}
			AssistantsLog.LogEndProcessingMailboxEvent(activityId, this.Assistant as AssistantBase, context.CustomDataToLog, context.MailboxData.MailboxGuid, context.MailboxData.DisplayName, this);
			return result;
		}

		private AssistantTaskContext ProcessStoreMailboxUnderPoisonControl(AssistantTaskContext context, EmergencyKit kit)
		{
			StoreMailboxData storeMailboxData = context.MailboxData as StoreMailboxData;
			AssistantTaskContext result = null;
			Guid activityId = (ActivityContext.ActivityId != null) ? ActivityContext.ActivityId.Value : Guid.Empty;
			base.TracePfd("PFD AIS {0} {1}: ProcessMailbox: {2}", new object[]
			{
				23127,
				this,
				storeMailboxData.DisplayName
			});
			AssistantsLog.LogStartProcessingMailboxEvent(activityId, this.Assistant as AssistantBase, storeMailboxData.MailboxGuid, storeMailboxData.DisplayName, this);
			bool flag = false;
			IMailboxFilter mailboxFilter = this.Driver.AssistantType as IMailboxFilter;
			if (mailboxFilter != null && mailboxFilter.MailboxType.Contains(MailboxType.InactiveMailbox))
			{
				flag = true;
				ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler.Enable();
			}
			try
			{
				result = this.ProcessStoreMailbox(context, kit);
			}
			finally
			{
				if (flag)
				{
					ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler.Disable();
				}
			}
			AssistantsLog.LogEndProcessingMailboxEvent(activityId, this.Assistant as AssistantBase, context.CustomDataToLog, storeMailboxData.MailboxGuid, storeMailboxData.DisplayName, this);
			return result;
		}

		private AssistantTaskContext ProcessStoreMailbox(AssistantTaskContext context, EmergencyKit kit)
		{
			TimeBasedDatabaseJob.processMailboxTestHook.Value();
			StoreMailboxData storeMailboxData = context.MailboxData as StoreMailboxData;
			AssistantTaskContext assistantTaskContext;
			using (StoreSession storeSession = this.OpenMailboxSession(storeMailboxData))
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				try
				{
					if (context.Args == null)
					{
						context.Args = InvokeArgs.Create(storeSession, this.Driver.TimePerTask, storeMailboxData);
					}
					kit.SetContext(this.Assistant, storeMailboxData);
					assistantTaskContext = context.Step(context);
				}
				finally
				{
					kit.UnsetContext();
					if (this.Driver.AssistantType.ControlDataPropertyDefinition != null && context.Args != null)
					{
						context.Args.StoreSession.Mailbox[this.Driver.AssistantType.ControlDataPropertyDefinition] = ControlData.Create(DateTime.UtcNow).ToByteArray();
						context.Args.StoreSession.Mailbox.Save();
					}
				}
				stopwatch.Stop();
				this.performanceCounters.AverageMailboxProcessingTime.IncrementBy(stopwatch.ElapsedTicks);
				this.performanceCounters.AverageMailboxProcessingTimeBase.Increment();
				if (assistantTaskContext == null)
				{
					this.performanceCounters.MailboxesProcessed.Increment();
				}
			}
			return assistantTaskContext;
		}

		private void PostProcessMailbox(AIException exception, AssistantTaskContext nextContext, MailboxData mailbox)
		{
			lock (this.instanceLock)
			{
				bool flag2 = this.Driver.Governor.ReportResult(exception);
				if (flag2)
				{
					if (nextContext == null)
					{
						this.FinalizeMailboxProcessing(mailbox, exception);
					}
				}
				else
				{
					this.AddForRetry(mailbox);
				}
			}
		}

		private void FinalizeMailboxProcessing(MailboxData mailbox, AIException e)
		{
			if (e != null)
			{
				lock (this.skippedMailboxesLock)
				{
					this.skippedMailboxes.Add(mailbox);
				}
				Interlocked.Increment(ref this.mailboxesProcessedFailureCount);
				return;
			}
			Interlocked.Increment(ref this.mailboxesProcessedSuccessfullyCount);
		}

		private void AddForRetry(MailboxData mailbox)
		{
			if (this.activeQueue.Contains(mailbox))
			{
				this.pendingQueue.Insert(0, mailbox);
				this.activeQueue.Remove(mailbox);
			}
			this.retry = true;
			Interlocked.Increment(ref this.mailboxesRetriedCount);
		}

		private StoreSession OpenMailboxSession(StoreMailboxData mailbox)
		{
			Guid activityId = (ActivityContext.ActivityId != null) ? ActivityContext.ActivityId.Value : Guid.Empty;
			AssistantBase assistant = this.Assistant as AssistantBase;
			string nonLocalizedName = this.Assistant.NonLocalizedName;
			Guid mailboxGuid = mailbox.MailboxGuid;
			StoreSession result;
			try
			{
				ExchangePrincipal exchangePrincipal;
				if (mailbox.TenantPartitionHint != null)
				{
					ADSessionSettings adSettings = ADSessionSettings.FromTenantPartitionHint(mailbox.TenantPartitionHint);
					exchangePrincipal = ExchangePrincipal.FromLocalServerMailboxGuid(adSettings, this.DatabaseInfo.Guid, mailbox.Guid);
				}
				else
				{
					exchangePrincipal = ExchangePrincipal.FromMailboxData(mailbox.Guid, this.DatabaseInfo.Guid, mailbox.OrganizationId ?? OrganizationId.ForestWideOrgId, Array<CultureInfo>.Empty);
				}
				if (mailbox.IsPublicFolderMailbox)
				{
					StoreSession storeSession = PublicFolderSession.OpenAsAdmin(null, exchangePrincipal, null, CultureInfo.InstalledUICulture, string.Format("{0};Action={1}", "Client=TBA", this.Assistant.GetType().Name), null);
					AssistantsLog.LogMailboxSucceedToOpenStoreSessionEvent(activityId, nonLocalizedName, assistant, mailboxGuid, mailbox.DisplayName, this);
					result = storeSession;
				}
				else
				{
					bool flag = false;
					MailboxSession mailbox2 = this.DatabaseInfo.GetMailbox(exchangePrincipal, ClientType.TimeBased, this.Assistant.GetType().Name);
					try
					{
						mailbox2.ReconstructExchangePrincipal();
						mailbox2.ExTimeZone = ExTimeZone.CurrentTimeZone;
						flag = true;
						AssistantsLog.LogMailboxSucceedToOpenStoreSessionEvent(activityId, nonLocalizedName, assistant, mailboxGuid, mailbox.DisplayName, this);
						result = mailbox2;
					}
					finally
					{
						if (!flag)
						{
							mailbox2.Dispose();
						}
					}
				}
			}
			catch (ObjectNotFoundException ex)
			{
				string text = "MailboxNotFound";
				string message = string.Format("{0}: {1}", this, text);
				string value = string.Format("{0}:{1}", text, mailbox.MailboxGuid);
				ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceError((long)this.GetHashCode(), message);
				AssistantsLog.LogMailboxFailedToOpenStoreSessionEvent(activityId, nonLocalizedName, assistant, ex, mailboxGuid, mailbox.DisplayName, this);
				throw new SkipException(new LocalizedString(value), ex);
			}
			catch (StorageTransientException ex2)
			{
				string message2 = string.Format("{0}: Could not open mailbox store session due to storage transient error: {1}", this, ex2.Message);
				ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceError((long)this.GetHashCode(), message2);
				AssistantsLog.LogMailboxFailedToOpenStoreSessionEvent(activityId, nonLocalizedName, assistant, ex2, mailboxGuid, mailbox.DisplayName, this);
				Interlocked.Increment(ref this.mailboxesFailedToOpenStoreSessionCount);
				throw;
			}
			catch (Exception ex3)
			{
				string message3 = string.Format("{0}: Could not open mailbox store session due to error: {1}", this, ex3.Message);
				ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceError((long)this.GetHashCode(), message3);
				AssistantsLog.LogMailboxFailedToOpenStoreSessionEvent(activityId, nonLocalizedName, assistant, ex3, mailboxGuid, mailbox.DisplayName, this);
				Interlocked.Increment(ref this.mailboxesFailedToOpenStoreSessionCount);
				throw;
			}
			return result;
		}

		private void LogSkippedMailboxes()
		{
			if (this.skippedMailboxes.Count <= 0)
			{
				return;
			}
			string newLine = Environment.NewLine;
			StringBuilder stringBuilder = new StringBuilder(Math.Min(20 * this.skippedMailboxes.Count, 16384));
			foreach (MailboxData mailboxData in this.skippedMailboxes)
			{
				if (stringBuilder.Length > 0 && stringBuilder.Length + newLine.Length + mailboxData.DisplayName.Length >= 16384)
				{
					ExTraceGlobals.TimeBasedDatabaseJobTracer.TraceDebug<TimeBasedDatabaseJob, int, int>((long)this.GetHashCode(), "{0}: Only logging the first {1} mailboxes out of {2} skipped mailboxes.", this, stringBuilder.Length, this.skippedMailboxes.Count);
					break;
				}
				stringBuilder.Append(newLine);
				stringBuilder.Append(mailboxData.DisplayName);
			}
			base.LogEvent(AssistantsEventLogConstants.Tuple_SkippedMailboxes, null, new object[]
			{
				this.Assistant.Name,
				this.skippedMailboxes.Count,
				this.DatabaseInfo.DisplayName,
				this.Chop(stringBuilder.ToString(), 16384)
			});
		}

		private string Chop(string str, int len)
		{
			if (str.Length > len)
			{
				return str.Substring(0, len);
			}
			return str;
		}

		internal void FinishIfNecessary()
		{
			lock (this.instanceLock)
			{
				if (!this.Finished && this.pendingQueue.Count == 0 && this.activeQueue.Count == 0)
				{
					this.Finish();
				}
			}
		}

		internal void RemoveFromActive(MailboxData mailbox)
		{
			lock (this.instanceLock)
			{
				if (this.activeQueue.Remove(mailbox))
				{
					this.Driver.DecrementNumberOfMailboxes();
				}
			}
		}

		internal static IDisposable SetProcessMailboxTestHook(Action delegateFunction)
		{
			return TimeBasedDatabaseJob.processMailboxTestHook.SetTestHook(delegateFunction);
		}

		private const int MaxCharsPerEventLog = 16384;

		private static Hookable<Action> processMailboxTestHook = Hookable<Action>.Create(true, delegate()
		{
		});

		protected List<MailboxData> pendingQueue;

		protected List<MailboxData> activeQueue = new List<MailboxData>();

		private string toString;

		private PerformanceCountersPerDatabaseInstance performanceCounters;

		private PoisonMailboxControl poisonControl;

		private bool loggedBegin;

		private readonly int initialPendingQueueCount;

		private int mailboxesProcessedSuccessfullyCount;

		private int mailboxesProcessedFailureCount;

		private int mailboxesFailedToOpenStoreSessionCount;

		private int mailboxesRetriedCount;

		private bool retry;

		private readonly List<MailboxData> skippedMailboxes = new List<MailboxData>();

		private readonly object skippedMailboxesLock = new object();

		private readonly object instanceLock = new object();
	}
}
