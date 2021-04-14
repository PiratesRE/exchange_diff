using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncQueueManager
	{
		public SyncQueueManager(SyncLogSession syncLogSession, TimeSpan minimumDispatchWaitForFailedSync)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.databaseQueueManagers = new Dictionary<Guid, DatabaseQueueManager>(10);
			this.syncLogSession = syncLogSession;
			this.minimumDispatchWaitForFailedSync = minimumDispatchWaitForFailedSync;
		}

		public event EventHandler<SyncQueueEventArgs> SubscriptionAddedOrRemovedEvent
		{
			add
			{
				lock (this.eventHandlerRegisterUnregisterSyncRoot)
				{
					this.InternalSubscriptionAddedOrRemovedEvent += value;
				}
			}
			remove
			{
				lock (this.eventHandlerRegisterUnregisterSyncRoot)
				{
					this.InternalSubscriptionAddedOrRemovedEvent -= value;
				}
			}
		}

		public event EventHandler<SyncQueueEventArgs> SubscriptionEnqueuedOrDequeuedEvent
		{
			add
			{
				lock (this.eventHandlerRegisterUnregisterSyncRoot)
				{
					this.InternalSubscriptionEnqueuedOrDequeuedEvent += value;
				}
			}
			remove
			{
				lock (this.eventHandlerRegisterUnregisterSyncRoot)
				{
					this.InternalSubscriptionEnqueuedOrDequeuedEvent -= value;
				}
			}
		}

		public event EventHandler<SyncQueueEventArgs> ReportSyncQueueDispatchLagTimeEvent
		{
			add
			{
				lock (this.eventHandlerRegisterUnregisterSyncRoot)
				{
					this.InternalReportSyncQueueDispatchLagTimeEvent += value;
				}
			}
			remove
			{
				lock (this.eventHandlerRegisterUnregisterSyncRoot)
				{
					this.InternalReportSyncQueueDispatchLagTimeEvent -= value;
				}
			}
		}

		private event EventHandler<SyncQueueEventArgs> InternalReportSyncQueueDispatchLagTimeEvent;

		private event EventHandler<SyncQueueEventArgs> InternalSubscriptionEnqueuedOrDequeuedEvent;

		private event EventHandler<SyncQueueEventArgs> InternalSubscriptionAddedOrRemovedEvent;

		protected Dictionary<Guid, DatabaseQueueManager> DatabaseQueueManagers
		{
			get
			{
				return this.databaseQueueManagers;
			}
		}

		protected ReaderWriterLockSlim CollectionLock
		{
			get
			{
				return this.collectionLock;
			}
		}

		public Guid[] GetListOfDatabases()
		{
			return this.databaseQueueManagers.Keys.ToArray<Guid>();
		}

		public bool AddSubscription(ISubscriptionInformation subscriptionInformation)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformation", subscriptionInformation);
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(subscriptionInformation.MailboxGuid, subscriptionInformation.SubscriptionGuid);
			syncLogSession.LogVerbose((TSLID)1287UL, "SQM.OnSubscriptionAddedHandler: DatabaseGuid: {0}, AggType: {1}, SyncPhase: {2}, LastSuccessfulDispatchTime: {3}.", new object[]
			{
				subscriptionInformation.DatabaseGuid,
				subscriptionInformation.AggregationType,
				subscriptionInformation.SyncPhase,
				subscriptionInformation.LastSuccessfulDispatchTime
			});
			if (SyncPhase.Completed == subscriptionInformation.SyncPhase)
			{
				return false;
			}
			if (subscriptionInformation.Disabled)
			{
				syncLogSession.LogVerbose((TSLID)232UL, "SQM.OnSubscriptionAddedHandler: Skipping disabled subscription.", new object[0]);
				this.RemoveSubscription(subscriptionInformation.DatabaseGuid, subscriptionInformation.SubscriptionGuid);
				return false;
			}
			DatabaseQueueManager databaseQueueManager = this.EnsureDatabaseDqmIsLoaded(subscriptionInformation.DatabaseGuid);
			if (databaseQueueManager.Add(subscriptionInformation))
			{
				this.ClassifyAndEnqueue(subscriptionInformation);
				return true;
			}
			return false;
		}

		public void SyncCompleted(SubscriptionCompletionData subscriptionCompletionData)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionCompletionData", subscriptionCompletionData);
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(subscriptionCompletionData.MailboxGuid, subscriptionCompletionData.SubscriptionGuid);
			syncLogSession.LogDebugging((TSLID)1299UL, "SQM.SyncCompleted: DatabaseGuid:{0}, Disabled:{1}, Sync Succeeded:{2}", new object[]
			{
				subscriptionCompletionData.DatabaseGuid,
				subscriptionCompletionData.DisableSubscription,
				!subscriptionCompletionData.SyncFailed
			});
			if (!this.ContainsDatabase(subscriptionCompletionData.DatabaseGuid))
			{
				syncLogSession.LogDebugging((TSLID)1302UL, "SQM.SyncCompleted: DB not loaded in SQM DatabaseGuid: {0}", new object[]
				{
					subscriptionCompletionData.DatabaseGuid
				});
				return;
			}
			bool flag = false;
			bool flag2 = true;
			WorkType? workType = null;
			if (subscriptionCompletionData.DisableSubscription || subscriptionCompletionData.InvalidState || subscriptionCompletionData.SubscriptionDeleted)
			{
				flag = true;
				flag2 = false;
			}
			else
			{
				WorkType? dispatchedWorkType = subscriptionCompletionData.DispatchedWorkType;
				if (dispatchedWorkType != null)
				{
					WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(dispatchedWorkType.Value);
					flag2 = !workTypeDefinition.IsOneOff;
				}
				if (!subscriptionCompletionData.TryGetCurrentWorkType(syncLogSession, out workType))
				{
					flag = true;
					flag2 = false;
				}
			}
			if (flag)
			{
				this.RemoveSubscription(subscriptionCompletionData.DatabaseGuid, subscriptionCompletionData.SubscriptionGuid);
				return;
			}
			if (flag2)
			{
				this.CalculateNextDispatchTimeAndEnqueue(subscriptionCompletionData.DatabaseGuid, subscriptionCompletionData.MailboxGuid, subscriptionCompletionData.SubscriptionGuid, workType.Value, subscriptionCompletionData.SyncFailed, subscriptionCompletionData.LastSuccessfulDispatchTime);
				return;
			}
			syncLogSession.LogDebugging((TSLID)1307UL, "SQM.SyncCompleted: Not re-enqueueing subscription because it is WorkType {0}.", new object[]
			{
				subscriptionCompletionData.DispatchedWorkType
			});
		}

		public void SyncTimedOut(DispatchEntry dispatchEntry)
		{
			SyncUtilities.ThrowIfArgumentNull("dispatchEntry", dispatchEntry);
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(dispatchEntry.MiniSubscriptionInformation.MailboxGuid, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid);
			syncLogSession.LogDebugging((TSLID)1308UL, "SQM.SyncTimedOut", new object[0]);
			DatabaseQueueManager databaseQueueManager = this.GetDatabaseQueueManager(dispatchEntry.MiniSubscriptionInformation.DatabaseGuid);
			WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(dispatchEntry.WorkType);
			ExDateTime exDateTime = dispatchEntry.DispatchAttemptTime + workTypeDefinition.TimeTillSyncDue;
			if (exDateTime < ExDateTime.UtcNow)
			{
				exDateTime = ExDateTime.UtcNow;
			}
			databaseQueueManager.Enqueue(dispatchEntry.WorkType, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid, exDateTime);
		}

		public void RemoveSubscription(Guid databaseGuid, Guid subscriptionGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			this.syncLogSession.LogDebugging((TSLID)1291UL, "SQM.RemoveSubscription: DatabaseGuid: {0}, SubscriptionGuid: {1}", new object[]
			{
				databaseGuid,
				subscriptionGuid
			});
			if (!this.ContainsDatabase(databaseGuid))
			{
				this.syncLogSession.LogDebugging((TSLID)1309UL, "SQM.RemoveSubscription: DB not loaded in SQM DatabaseGuid: {0}", new object[]
				{
					databaseGuid
				});
				return;
			}
			DatabaseQueueManager databaseQueueManager = this.GetDatabaseQueueManager(databaseGuid);
			databaseQueueManager.Remove(subscriptionGuid);
		}

		public void OnSubscriptionAddedHandler(object sender, ISubscriptionInformation subscriptionInformation)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformation", subscriptionInformation);
			this.AddSubscription(subscriptionInformation);
		}

		public void OnSubscriptionRemovedHandler(object sender, ISubscriptionInformation subscriptionInformation)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformation", subscriptionInformation);
			this.RemoveSubscription(subscriptionInformation.DatabaseGuid, subscriptionInformation.SubscriptionGuid);
		}

		public void OnSubscriptionDisabledHandler(object sender, ISubscriptionInformation subscriptionInformation)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformation", subscriptionInformation);
			this.RemoveSubscription(subscriptionInformation.DatabaseGuid, subscriptionInformation.SubscriptionGuid);
		}

		public void OnSubscriptionSyncNowHandler(object sender, ISubscriptionInformation subscriptionInformation)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformation", subscriptionInformation);
			this.SyncNowForSubscription(subscriptionInformation, ExDateTime.UtcNow);
		}

		public void OnWorkTypeBasedSyncNowHandler(object workType, ISubscriptionInformation subscriptionInformation)
		{
			SyncUtilities.ThrowIfArgumentNull("workType", workType);
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformation", subscriptionInformation);
			this.WorkTypeBasedSyncNowForSubscription((WorkType)workType, subscriptionInformation, ExDateTime.UtcNow);
		}

		public IList<WorkType> GetDueWorkTypesByNextPollingTime(Guid databaseGuid, ExDateTime currentTime)
		{
			if (!this.ContainsDatabase(databaseGuid))
			{
				this.syncLogSession.LogDebugging((TSLID)1321UL, "SQM.GetWorkTypesByNextPollingTime: DB not in SQM", new object[]
				{
					databaseGuid
				});
				return null;
			}
			DatabaseQueueManager databaseQueueManager = this.GetDatabaseQueueManager(databaseGuid);
			return databaseQueueManager.GetDueWorkTypesByNextPollingTime(currentTime);
		}

		public bool TryGetWorkToDispatch(ExDateTime currentTime, Guid databaseGuid, WorkType workType, out DispatchEntry dispatchEntry)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			this.syncLogSession.LogDebugging((TSLID)1310UL, "SQM.TryGetWorkToDispatch: Database: {0}", new object[]
			{
				databaseGuid
			});
			dispatchEntry = null;
			if (!this.ContainsDatabase(databaseGuid))
			{
				this.syncLogSession.LogDebugging((TSLID)1311UL, "SQM.TryGetWorkToDispatch: DB not in SQM", new object[]
				{
					databaseGuid
				});
				return false;
			}
			DatabaseQueueManager databaseQueueManager = this.GetDatabaseQueueManager(databaseGuid);
			MiniSubscriptionInformation miniSubscriptionInformation;
			ExDateTime enqueueTime;
			if (databaseQueueManager.TryDequeue(currentTime, workType, out miniSubscriptionInformation, out enqueueTime))
			{
				dispatchEntry = new DispatchEntry(miniSubscriptionInformation, workType, currentTime, enqueueTime);
				SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(miniSubscriptionInformation.MailboxGuid, miniSubscriptionInformation.SubscriptionGuid);
				syncLogSession.LogDebugging((TSLID)1312UL, "SQM.TryGetWorkToDispatch: Dequeued subscription. WorkType {0}", new object[]
				{
					dispatchEntry.WorkType
				});
				return true;
			}
			return false;
		}

		public void DispatchAttemptCompleted(DispatchEntry dispatchEntry, DispatchResult dispatchResult)
		{
			SyncUtilities.ThrowIfArgumentNull("dispatchEntry", dispatchEntry);
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(dispatchEntry.MiniSubscriptionInformation.MailboxGuid, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid);
			syncLogSession.LogDebugging((TSLID)1313UL, "SQM.DispatchAttemptCompleted. Result:{0}", new object[]
			{
				dispatchResult
			});
			if (dispatchResult == DispatchResult.Success)
			{
				return;
			}
			if (DispatchResult.Success < (DispatchResult.InvalidSubscription & dispatchResult))
			{
				this.RemoveSubscription(dispatchEntry.MiniSubscriptionInformation.DatabaseGuid, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid);
				return;
			}
			if (dispatchResult == DispatchResult.PolicyInducedDeletion)
			{
				Guid databaseGuid = dispatchEntry.MiniSubscriptionInformation.DatabaseGuid;
				Guid mailboxGuid = dispatchEntry.MiniSubscriptionInformation.MailboxGuid;
				Guid subscriptionGuid = dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid;
				SubscriptionInformation subscriptionInformation;
				if (DataAccessLayer.TryUpdateCacheMessageSyncPhase(databaseGuid, mailboxGuid, subscriptionGuid, SyncPhase.Delete, out subscriptionInformation))
				{
					this.RemoveSubscription(databaseGuid, subscriptionGuid);
					this.AddSubscription(subscriptionInformation);
					return;
				}
				syncLogSession.LogDebugging((TSLID)332UL, "SQM PolicyInducedDelete redirect failed, databaseGuid:{0}, mailboxGuid:{0}, subscriptionGuid:{0}", new object[]
				{
					databaseGuid,
					mailboxGuid,
					subscriptionGuid
				});
				return;
			}
			else
			{
				bool flag = (DispatchResult.PermanentFailure & dispatchResult) == DispatchResult.PermanentFailure;
				bool flag2 = (DispatchResult.TransientFailure & dispatchResult) == DispatchResult.TransientFailure;
				bool flag3 = (DispatchResult.SubscriptionLoseItsTurnAtTransientFailure & dispatchResult) == DispatchResult.SubscriptionLoseItsTurnAtTransientFailure;
				if (flag || flag3)
				{
					DatabaseQueueManager databaseQueueManager = this.GetDatabaseQueueManager(dispatchEntry.MiniSubscriptionInformation.DatabaseGuid);
					WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(dispatchEntry.WorkType);
					databaseQueueManager.Enqueue(dispatchEntry.WorkType, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid, dispatchEntry.DispatchAttemptTime + workTypeDefinition.TimeTillSyncDue);
					return;
				}
				if (flag2)
				{
					DatabaseQueueManager databaseQueueManager2 = this.GetDatabaseQueueManager(dispatchEntry.MiniSubscriptionInformation.DatabaseGuid);
					databaseQueueManager2.EnqueueAtFront(dispatchEntry.WorkType, dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid);
					return;
				}
				throw new InvalidOperationException("Unsupported dispatch result: " + dispatchResult);
			}
		}

		internal bool IsEmpty()
		{
			this.collectionLock.EnterReadLock();
			bool result;
			try
			{
				foreach (DatabaseQueueManager databaseQueueManager in this.databaseQueueManagers.Values)
				{
					if (databaseQueueManager.SubscriptionCount > 0)
					{
						return false;
					}
				}
				result = true;
			}
			finally
			{
				this.collectionLock.ExitReadLock();
			}
			return result;
		}

		internal void OnDatabaseDismounted(object sender, OnDatabaseDismountedEventArgs databaseDismountedArgs)
		{
			Guid databaseGuid = databaseDismountedArgs.DatabaseGuid;
			this.syncLogSession.LogDebugging((TSLID)1332UL, "Removing DQM for {0}", new object[]
			{
				databaseGuid
			});
			DatabaseQueueManager databaseQueueManager = null;
			this.collectionLock.EnterReadLock();
			try
			{
				this.databaseQueueManagers.TryGetValue(databaseGuid, out databaseQueueManager);
			}
			finally
			{
				this.collectionLock.ExitReadLock();
			}
			if (databaseQueueManager != null)
			{
				databaseQueueManager.Clear();
			}
		}

		internal bool IsSubscriptionInDqm(Guid databaseId, Guid subscriptionId)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseId", databaseId);
			SyncUtilities.ThrowIfGuidEmpty("subscriptionId", subscriptionId);
			if (!this.ContainsDatabase(databaseId))
			{
				return false;
			}
			DatabaseQueueManager databaseQueueManager = this.GetDatabaseQueueManager(databaseId);
			HashSet<WorkType> hashSet;
			return databaseQueueManager.TryFindSubscription(subscriptionId, out hashSet);
		}

		internal void SyncNowForSubscription(ISubscriptionInformation subscriptionInformation, ExDateTime currentTime)
		{
			this.syncLogSession.LogDebugging((TSLID)1293UL, subscriptionInformation.SubscriptionGuid, subscriptionInformation.MailboxGuid, "SQM.InternalOnSubscriptionSyncNowHandler: DatabaseGuid: {0}, LastSuccessfulDispatchTime: {1}", new object[]
			{
				subscriptionInformation.DatabaseGuid,
				subscriptionInformation.LastSuccessfulDispatchTime
			});
			DatabaseQueueManager databaseQueueManager = this.EnsureDatabaseDqmIsLoaded(subscriptionInformation.DatabaseGuid);
			this.AddSubscription(subscriptionInformation);
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(subscriptionInformation.MailboxGuid, subscriptionInformation.SubscriptionGuid);
			if (subscriptionInformation.LastSyncCompletedTime == null)
			{
				syncLogSession.LogDebugging((TSLID)1314UL, "SQM.InternalOnSubscriptionSyncNowHandler. Skipping sync now as it is not allowed for subscriptions that have never sync'd before", new object[0]);
				return;
			}
			WorkType? workType = null;
			if (subscriptionInformation.AggregationType == AggregationType.Aggregation)
			{
				workType = new WorkType?(WorkType.AggregationSubscriptionSaved);
			}
			else if (subscriptionInformation.AggregationType == AggregationType.PeopleConnection)
			{
				workType = new WorkType?(WorkType.PeopleConnectionTriggered);
			}
			else if (subscriptionInformation.AggregationType == AggregationType.Migration && subscriptionInformation.SyncPhase == SyncPhase.Finalization)
			{
				workType = new WorkType?(WorkType.MigrationFinalization);
			}
			if (workType != null)
			{
				syncLogSession.LogDebugging((TSLID)1315UL, "SQM.InternalOnSubscriptionSyncNowHandler. {0}", new object[]
				{
					workType.Value
				});
				WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(workType.Value);
				databaseQueueManager.Enqueue(workType.Value, subscriptionInformation.SubscriptionGuid, currentTime + workTypeDefinition.TimeTillSyncDue);
				return;
			}
			syncLogSession.LogDebugging((TSLID)1316UL, "SQM.InternalOnSubscriptionSyncNowHandler. Skipping sync now as it is not mapped to any work type. AggregationType {0}, SubscriptionType {1}, SyncPhase: {2}", new object[]
			{
				subscriptionInformation.AggregationType,
				subscriptionInformation.SubscriptionType,
				subscriptionInformation.SyncPhase
			});
		}

		internal XElement GetDiagnosticInfo(SyncDiagnosticMode mode)
		{
			XElement xelement = new XElement("SyncQueueManager");
			this.collectionLock.EnterReadLock();
			try
			{
				if (mode == SyncDiagnosticMode.Verbose)
				{
					xelement.Add(new XElement("currentTime", ExDateTime.UtcNow.ToString("o")));
					xelement.Add(new XElement("totalDatabases", this.databaseQueueManagers.Count));
				}
				foreach (DatabaseQueueManager databaseQueueManager in this.databaseQueueManagers.Values)
				{
					xelement.Add(databaseQueueManager.GetDiagnosticInfo(mode));
				}
			}
			finally
			{
				this.collectionLock.ExitReadLock();
			}
			return xelement;
		}

		protected DatabaseQueueManager GetDatabaseQueueManager(Guid databaseGuid)
		{
			this.collectionLock.EnterReadLock();
			DatabaseQueueManager result;
			try
			{
				result = this.databaseQueueManagers[databaseGuid];
			}
			finally
			{
				this.collectionLock.ExitReadLock();
			}
			return result;
		}

		private void WorkTypeBasedSyncNowForSubscription(WorkType workType, ISubscriptionInformation subscriptionInformation, ExDateTime currentTime)
		{
			this.syncLogSession.LogDebugging((TSLID)478UL, subscriptionInformation.SubscriptionGuid, subscriptionInformation.MailboxGuid, "SQM.WorkTypeBasedSyncNowForSubscription: DatabaseGuid: {0}, LastSuccessfulDispatchTime: {1}, WorkType:{2}.", new object[]
			{
				subscriptionInformation.DatabaseGuid,
				subscriptionInformation.LastSuccessfulDispatchTime,
				workType
			});
			DatabaseQueueManager databaseQueueManager = this.EnsureDatabaseDqmIsLoaded(subscriptionInformation.DatabaseGuid);
			this.AddSubscription(subscriptionInformation);
			if (subscriptionInformation.SyncPhase == SyncPhase.Initial)
			{
				this.syncLogSession.LogDebugging((TSLID)479UL, subscriptionInformation.SubscriptionGuid, subscriptionInformation.MailboxGuid, "SQM.WorkTypeBasedSyncNowForSubscription. We don't allow sync nows while subscription is in initial sync mode.", new object[0]);
				return;
			}
			WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(workType);
			databaseQueueManager.Enqueue(workTypeDefinition.WorkType, subscriptionInformation.SubscriptionGuid, currentTime + workTypeDefinition.TimeTillSyncDue);
		}

		private void ClassifyAndEnqueue(ISubscriptionInformation subscriptionInformation)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformation", subscriptionInformation);
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(subscriptionInformation.MailboxGuid, subscriptionInformation.SubscriptionGuid);
			WorkType workType;
			try
			{
				workType = WorkTypeManager.ClassifyWorkTypeFromSubscriptionInformation(subscriptionInformation.AggregationType, subscriptionInformation.SyncPhase);
			}
			catch (NotSupportedException ex)
			{
				syncLogSession.LogError((TSLID)1317UL, "SQM.ClassifyAndEnqueue. Invalid Aggregation type ({0}) or Sync Phase ({1}): {2}", new object[]
				{
					subscriptionInformation.AggregationType,
					subscriptionInformation.SyncPhase,
					ex
				});
				this.RemoveSubscription(subscriptionInformation.DatabaseGuid, subscriptionInformation.SubscriptionGuid);
				return;
			}
			syncLogSession.LogDebugging((TSLID)1318UL, "SQM.ClassifyAndEnqueue. WorkType:{0}", new object[]
			{
				workType
			});
			this.CalculateNextDispatchTimeAndEnqueue(subscriptionInformation.DatabaseGuid, subscriptionInformation.MailboxGuid, subscriptionInformation.SubscriptionGuid, workType, false, subscriptionInformation.LastSuccessfulDispatchTime);
		}

		private void CalculateNextDispatchTimeAndEnqueue(Guid databaseGuid, Guid mailboxGuid, Guid subscriptionGuid, WorkType workType, bool syncFailed, ExDateTime? lastSuccessfulDispatchTime)
		{
			SyncLogSession syncLogSession = this.syncLogSession.OpenWithContext(mailboxGuid, subscriptionGuid);
			DatabaseQueueManager databaseQueueManager = this.GetDatabaseQueueManager(databaseGuid);
			if (WorkType.AggregationInitial == workType && lastSuccessfulDispatchTime == null)
			{
				syncLogSession.LogDebugging((TSLID)1319UL, "SQM.CalculateNextDispatchTimeAndEnqueue. Aggregation subscription never sync'd, enqueueing at front", new object[0]);
				databaseQueueManager.EnqueueAtFront(workType, subscriptionGuid);
				return;
			}
			ExDateTime utcNow = ExDateTime.UtcNow;
			WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(workType);
			ExDateTime exDateTime;
			if (workTypeDefinition.TimeTillSyncDue.TotalSeconds == 0.0)
			{
				exDateTime = utcNow;
			}
			else
			{
				ExDateTime d = utcNow;
				if (lastSuccessfulDispatchTime != null)
				{
					d = lastSuccessfulDispatchTime.Value;
				}
				exDateTime = d + workTypeDefinition.TimeTillSyncDue;
			}
			syncLogSession.LogDebugging((TSLID)1380UL, "SQM.CalculateNextDispatchTimeAndEnqueue. New Dispatch Time: {0}", new object[]
			{
				exDateTime
			});
			if (syncFailed && (exDateTime - utcNow).TotalSeconds < this.minimumDispatchWaitForFailedSync.TotalSeconds)
			{
				exDateTime = utcNow + this.minimumDispatchWaitForFailedSync;
				syncLogSession.LogDebugging((TSLID)1379UL, "SQM.CalculateNextDispatchTimeAndEnqueue. Subscription dispatch time is earlier than the minimum specified. New Dispatch Time: {0}", new object[]
				{
					exDateTime
				});
			}
			databaseQueueManager.Enqueue(workType, subscriptionGuid, exDateTime);
		}

		private bool ContainsDatabase(Guid databaseGuid)
		{
			this.collectionLock.EnterReadLock();
			bool result;
			try
			{
				result = this.databaseQueueManagers.ContainsKey(databaseGuid);
			}
			finally
			{
				this.collectionLock.ExitReadLock();
			}
			return result;
		}

		private DatabaseQueueManager EnsureDatabaseDqmIsLoaded(Guid databaseGuid)
		{
			this.collectionLock.EnterUpgradeableReadLock();
			DatabaseQueueManager databaseQueueManager;
			try
			{
				if (!this.databaseQueueManagers.ContainsKey(databaseGuid))
				{
					databaseQueueManager = new DatabaseQueueManager(databaseGuid, this.syncLogSession);
					databaseQueueManager.SubscriptionAddedOrRemovedEvent += this.OnSubscriptionAddedOrRemovedEvent;
					databaseQueueManager.SubscriptionEnqueuedOrDequeuedEvent += this.OnSubscriptionSyncEnqueuedOrDequeuedEvent;
					this.collectionLock.EnterWriteLock();
					try
					{
						this.databaseQueueManagers.Add(databaseGuid, databaseQueueManager);
						goto IL_7D;
					}
					finally
					{
						this.collectionLock.ExitWriteLock();
					}
				}
				databaseQueueManager = this.databaseQueueManagers[databaseGuid];
				IL_7D:;
			}
			finally
			{
				this.collectionLock.ExitUpgradeableReadLock();
			}
			return databaseQueueManager;
		}

		private void RaiseReportSyncQueueDispatchLagTimeEvent(TimeSpan dispatchLagTime)
		{
			if (this.InternalReportSyncQueueDispatchLagTimeEvent != null)
			{
				this.InternalReportSyncQueueDispatchLagTimeEvent(this, SyncQueueEventArgs.CreateReportSyncQueueDispatchLagTimeEventArgs(dispatchLagTime));
			}
		}

		private void OnSubscriptionAddedOrRemovedEvent(object sender, SyncQueueEventArgs e)
		{
			if (this.InternalSubscriptionAddedOrRemovedEvent != null)
			{
				this.InternalSubscriptionAddedOrRemovedEvent(sender, e);
			}
		}

		private void OnSubscriptionSyncEnqueuedOrDequeuedEvent(object sender, SyncQueueEventArgs e)
		{
			if (this.InternalSubscriptionEnqueuedOrDequeuedEvent != null)
			{
				this.InternalSubscriptionEnqueuedOrDequeuedEvent(sender, e);
			}
		}

		private const int DefaultNumberOfDatabasesPerMailboxServer = 10;

		private readonly object eventHandlerRegisterUnregisterSyncRoot = new object();

		private readonly Dictionary<Guid, DatabaseQueueManager> databaseQueueManagers;

		private SyncLogSession syncLogSession;

		private ReaderWriterLockSlim collectionLock = new ReaderWriterLockSlim();

		private TimeSpan minimumDispatchWaitForFailedSync;
	}
}
