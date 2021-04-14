using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog;
using Microsoft.Exchange.Transport.Sync.Manager.Throttling;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DispatchManager : DisposeTrackableBase, IHealthLogDispatchEntryReporter
	{
		public DispatchManager(SyncLogSession syncLogSession, bool isDispatchingEnabled, TimeSpan primingDispatchTime, TimeSpan minimumDispatchWaitForFailedSync, TimeSpan dispatchOutageThreshold, Action<EventLogEntry> eventLoggerDelegate, ISyncHealthLog syncHealthLogger, ISyncManagerConfiguration configuration)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.syncLogSession = syncLogSession;
			this.isDispatchingEnabled = isDispatchingEnabled;
			this.syncQueueManager = this.CreateSyncQueueManager(minimumDispatchWaitForFailedSync);
			this.dispatchEntryManager = this.CreateDispatchEntryManager(syncHealthLogger, configuration);
			this.dispatchEntryManager.EntryExpiredEvent += this.OnSyncTimedOut;
			this.dbPicker = this.CreateDatabasePicker(configuration, this.syncLogSession, this.dispatchEntryManager, this, this.syncQueueManager);
			this.dispatchWorkChecker = this.CreateDispatchWorkChecker(configuration);
			this.dispatcher = this.CreateDispatcher();
			this.dispatchDriver = this.CreateDispatchDriver(primingDispatchTime);
			this.dispatchDriver.PrimingEvent += this.OnPrimingEventHandler;
		}

		internal SyncQueueManager SyncQueueManager
		{
			get
			{
				return this.syncQueueManager;
			}
		}

		public void OnSubscriptionSyncCompletedHandler(object sender, OnSyncCompletedEventArgs eventArgs)
		{
			SyncUtilities.ThrowIfArgumentNull("eventArgs", eventArgs);
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.SyncCompletedCallback), eventArgs);
		}

		public virtual void SyncCompleted(SubscriptionCompletionData subscriptionCompletionData)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionCompletionData", subscriptionCompletionData);
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(subscriptionCompletionData.MailboxGuid, subscriptionCompletionData.SubscriptionGuid);
			DispatchEntry dispatchEntry;
			if (!this.dispatchEntryManager.TryRemoveDispatchedEntry(subscriptionCompletionData.SubscriptionGuid, out dispatchEntry))
			{
				syncLogSession.LogDebugging((TSLID)195UL, "DM.SyncCompleted: Subscription not in list of dispatched subscriptions", new object[0]);
			}
			else
			{
				subscriptionCompletionData.DispatchedWorkType = new WorkType?(dispatchEntry.WorkType);
			}
			this.syncQueueManager.SyncCompleted(subscriptionCompletionData);
			WorkType? dispatchedWorkType = subscriptionCompletionData.DispatchedWorkType;
			if (dispatchedWorkType == null)
			{
				subscriptionCompletionData.TryGetCurrentWorkType(syncLogSession, out dispatchedWorkType);
			}
			Guid databaseGuid;
			IList<WorkType> workTypes;
			if (this.dbPicker.TryGetNextDatabaseToDispatchFrom(DispatchTrigger.Completion, out databaseGuid, out workTypes))
			{
				this.DispatchSubscriptionForDatabase(databaseGuid, workTypes, DispatchTrigger.Completion, dispatchedWorkType);
			}
		}

		public void ReportDispatchAttempt(DispatchEntry dispatchEntry, DispatchTrigger dispatchTrigger, WorkType? workType, DispatchResult dispatchResult, ISubscriptionInformation subscriptionInformation, ExDateTime? lastDispatchTime)
		{
			Guid guid = Guid.Empty;
			Guid guid2 = Guid.Empty;
			Guid guid3 = Guid.Empty;
			Guid guid4 = Guid.Empty;
			string incomingServerName = string.Empty;
			string dispatchedTo = string.Empty;
			string subscriptionType = string.Empty;
			string aggregationType = string.Empty;
			int num = 0;
			if (dispatchEntry != null)
			{
				guid2 = dispatchEntry.MiniSubscriptionInformation.MailboxGuid;
				guid3 = dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid;
				guid4 = dispatchEntry.MiniSubscriptionInformation.DatabaseGuid;
				if (subscriptionInformation != null)
				{
					guid = subscriptionInformation.TenantGuid;
					subscriptionType = subscriptionInformation.SubscriptionType.ToString();
					aggregationType = subscriptionInformation.AggregationType.ToString();
					incomingServerName = subscriptionInformation.IncomingServerName;
					dispatchedTo = ((subscriptionInformation.HubServerDispatched == null) ? string.Empty : subscriptionInformation.HubServerDispatched);
					if (lastDispatchTime != null)
					{
						WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(dispatchEntry.WorkType);
						ExDateTime dt;
						if (workTypeDefinition.TimeTillSyncDue.TotalSeconds == 0.0)
						{
							dt = dispatchEntry.DispatchEnqueuedTime;
						}
						else
						{
							dt = lastDispatchTime.Value;
						}
						TimeSpan t = dispatchEntry.DispatchAttemptTime - dt;
						double totalSeconds = (t - workTypeDefinition.TimeTillSyncDue).TotalSeconds;
						if (totalSeconds >= 2147483647.0)
						{
							num = int.MaxValue;
						}
						else if (totalSeconds <= -2147483648.0)
						{
							num = int.MinValue;
						}
						else
						{
							num = Convert.ToInt32(totalSeconds);
						}
					}
				}
			}
			bool beyondSyncPollingFrequency = num > 0;
			this.InternalReportSubscriptionDispatch(Environment.MachineName, guid.ToString(), guid2.ToString(), guid3.ToString(), incomingServerName, subscriptionType, aggregationType, dispatchedTo, DispatchResult.Success == dispatchResult, DispatchResult.Success < (DispatchResult.PermanentFailure & dispatchResult), DispatchResult.Success < (DispatchResult.TransientFailure & dispatchResult), dispatchResult.ToString(), beyondSyncPollingFrequency, num, (workType != null) ? workType.ToString() : string.Empty, dispatchTrigger.ToString(), guid4.ToString());
		}

		internal void StopActiveDispatching()
		{
			this.dispatchDriver.PrimingEvent -= this.OnPrimingEventHandler;
		}

		internal XElement GetDiagnosticInfo(SyncDiagnosticMode mode)
		{
			XElement xelement = new XElement("DispatchManager");
			xelement.Add(new XElement("lastDequeueAttemptTime", (this.lastDequeueAttempt != null) ? this.lastDequeueAttempt.Value.ToString("o") : string.Empty));
			xelement.Add(new XElement("lastPrimerStartTime", (this.lastPrimerStartTime != null) ? this.lastPrimerStartTime.Value.ToString("o") : string.Empty));
			if (mode == SyncDiagnosticMode.Verbose)
			{
				this.dispatchDriver.AddDiagnosticInfoTo(xelement);
				this.dbPicker.AddDiagnosticInfoTo(xelement);
			}
			xelement.Add(this.syncQueueManager.GetDiagnosticInfo(mode));
			xelement.Add(this.dispatchEntryManager.GetDiagnosticInfo(mode));
			xelement.Add(this.dispatcher.GetDiagnosticInfo(mode));
			return xelement;
		}

		protected virtual ExDateTime GetCurrentTime()
		{
			return ExDateTime.UtcNow;
		}

		protected virtual void InternalReportSubscriptionDispatch(string mailboxServerName, string tenantGuid, string userMailboxGuid, string subscriptionGuid, string incomingServerName, string subscriptionType, string aggregationType, string dispatchedTo, bool successful, bool permanentError, bool transientError, string dispatchError, bool beyondSyncPollingFrequency, int secondsBeyondPollingFrequency, string workType, string dispatchTrigger, string databaseGuid)
		{
			DataAccessLayer.ReportSubscriptionDispatch(mailboxServerName, tenantGuid, userMailboxGuid, subscriptionGuid, incomingServerName, subscriptionType, aggregationType, dispatchedTo, successful, permanentError, transientError, dispatchError, beyondSyncPollingFrequency, secondsBeyondPollingFrequency, workType, dispatchTrigger, databaseGuid);
		}

		protected virtual SyncQueueManager CreateSyncQueueManager(TimeSpan minimumDispatchWaitForFailedSync)
		{
			return new SyncQueueManager(this.syncLogSession, minimumDispatchWaitForFailedSync);
		}

		protected virtual DispatchWorkChecker CreateDispatchWorkChecker(ISyncManagerConfiguration syncManagerConfiguration)
		{
			return new DispatchWorkChecker(this.syncLogSession, this.dispatchEntryManager, syncManagerConfiguration, this);
		}

		protected virtual IDispatcher CreateDispatcher()
		{
			return new SubscriptionDispatcher();
		}

		protected virtual IDispatchDriver CreateDispatchDriver(TimeSpan primingDispatchTime)
		{
			return new PrimingDispatchDriver(this.syncLogSession, primingDispatchTime);
		}

		protected void DisabledExpiration()
		{
			this.dispatchEntryManager.DisabledExpiration();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.dispatchDriver != null)
				{
					IDisposable disposable = this.dispatchDriver as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					this.dispatchDriver = null;
				}
				if (this.dispatchEntryManager != null)
				{
					IDisposable disposable2 = this.dispatchEntryManager as IDisposable;
					if (disposable2 != null)
					{
						disposable2.Dispose();
					}
					this.dispatchEntryManager = null;
				}
				this.dispatcher.Shutdown();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DispatchManager>(this);
		}

		protected virtual IDispatchEntryManager CreateDispatchEntryManager(ISyncHealthLog syncHealthLogger, ISyncManagerConfiguration configuration)
		{
			return new DispatchEntryManager(this.syncLogSession, syncHealthLogger, configuration);
		}

		protected virtual DatabasePicker CreateDatabasePicker(ISyncManagerConfiguration configuration, SyncLogSession syncLogSession, IDispatchEntryManager dispatchEntryManager, IHealthLogDispatchEntryReporter healthLogDispatchEntryReporter, SyncQueueManager syncQueueManager)
		{
			return new DatabasePicker(configuration, syncLogSession, dispatchEntryManager, healthLogDispatchEntryReporter, syncQueueManager);
		}

		protected virtual DispatchResult HandSubscriptionToDispatcher(DispatchEntry dispatchEntry, ISubscriptionInformation subscriptionInformation)
		{
			return this.dispatcher.DispatchSubscription(dispatchEntry, subscriptionInformation);
		}

		protected virtual DispatchResult TryGetSubscriptionInformation(DispatchEntry dispatchEntry, out ISubscriptionInformation subscriptionInformation)
		{
			bool flag = false;
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(dispatchEntry.MiniSubscriptionInformation.MailboxGuid, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid);
			subscriptionInformation = null;
			SubscriptionInformation subscriptionInformation2;
			if (!DataAccessLayer.TryReadSubscriptionInformation(dispatchEntry.MiniSubscriptionInformation.DatabaseGuid, dispatchEntry.MiniSubscriptionInformation.MailboxGuid, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid, out subscriptionInformation2, out flag))
			{
				return DispatchResult.TransientFailureReadingCache;
			}
			if (subscriptionInformation2 == null)
			{
				syncLogSession.LogInformation((TSLID)353UL, "DM: Subscription no longer exists in cache.", new object[0]);
				return DispatchResult.SubscriptionCacheMessageDoesNotExist;
			}
			if (subscriptionInformation2.Disabled)
			{
				syncLogSession.LogInformation((TSLID)354UL, "DM: Subscription is disabled.", new object[0]);
				subscriptionInformation = null;
				return DispatchResult.SubscriptionDisabled;
			}
			subscriptionInformation = subscriptionInformation2;
			return DispatchResult.Success;
		}

		private void DispatchSubscriptionForDatabase(Guid databaseGuid, IList<WorkType> workTypes, DispatchTrigger dispatchTrigger, WorkType? completedWorkType)
		{
			this.syncLogSession.LogDebugging((TSLID)196UL, "DM.DispatchSubscriptionForDatabase: databaseGuid: {0}, dispatchTrigger: {1}, completedWorkType: {2}.", new object[]
			{
				databaseGuid,
				dispatchTrigger,
				(completedWorkType != null) ? completedWorkType.Value.ToString() : string.Empty
			});
			if (!this.isDispatchingEnabled)
			{
				this.syncLogSession.LogVerbose((TSLID)1363UL, "DM.DispatchSubscriptionForDatabase. Not dispatching subcription. Dispatching is disabled", new object[0]);
				return;
			}
			DispatchEntry dispatchEntry = null;
			lock (this.dispatchEntryManager)
			{
				foreach (WorkType workType in workTypes)
				{
					if (!this.dispatchEntryManager.HasBudget(workType))
					{
						this.syncLogSession.LogVerbose((TSLID)496UL, "DM.DispatchSubscriptionForDatabase: Work type {0} is out of budget.", new object[]
						{
							workType
						});
					}
					else if (this.TryGetSubscriptionForDatabaseAndWorkType(databaseGuid, workType, dispatchTrigger, completedWorkType, out dispatchEntry))
					{
						break;
					}
				}
				if (dispatchEntry == null)
				{
					foreach (WorkType workType2 in workTypes)
					{
						if (this.TryGetSubscriptionForDatabaseAndWorkType(databaseGuid, workType2, dispatchTrigger, completedWorkType, out dispatchEntry))
						{
							break;
						}
					}
				}
			}
			if (dispatchEntry != null)
			{
				this.DispatchSubscription(dispatchEntry, dispatchTrigger);
				return;
			}
			this.syncLogSession.LogDebugging((TSLID)335UL, "DispatchSubscriptionForDatabase: No subscription available for dispatch", new object[0]);
		}

		private bool TryGetSubscriptionForDatabaseAndWorkType(Guid databaseGuid, WorkType workType, DispatchTrigger dispatchTrigger, WorkType? completedWorkType, out DispatchEntry dispatchEntry)
		{
			dispatchEntry = null;
			if (!this.dispatchWorkChecker.CanAttemptDispatchForWorkType(dispatchTrigger, workType, completedWorkType))
			{
				return false;
			}
			this.lastDequeueAttempt = new ExDateTime?(this.GetCurrentTime());
			if (!this.syncQueueManager.TryGetWorkToDispatch(this.GetCurrentTime(), databaseGuid, workType, out dispatchEntry))
			{
				this.syncLogSession.LogDebugging((TSLID)1320UL, "DM.TryDispatchSubscriptionForDatabaseAndWorkType: No work to dispatch", new object[0]);
				return false;
			}
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(dispatchEntry.MiniSubscriptionInformation.MailboxGuid, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid);
			DispatchResult? dispatchResult;
			if (!this.dispatchWorkChecker.CanAttemptDispatchForSubscription(dispatchEntry, out dispatchResult))
			{
				this.ReportDispatchAttempt(dispatchEntry, dispatchTrigger, new WorkType?(workType), dispatchResult.Value, null, null);
				WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(workType);
				if (!workTypeDefinition.IsOneOff)
				{
					this.syncQueueManager.DispatchAttemptCompleted(dispatchEntry, dispatchResult.Value);
				}
				else
				{
					syncLogSession.LogDebugging((TSLID)215UL, "DM.TryDispatchSubscriptionForDatabaseAndWorkType: Not reenqueueing for dispatch one-off worktype : {0}", new object[]
					{
						workType
					});
				}
				dispatchEntry = null;
				return false;
			}
			this.dispatchEntryManager.Add(dispatchEntry);
			return true;
		}

		private void DispatchSubscription(DispatchEntry dispatchEntry, DispatchTrigger dispatchTrigger)
		{
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(dispatchEntry.MiniSubscriptionInformation.MailboxGuid, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid);
			ISubscriptionInformation subscriptionInformation;
			DispatchResult dispatchResult = this.TryGetSubscriptionInformation(dispatchEntry, out subscriptionInformation);
			ExDateTime? lastDispatchTime = null;
			if (dispatchResult == DispatchResult.Success)
			{
				bool flag = false;
				lastDispatchTime = subscriptionInformation.LastSuccessfulDispatchTime;
				AggregationSubscription subscription = null;
				if (subscriptionInformation.SupportsSerialization && subscriptionInformation.SerializedSubscription.TryDeserializeSubscription(out subscription))
				{
					flag = MrsAdapter.UpdateAndCheckMrsJob(syncLogSession, subscription, dispatchEntry.MiniSubscriptionInformation.DatabaseGuid, dispatchEntry.MiniSubscriptionInformation.ExternalDirectoryOrgId);
				}
				if (flag)
				{
					return;
				}
				dispatchResult = this.HandSubscriptionToDispatcher(dispatchEntry, subscriptionInformation);
			}
			syncLogSession.LogDebugging((TSLID)342UL, "DM.DispatchSubscription. Result: {0}", new object[]
			{
				dispatchResult
			});
			this.syncQueueManager.DispatchAttemptCompleted(dispatchEntry, dispatchResult);
			DispatchResult dispatchResult2 = dispatchResult;
			if (dispatchResult2 == DispatchResult.Success)
			{
				this.dispatchEntryManager.MarkDispatchSuccess(dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid);
			}
			else
			{
				this.dispatchEntryManager.RemoveDispatchAttempt(dispatchEntry.MiniSubscriptionInformation.DatabaseGuid, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid);
			}
			this.dbPicker.MarkDispatchCompleted(dispatchEntry.MiniSubscriptionInformation.DatabaseGuid, dispatchResult);
			this.ReportDispatchAttempt(dispatchEntry, dispatchTrigger, new WorkType?(dispatchEntry.WorkType), dispatchResult, subscriptionInformation, lastDispatchTime);
		}

		private void SyncCompletedCallback(object state)
		{
			OnSyncCompletedEventArgs onSyncCompletedEventArgs = (OnSyncCompletedEventArgs)state;
			this.SyncCompleted(onSyncCompletedEventArgs.SubscriptionCompletionData);
		}

		private void OnPrimingEventHandler(object sender, EventArgs args)
		{
			Guid databaseGuid;
			IList<WorkType> workTypes;
			if (this.dbPicker.TryGetNextDatabaseToDispatchFrom(DispatchTrigger.Primer, out databaseGuid, out workTypes))
			{
				this.DispatchSubscriptionForDatabase(databaseGuid, workTypes, DispatchTrigger.Primer, null);
			}
		}

		private void OnSyncTimedOut(object sender, DispatchEntry dispatchEntry)
		{
			SyncUtilities.ThrowIfArgumentNull("dispatchEntry", dispatchEntry);
			this.syncQueueManager.SyncTimedOut(dispatchEntry);
		}

		private SyncQueueManager syncQueueManager;

		private IDispatchEntryManager dispatchEntryManager;

		private IDispatcher dispatcher;

		private IDispatchDriver dispatchDriver;

		private SyncLogSession syncLogSession;

		private DatabasePicker dbPicker;

		private ExDateTime? lastDequeueAttempt = null;

		private ExDateTime? lastPrimerStartTime = null;

		private DispatchWorkChecker dispatchWorkChecker;

		private bool isDispatchingEnabled;
	}
}
