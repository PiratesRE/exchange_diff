using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Assistants.Diagnostics;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Assistants.Logging;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class TimeBasedDatabaseDriver : Base, IDisposable
	{
		internal TimeBasedDatabaseDriver(ThrottleGovernor parentGovernor, DatabaseInfo databaseInfo, ITimeBasedAssistantType timeBasedAssistantType, PoisonMailboxControl poisonControl, PerformanceCountersPerDatabaseInstance databaseCounters)
		{
			this.databaseInfo = databaseInfo;
			this.performanceCounters = databaseCounters;
			this.governor = new DatabaseGovernor("time based for '" + databaseInfo.DisplayName + "'", parentGovernor, new Throttle("TimeBasedDatabaseDriver", parentGovernor.Throttle.OpenThrottleValue, parentGovernor.Throttle));
			this.assistant = timeBasedAssistantType.CreateInstance(databaseInfo);
			if (this.assistant == null)
			{
				throw new ApplicationException(string.Format("Assistant failed to create instance, assistant type {0}", timeBasedAssistantType.NonLocalizedName));
			}
			this.poisonControl = poisonControl;
			this.assistantType = timeBasedAssistantType;
			this.assistantWorkloadState = TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.Enabled;
			this.windowJobHistory = new DiagnosticsHistoryQueue<DiagnosticsSummaryJobWindow>(100);
		}

		internal bool EnabledAndRunning
		{
			get
			{
				return this.assistantWorkloadState == TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.EnabledAndRunning;
			}
		}

		internal bool DisabledAndNotRunning
		{
			get
			{
				return this.assistantWorkloadState == TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.DisabledAndNotRunning;
			}
		}

		public ThrottleGovernor Governor
		{
			get
			{
				return this.governor;
			}
		}

		public Throttle Throttle
		{
			get
			{
				return this.governor.Throttle;
			}
		}

		public ITimeBasedAssistant Assistant
		{
			get
			{
				return this.assistant;
			}
		}

		public ITimeBasedAssistantType AssistantType
		{
			get
			{
				return this.assistantType;
			}
		}

		public DatabaseInfo DatabaseInfo
		{
			get
			{
				return this.databaseInfo;
			}
		}

		public PoisonMailboxControl PoisonControl
		{
			get
			{
				return this.poisonControl;
			}
		}

		public int NumberOfMailboxesInQueue
		{
			get
			{
				return this.numberOfMailboxesInQueue;
			}
		}

		public TimeSpan TimePerTask { get; private set; }

		public uint TotalMailboxesQueued
		{
			get
			{
				uint result;
				lock (this.instanceLock)
				{
					uint num = 0U;
					if (this.windowJob != null)
					{
						num += (uint)this.windowJob.MailboxesQueued;
					}
					foreach (TimeBasedDatabaseDemandJob timeBasedDatabaseDemandJob in this.demandJobs)
					{
						num += (uint)timeBasedDatabaseDemandJob.MailboxesQueued;
					}
					result = num;
				}
				return result;
			}
		}

		public IEnumerable<ResourceKey> ResourceDependencies
		{
			get
			{
				lock (this.instanceLock)
				{
					if (this.started)
					{
						return this.Assistant.GetResourceDependencies();
					}
				}
				return null;
			}
		}

		private bool WindowJobRunning
		{
			get
			{
				return this.windowJob != null && !this.windowJob.Finished;
			}
		}

		public void Dispose()
		{
			this.governor.Dispose();
			IDisposable disposable = this.Assistant as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			if (this.workerThreadsClear != null)
			{
				this.workerThreadsClear.Dispose();
				this.workerThreadsClear = null;
			}
		}

		public override string ToString()
		{
			return "TimeBasedDatabaseDriver for database '" + (this.databaseInfo.DisplayName ?? "<null>") + "', Assistant " + this.Assistant.GetType().Name;
		}

		public bool IsVariantConfigurationChanged()
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(new DatabaseSettingsContext(this.databaseInfo.Guid, null), null, null);
			bool enabled = snapshot.MailboxAssistants.GetObject<IMailboxAssistantSettings>(this.assistantType.Identifier, new object[0]).Enabled;
			ExTraceGlobals.TimeBasedDriverManagerTracer.TraceDebug((long)this.GetHashCode(), "{0}: Assistant {1} is enabled: {2}, on database {3}.", new object[]
			{
				this.ToString(),
				this.assistantType.Identifier,
				enabled,
				this.databaseInfo.Guid
			});
			bool flag = false;
			lock (this.instanceLock)
			{
				if (enabled)
				{
					if (this.assistantWorkloadState != TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.EnabledAndRunning)
					{
						flag = true;
					}
				}
				else if (this.assistantWorkloadState != TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.DisabledAndNotRunning)
				{
					flag = true;
				}
			}
			ExTraceGlobals.TimeBasedDriverManagerTracer.TraceDebug((long)this.GetHashCode(), "{0}: Assistant {1} needs Variant Configuration update: {2}, on database {3}.", new object[]
			{
				this.ToString(),
				this.assistantType.Identifier,
				flag,
				this.databaseInfo.Guid
			});
			return flag;
		}

		public bool IsAssistantEnabled()
		{
			bool flag;
			if (this.AssistantType.WorkCycle.Equals(TimeSpan.Zero) || this.AssistantType.WorkCycleCheckpoint.Equals(TimeSpan.Zero))
			{
				ExTraceGlobals.TimeBasedDriverManagerTracer.TraceDebug((long)this.GetHashCode(), "{0}: Assistant {1} disabled with work cycle period {2}, work cycle check point {3}", new object[]
				{
					this,
					this.AssistantType.Identifier,
					this.AssistantType.WorkCycle,
					this.AssistantType.WorkCycleCheckpoint
				});
				flag = false;
			}
			else
			{
				ExTraceGlobals.TimeBasedDriverManagerTracer.TraceDebug<TimeBasedDatabaseDriver, TimeBasedAssistantIdentifier>((long)this.GetHashCode(), "{0}: Assistant {1} enabled on server.", this, this.AssistantType.Identifier);
				flag = true;
			}
			if (flag)
			{
				VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(DatabaseSettingsContext.Get(this.databaseInfo.Guid), null, null);
				flag = snapshot.MailboxAssistants.GetObject<IMailboxAssistantSettings>(this.assistantType.Identifier, new object[0]).Enabled;
				ExTraceGlobals.TimeBasedDriverManagerTracer.TraceDebug((long)this.GetHashCode(), "{0}: Assistant {1} is enabled: {2}, on database {3}.", new object[]
				{
					this.ToString(),
					this.assistantType.Identifier,
					flag,
					this.databaseInfo.Guid
				});
			}
			lock (this.instanceLock)
			{
				if (flag)
				{
					if (this.assistantWorkloadState != TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.EnabledAndRunning)
					{
						this.assistantWorkloadState = TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.Enabled;
					}
				}
				else if (this.assistantWorkloadState != TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.DisabledAndNotRunning)
				{
					this.assistantWorkloadState = TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.Disabled;
				}
			}
			return flag;
		}

		public void Start()
		{
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Starting...", this);
			lock (this.instanceLock)
			{
				this.started = true;
				AssistantsLog.LogDatabaseStartEvent(this.Assistant as AssistantBase);
			}
			base.TracePfd("PFD AIS {0} {1}: Started", new object[]
			{
				30295,
				this
			});
		}

		public void RequestStop()
		{
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Requesting stop", this);
			AIBreadcrumbs.ShutdownTrail.Drop("Stopping time assistant: " + this.Assistant.NonLocalizedName);
			this.Assistant.OnShutdown();
			AIBreadcrumbs.ShutdownTrail.Drop("Finished stopping " + this.Assistant.NonLocalizedName);
			lock (this.instanceLock)
			{
				ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Stopping all jobs", this);
				this.StopAllJobs();
				this.started = false;
				AssistantsLog.LogDatabaseStopEvent(this.Assistant as AssistantBase);
			}
			base.TracePfd("PFD AIS {0} {1}: Requested Stop.", new object[]
			{
				19031,
				this
			});
		}

		public void WaitUntilStopped(TimeBasedAssistantController assistantController)
		{
			AIBreadcrumbs.ShutdownTrail.Drop("Waiting for stop on time assistant: " + this.Assistant.NonLocalizedName);
			ExTraceGlobals.TimeBasedAssistantControllerTracer.TraceDebug<TimeBasedAssistantController, TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Waiting stop of {1}", assistantController, this);
			this.workerThreadsClear.WaitOne();
			AIBreadcrumbs.ShutdownTrail.Drop("Done waiting for: " + this.Assistant.NonLocalizedName);
			lock (this.instanceLock)
			{
				this.Deinitialize();
			}
		}

		public abstract void RunNow(Guid mailboxGuid, string parameters);

		public void Halt()
		{
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Halt requested...", this);
			lock (this.instanceLock)
			{
				this.StopAllJobs();
			}
			base.LogEvent(AssistantsEventLogConstants.Tuple_TimeHalt, null, new object[]
			{
				this.Assistant.Name,
				this.databaseInfo.DisplayName
			});
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Halted.", this);
		}

		public void IncrementNumberOfMailboxes()
		{
			int arg = Interlocked.Increment(ref this.numberOfMailboxesInQueue);
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, int>((long)this.GetHashCode(), "{0}: Number of mailboxes incremented to {1}", this, arg);
		}

		public void IncrementNumberOfMailboxes(int numberOfMailboxes)
		{
			int arg = Interlocked.Add(ref this.numberOfMailboxesInQueue, numberOfMailboxes);
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, int, int>((long)this.GetHashCode(), "{0}: Number of mailboxes incremented by {1} to {2}", this, numberOfMailboxes, arg);
		}

		public void DecrementNumberOfMailboxes()
		{
			int arg = Interlocked.Decrement(ref this.numberOfMailboxesInQueue);
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, int>((long)this.GetHashCode(), "{0}: Number of mailboxes decremented to {1}", this, arg);
		}

		public void DecrementNumberOfMailboxes(int numberOfMailboxes)
		{
			int arg = Interlocked.Add(ref this.numberOfMailboxesInQueue, -numberOfMailboxes);
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, int, int>((long)this.GetHashCode(), "{0}: Number of mailboxes decremented by {1} to {2}", this, numberOfMailboxes, arg);
		}

		public bool HasTask()
		{
			TimeBasedDatabaseJob pendingJob;
			lock (this.instanceLock)
			{
				pendingJob = this.GetPendingJob(false);
			}
			return pendingJob != null && pendingJob.HasTask();
		}

		public DiagnosticsSummaryDatabase GetDatabaseDiagnosticsSummary()
		{
			return new DiagnosticsSummaryDatabase(this.EnabledAndRunning, this.startTime, this.GetWindowJobMailboxesSummary(), this.GetOnDemandJobMailboxesSummary());
		}

		public DiagnosticsSummaryJobWindow[] GetWindowJobHistory()
		{
			DiagnosticsSummaryJobWindow[] result;
			lock (this.instanceLock)
			{
				result = this.windowJobHistory.ToArray();
			}
			return result;
		}

		public List<Guid> GetMailboxGuidList(bool isActive)
		{
			List<Guid> list = new List<Guid>();
			lock (this.instanceLock)
			{
				if (this.windowJob != null)
				{
					list.AddRange(this.windowJob.GetMailboxGuidList(isActive));
				}
				foreach (TimeBasedDatabaseDemandJob timeBasedDatabaseDemandJob in this.demandJobs)
				{
					list.AddRange(timeBasedDatabaseDemandJob.GetMailboxGuidList(isActive));
				}
			}
			return list;
		}

		internal AssistantTaskContext ProcessNextTask(AssistantTaskContext context)
		{
			this.performanceCounters.NumberOfThreadsUsed.Increment();
			AssistantTaskContext nextContext = null;
			try
			{
				base.CatchMeIfYouCan(delegate
				{
					nextContext = this.ProcessOneMailbox(ref context);
				}, this.Assistant.NonLocalizedName);
			}
			catch (AIException e)
			{
				if (context != null && context.Job != null)
				{
					context.Job.LogAIException(context.MailboxData, e);
				}
				else
				{
					AssistantsLog.LogErrorProcessingMailboxEvent(this.Assistant.NonLocalizedName, (context == null) ? null : context.MailboxData, e, this.DatabaseInfo.DatabaseName, "", MailboxSlaRequestType.Unknown);
				}
			}
			catch (Exception e2)
			{
				AssistantsLog.LogErrorProcessingMailboxEvent(this.Assistant.NonLocalizedName, (context == null) ? null : context.MailboxData, e2, this.DatabaseInfo.DatabaseName, "", MailboxSlaRequestType.Unknown);
				throw;
			}
			finally
			{
				this.performanceCounters.NumberOfThreadsUsed.Decrement();
			}
			return nextContext;
		}

		internal void UpdateWorkCycle(TimeSpan workCyclePeriod)
		{
			bool flag = false;
			bool flag2 = false;
			lock (this.instanceLock)
			{
				if (this.assistantWorkloadState == TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.Enabled || this.assistantWorkloadState == TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.EnabledAndRunning)
				{
					flag = true;
					if (this.assistantWorkloadState == TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.Enabled)
					{
						this.windowJobHistory.Clear();
						this.startTime = DateTime.UtcNow;
						this.assistantWorkloadState = TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.EnabledAndRunning;
					}
					if (!this.inWorkCycle)
					{
						ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, bool, bool>((long)this.GetHashCode(), "{0}: Start Work Cycle. Started: {1}, Job Running: {2}.", this, this.started, this.WindowJobRunning);
						if (!this.started)
						{
							AssistantsLog.LogDriverNotStartedEvent(this.Assistant.NonLocalizedName, this.Assistant as AssistantBase);
						}
						else if (this.WindowJobRunning)
						{
							AssistantsLog.LogJobAlreadyRunningEvent(this.Assistant.NonLocalizedName);
						}
						else
						{
							flag2 = true;
						}
					}
				}
				else if (this.assistantWorkloadState == TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.Disabled)
				{
					this.windowJobHistory.Clear();
					this.startTime = DateTime.UtcNow;
					this.assistantWorkloadState = TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase.DisabledAndNotRunning;
				}
			}
			if (flag && !this.inWorkCycle)
			{
				if (flag2)
				{
					this.assistantType.OnWorkCycleStart(this.DatabaseInfo);
					this.TryStartWorkCycle(workCyclePeriod);
				}
				this.inWorkCycle = true;
			}
		}

		internal uint StopWorkCycle()
		{
			return this.StopWorkCycle(false);
		}

		internal uint StopWorkCycle(bool enqueueHistoryNow)
		{
			uint result = 0U;
			if (this.inWorkCycle)
			{
				lock (this.instanceLock)
				{
					ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Work Cycle is stopping.", this);
					result = this.StopWorkCycleJob(enqueueHistoryNow);
					ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Deinitializing driver.", this);
					this.Deinitialize();
				}
				this.inWorkCycle = false;
			}
			return result;
		}

		protected abstract List<MailboxData> GetMailboxesForCurrentWindow(out int totalMailboxOnDatabaseCount, out int notInterestingMailboxCount, out int filteredMailboxCount, out int failedFilteringCount);

		protected void RunNow(MailboxData mailboxData)
		{
			lock (this.instanceLock)
			{
				if (this.windowJob != null && this.windowJob.Remove(mailboxData))
				{
					ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, string>((long)this.GetHashCode(), "{0}: Removed mailbox {1} from windowJob so it can be added to a DemandJob.", this, mailboxData.DisplayName);
				}
				if (!this.demandJobs.Any((TimeBasedDatabaseDemandJob demandJob) => demandJob.GetMailboxGuidList(true).Contains(mailboxData.MailboxGuid) || demandJob.GetMailboxGuidList(false).Contains(mailboxData.MailboxGuid)))
				{
					this.demandJobs.Add(new TimeBasedDatabaseDemandJob(this, mailboxData, this.poisonControl, this.performanceCounters));
					ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, string, uint>((long)this.GetHashCode(), "{0}: Adding demand job with the following mailbox {1}, total queued: {2}", this, mailboxData.DisplayName, this.TotalMailboxesQueued);
				}
				else
				{
					ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, string, uint>((long)this.GetHashCode(), "{0}: Mailbox {1} has already been requested for a demand job, not queueing it again, total queued: {2}", this, mailboxData.DisplayName, this.TotalMailboxesQueued);
				}
			}
		}

		protected void TryStartWorkCycle(TimeSpan workCyclePeriod)
		{
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Starting a Work Cycle", this);
			List<MailboxData> mailboxes = null;
			int notInterestingCount = 0;
			int filteredCount = 0;
			int totalOnDatabaseMailboxCount = 0;
			int failedFilteringCount = 0;
			try
			{
				base.CatchMeIfYouCan(delegate
				{
					mailboxes = this.GetMailboxesForCurrentWindow(out totalOnDatabaseMailboxCount, out notInterestingCount, out filteredCount, out failedFilteringCount);
				}, this.Assistant.NonLocalizedName);
				if (mailboxes == null || mailboxes.Count <= 0)
				{
					ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceError<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: No mailboxes to process", this);
					lock (this.instanceLock)
					{
						this.windowJobHistory.Enqueue(new DiagnosticsSummaryJobWindow(totalOnDatabaseMailboxCount, 0, notInterestingCount, filteredCount, failedFilteringCount, 0, DateTime.UtcNow, DateTime.UtcNow, new DiagnosticsSummaryJob()));
					}
					return;
				}
				base.CatchMeIfYouCan(delegate
				{
					this.Assistant.OnWorkCycleCheckpoint();
				}, this.Assistant.NonLocalizedName);
			}
			catch (AIException ex)
			{
				ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceError<TimeBasedDatabaseDriver, AIException>((long)this.GetHashCode(), "{0}: Currently unable to list mailboxes: {1}", this, ex);
				base.LogEvent(AssistantsEventLogConstants.Tuple_TimeWindowBeginError, null, new object[]
				{
					this.Assistant.Name,
					this.databaseInfo.DisplayName,
					ex
				});
				return;
			}
			TimeBasedDatabaseWindowJob timeBasedDatabaseWindowJob = new TimeBasedDatabaseWindowJob(this, mailboxes, notInterestingCount, filteredCount, failedFilteringCount, totalOnDatabaseMailboxCount, this.poisonControl, this.performanceCounters);
			lock (this.instanceLock)
			{
				if (this.started && !this.WindowJobRunning)
				{
					this.RemoveWindowJobWithHistoryEntry();
					this.windowJob = timeBasedDatabaseWindowJob;
					double num = workCyclePeriod.TotalSeconds / (double)mailboxes.Count;
					if (num > TimeBasedDatabaseDriver.maxTimerIntervalInSeconds)
					{
						num = TimeBasedDatabaseDriver.maxTimerIntervalInSeconds;
					}
					this.TimePerTask = TimeSpan.FromSeconds(num);
					ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug((long)this.GetHashCode(), "{0}: Starting job with Work Cycle {1}, {2} mailboxes, default timer period of {3}.", new object[]
					{
						this,
						workCyclePeriod,
						mailboxes.Count,
						num
					});
				}
			}
		}

		protected uint StopWorkCycleJob()
		{
			return this.StopWorkCycleJob(false);
		}

		protected uint StopWorkCycleJob(bool enqueueHistoryNow)
		{
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Stoppping the window job", this);
			uint result = 0U;
			if (this.WindowJobRunning)
			{
				result = this.windowJob.RequestStop();
				if (enqueueHistoryNow)
				{
					this.windowJobHistory.Enqueue(this.windowJob.GetJobDiagnosticsSummary());
				}
				if (this.TotalMailboxesQueued == 0U && this.workersActive == 0)
				{
					ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Deinitializing the driver...", this);
					this.Deinitialize();
				}
			}
			return result;
		}

		protected void StopAllJobs()
		{
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: StopAllJobs", this);
			this.StopWorkCycleJob();
			foreach (TimeBasedDatabaseDemandJob timeBasedDatabaseDemandJob in this.demandJobs)
			{
				AIBreadcrumbs.ShutdownTrail.Drop("Stopping job: " + timeBasedDatabaseDemandJob);
				timeBasedDatabaseDemandJob.RequestStop();
				AIBreadcrumbs.ShutdownTrail.Drop("Finished stopping " + timeBasedDatabaseDemandJob);
			}
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, uint>((long)this.GetHashCode(), "{0}: Stopped all jobs. Total mailboxes queued: {1}", this, this.TotalMailboxesQueued);
		}

		protected void Deinitialize()
		{
			if (this.poisonControl != null)
			{
				this.poisonControl.Clear();
			}
		}

		protected AssistantTaskContext ProcessOneMailbox(ref AssistantTaskContext context)
		{
			lock (this.instanceLock)
			{
				if (context == null)
				{
					if (!this.started)
					{
						ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, string>((long)this.GetHashCode(), "{0}: Worker bailing (Not started) for assistant: {1}", this, this.Assistant.NonLocalizedName);
						AssistantsLog.LogNotStartedEvent(this.Assistant.NonLocalizedName, this.Assistant as AssistantBase);
						return null;
					}
					if (this.TotalMailboxesQueued == 0U)
					{
						ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, string>((long)this.GetHashCode(), "{0}: Worker bailing (Empty queue) for assistant: {1}", this, this.Assistant.NonLocalizedName);
						AssistantsLog.LogNoMailboxesPendingEvent(this.Assistant.NonLocalizedName);
						return null;
					}
				}
				if (this.workersActive++ == 0)
				{
					FastManualResetEvent fastManualResetEvent = this.workerThreadsClear;
					if (fastManualResetEvent != null)
					{
						fastManualResetEvent.Reset();
					}
				}
				ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, int, string>((long)this.GetHashCode(), "{0}: Worker started. Workers Active on this Driver: {1}, assistant: {2}", this, this.workersActive, this.Assistant.NonLocalizedName);
			}
			AssistantTaskContext assistantTaskContext = null;
			TimeBasedDatabaseJob timeBasedDatabaseJob = null;
			MailboxData mailboxData = null;
			try
			{
				if (context == null)
				{
					lock (this.instanceLock)
					{
						timeBasedDatabaseJob = this.GetPendingJob(true);
						ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, uint, string>((long)this.GetHashCode(), "{0}: Total Mailboxes Queued on this database: {1}, assistant: {2}", this, this.TotalMailboxesQueued, this.Assistant.NonLocalizedName);
					}
					if (timeBasedDatabaseJob != null)
					{
						mailboxData = timeBasedDatabaseJob.GetNextMailbox();
						if (mailboxData != null)
						{
							context = this.Assistant.InitializeContext(mailboxData, timeBasedDatabaseJob);
						}
					}
					else
					{
						AssistantsLog.LogNoJobsEvent(this.Assistant.NonLocalizedName);
					}
				}
				else
				{
					timeBasedDatabaseJob = context.Job;
					mailboxData = context.MailboxData;
				}
				if (context != null && context.Job != null)
				{
					assistantTaskContext = context.Job.ProcessNextMailbox(context);
				}
			}
			catch
			{
				lock (this.instanceLock)
				{
					if (--this.workersActive == 0)
					{
						ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, int, string>((long)this.GetHashCode(), "{0}: Worker exiting due to exception. Workers Active {1}, assistant: {2}", this, this.workersActive, this.Assistant.NonLocalizedName);
						this.workerThreadsClear.Set();
					}
				}
				throw;
			}
			finally
			{
				if (timeBasedDatabaseJob != null && mailboxData != null && (assistantTaskContext == null || context == null))
				{
					timeBasedDatabaseJob.RemoveFromActive(mailboxData);
					timeBasedDatabaseJob.FinishIfNecessary();
				}
			}
			lock (this.instanceLock)
			{
				try
				{
					ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, int, uint>((long)this.GetHashCode(), "{0}: Yielding thread. Workers Active: {1}, Remaining Mailboxes on this database: {2}", this, this.workersActive, this.TotalMailboxesQueued);
					if (context != null && context.Job != null && (context.Job.MailboxesQueued == 0 || context.Job.Finished))
					{
						TimeBasedDatabaseDemandJob timeBasedDatabaseDemandJob = context.Job as TimeBasedDatabaseDemandJob;
						if (timeBasedDatabaseDemandJob != null)
						{
							this.demandJobs.Remove(timeBasedDatabaseDemandJob);
							ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, int>((long)this.GetHashCode(), "{0}: Demand Job is done and has been removed. Remaining Demand Jobs: {1}", this, this.demandJobs.Count);
						}
						else
						{
							this.RemoveWindowJobWithHistoryEntry();
						}
					}
				}
				finally
				{
					if (--this.workersActive == 0)
					{
						ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver, int>((long)this.GetHashCode(), "{0}: Worker exiting. Workers Active {1}", this, this.workersActive);
						this.workerThreadsClear.Set();
					}
				}
			}
			return assistantTaskContext;
		}

		protected TimeBasedDatabaseJob GetPendingJob(bool cycleDemandJob = true)
		{
			if (this.demandJobs.Count > 0)
			{
				TimeBasedDatabaseDemandJob timeBasedDatabaseDemandJob = this.demandJobs[0];
				if (cycleDemandJob)
				{
					this.demandJobs.RemoveAt(0);
					this.demandJobs.Add(timeBasedDatabaseDemandJob);
				}
				return timeBasedDatabaseDemandJob;
			}
			return this.windowJob;
		}

		protected bool IsMailboxInDemandJob(MailboxData mailbox)
		{
			bool result;
			lock (this.instanceLock)
			{
				foreach (TimeBasedDatabaseDemandJob timeBasedDatabaseDemandJob in this.demandJobs)
				{
					if (timeBasedDatabaseDemandJob.IsMailboxActiveOrPending(mailbox))
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private void RemoveWindowJobWithHistoryEntry()
		{
			if (this.windowJob == null)
			{
				ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Called RemoveWindowJob, but it's already gone.", this);
				return;
			}
			if (!this.windowJob.Finished)
			{
				ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Called RemoveWindowJob, but it's not yet finished.", this);
				return;
			}
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedDatabaseDriver>((long)this.GetHashCode(), "{0}: Enqueue history and remove Window Job.", this);
			DiagnosticsSummaryJobWindow jobDiagnosticsSummary = this.windowJob.GetJobDiagnosticsSummary();
			DiagnosticsSummaryJobWindow lastQueuedElement = this.windowJobHistory.GetLastQueuedElement();
			if (lastQueuedElement == null || !lastQueuedElement.StartTime.Equals(jobDiagnosticsSummary.StartTime))
			{
				this.windowJobHistory.Enqueue(jobDiagnosticsSummary);
			}
			this.windowJob = null;
		}

		private DiagnosticsSummaryJobWindow GetWindowJobMailboxesSummary()
		{
			DiagnosticsSummaryJobWindow result;
			lock (this.instanceLock)
			{
				result = ((this.windowJob != null) ? this.windowJob.GetJobDiagnosticsSummary() : new DiagnosticsSummaryJobWindow());
			}
			return result;
		}

		private DiagnosticsSummaryJob GetOnDemandJobMailboxesSummary()
		{
			DiagnosticsSummaryJob diagnosticsSummaryJob = new DiagnosticsSummaryJob();
			lock (this.instanceLock)
			{
				foreach (TimeBasedDatabaseDemandJob timeBasedDatabaseDemandJob in this.demandJobs)
				{
					diagnosticsSummaryJob.AddMoreSummary(timeBasedDatabaseDemandJob.GetJobDiagnosticsSummary());
				}
			}
			return diagnosticsSummaryJob;
		}

		private const int WindowJobHistoryLimit = 100;

		private static readonly double maxTimerIntervalInSeconds = TimeSpan.FromDays(45.0).TotalSeconds;

		private readonly DatabaseInfo databaseInfo;

		private readonly ThrottleGovernor governor;

		private readonly ITimeBasedAssistant assistant;

		private bool started;

		private int workersActive;

		private int numberOfMailboxesInQueue;

		private TimeBasedDatabaseWindowJob windowJob;

		private List<TimeBasedDatabaseDemandJob> demandJobs = new List<TimeBasedDatabaseDemandJob>();

		private PoisonMailboxControl poisonControl;

		private FastManualResetEvent workerThreadsClear = new FastManualResetEvent(true);

		private bool inWorkCycle;

		private ITimeBasedAssistantType assistantType;

		private PerformanceCountersPerDatabaseInstance performanceCounters;

		private readonly object instanceLock = new object();

		private DateTime startTime;

		private readonly DiagnosticsHistoryQueue<DiagnosticsSummaryJobWindow> windowJobHistory;

		private TimeBasedDatabaseDriver.AssistantWorkloadStateOnDatabase assistantWorkloadState;

		private enum AssistantWorkloadStateOnDatabase
		{
			Disabled,
			DisabledAndNotRunning,
			Enabled,
			EnabledAndRunning
		}
	}
}
