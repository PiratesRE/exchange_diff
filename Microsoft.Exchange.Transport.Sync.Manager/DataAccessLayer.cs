using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Manager.Throttling;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DataAccessLayer
	{
		internal static event EventHandler<SubscriptionInformation> OnSubscriptionAdded
		{
			add
			{
				DataAccessLayer.OnSubscriptionAddedEvent += value;
			}
			remove
			{
				DataAccessLayer.OnSubscriptionAddedEvent -= value;
			}
		}

		internal static event EventHandler<SubscriptionInformation> OnSubscriptionRemoved
		{
			add
			{
				DataAccessLayer.OnSubscriptionRemovedEvent += value;
			}
			remove
			{
				DataAccessLayer.OnSubscriptionRemovedEvent -= value;
			}
		}

		internal static event EventHandler<SubscriptionInformation> OnSubscriptionSyncNow
		{
			add
			{
				DataAccessLayer.OnSubscriptionSyncNowEvent += value;
			}
			remove
			{
				DataAccessLayer.OnSubscriptionSyncNowEvent -= value;
			}
		}

		internal static event EventHandler<SubscriptionInformation> OnWorkTypeBasedSyncNow
		{
			add
			{
				DataAccessLayer.OnWorkTypeBasedSyncNowEvent += value;
			}
			remove
			{
				DataAccessLayer.OnWorkTypeBasedSyncNowEvent -= value;
			}
		}

		internal static event EventHandler<OnSyncCompletedEventArgs> OnSubscriptionSyncCompleted
		{
			add
			{
				DataAccessLayer.OnSubscriptionSyncCompletedEvent += value;
			}
			remove
			{
				DataAccessLayer.OnSubscriptionSyncCompletedEvent -= value;
			}
		}

		internal static event EventHandler<OnDatabaseDismountedEventArgs> OnDatabaseDismounted
		{
			add
			{
				DataAccessLayer.OnDatabaseDismountedEvent += value;
			}
			remove
			{
				DataAccessLayer.OnDatabaseDismountedEvent -= value;
			}
		}

		private static event EventHandler<SubscriptionInformation> OnSubscriptionAddedEvent;

		private static event EventHandler<SubscriptionInformation> OnSubscriptionRemovedEvent;

		private static event EventHandler<SubscriptionInformation> OnSubscriptionSyncNowEvent;

		private static event EventHandler<SubscriptionInformation> OnWorkTypeBasedSyncNowEvent;

		private static event EventHandler<OnSyncCompletedEventArgs> OnSubscriptionSyncCompletedEvent;

		private static event EventHandler<OnDatabaseDismountedEventArgs> OnDatabaseDismountedEvent;

		internal static GlobalDatabaseHandler DatabaseHandler
		{
			get
			{
				return DataAccessLayer.databaseHandler;
			}
		}

		internal static bool Initialized
		{
			get
			{
				bool result;
				lock (DataAccessLayer.startupShutdownLock)
				{
					result = (DataAccessLayer.initialized && !DataAccessLayer.initOrShutdownPending);
				}
				return result;
			}
		}

		internal static bool Initializing
		{
			get
			{
				bool result;
				lock (DataAccessLayer.startupShutdownLock)
				{
					result = (!DataAccessLayer.initialized && DataAccessLayer.initOrShutdownPending);
				}
				return result;
			}
		}

		internal static bool GenerateWatsonReports
		{
			set
			{
				DataAccessLayer.generateWatsonReports = value;
			}
		}

		internal static int DatabaseCount
		{
			get
			{
				return DataAccessLayer.databaseHandler.GetDatabaseCount();
			}
		}

		internal static SyncQueueManager SyncQueueManager
		{
			get
			{
				return DataAccessLayer.dispatchManager.SyncQueueManager;
			}
		}

		internal static void ShutdownDispatcher()
		{
			DataAccessLayer.dispatchManager.StopActiveDispatching();
		}

		internal static Guid[] GetDatabases()
		{
			GlobalDatabaseHandler globalDatabaseHandler = null;
			lock (DataAccessLayer.startupShutdownLock)
			{
				if (DataAccessLayer.Initialized)
				{
					globalDatabaseHandler = DataAccessLayer.databaseHandler;
				}
			}
			if (globalDatabaseHandler != null)
			{
				return globalDatabaseHandler.GetDatabases();
			}
			return null;
		}

		internal static void ReportWatson(string message, Exception exception)
		{
			if (DataAccessLayer.generateWatsonReports)
			{
				ContentAggregationConfig.SyncLogSession.ReportWatson(message, exception);
				return;
			}
			string format = string.Format(CultureInfo.InvariantCulture, "Message: {0}. Exception: {1}.", new object[]
			{
				message,
				exception
			});
			ContentAggregationConfig.SyncLogSession.LogError((TSLID)171UL, format, new object[0]);
		}

		internal static DatabaseManager GetDatabaseManager(Guid databaseGuid)
		{
			GlobalDatabaseHandler globalDatabaseHandler = null;
			lock (DataAccessLayer.startupShutdownLock)
			{
				if (DataAccessLayer.Initialized)
				{
					globalDatabaseHandler = DataAccessLayer.databaseHandler;
				}
			}
			if (globalDatabaseHandler != null)
			{
				return globalDatabaseHandler.GetDatabaseManager(databaseGuid);
			}
			return null;
		}

		internal static void TriggerOnDatabaseDismountedEvent(Guid databaseGuid)
		{
			EventHandler<OnDatabaseDismountedEventArgs> onDatabaseDismountedEvent = DataAccessLayer.OnDatabaseDismountedEvent;
			if (onDatabaseDismountedEvent != null)
			{
				onDatabaseDismountedEvent(null, new OnDatabaseDismountedEventArgs(databaseGuid));
			}
		}

		internal static void ScheduleMailboxCrawl(Guid databaseGuid, Guid mailboxGuid)
		{
			DatabaseManager databaseManager = DataAccessLayer.GetDatabaseManager(databaseGuid);
			if (databaseManager != null)
			{
				databaseManager.ScheduleMailboxCrawl(mailboxGuid);
			}
		}

		internal static SubscriptionCacheManager GetCacheManager(Guid databaseGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			GlobalDatabaseHandler globalDatabaseHandler = null;
			lock (DataAccessLayer.startupShutdownLock)
			{
				if (DataAccessLayer.Initialized)
				{
					globalDatabaseHandler = DataAccessLayer.databaseHandler;
				}
			}
			if (globalDatabaseHandler != null)
			{
				return globalDatabaseHandler.GetCacheManager(databaseGuid);
			}
			return null;
		}

		internal static SubscriptionCacheManager GetCacheManager(int databaseManagerIndex)
		{
			GlobalDatabaseHandler globalDatabaseHandler = null;
			lock (DataAccessLayer.startupShutdownLock)
			{
				if (DataAccessLayer.Initialized)
				{
					globalDatabaseHandler = DataAccessLayer.databaseHandler;
				}
			}
			if (globalDatabaseHandler != null)
			{
				return DataAccessLayer.GetCacheManager(globalDatabaseHandler.GetDatabaseGuid(databaseManagerIndex));
			}
			return null;
		}

		internal static DatabaseManager GetDatabaseManager(int databaseManagerIndex)
		{
			GlobalDatabaseHandler globalDatabaseHandler = null;
			lock (DataAccessLayer.startupShutdownLock)
			{
				if (DataAccessLayer.Initialized)
				{
					globalDatabaseHandler = DataAccessLayer.databaseHandler;
				}
			}
			if (globalDatabaseHandler != null)
			{
				return DataAccessLayer.GetDatabaseManager(globalDatabaseHandler.GetDatabaseGuid(databaseManagerIndex));
			}
			return null;
		}

		internal static void RefreshAppConfig()
		{
		}

		internal static bool Initialize()
		{
			lock (DataAccessLayer.startupShutdownLock)
			{
				if (DataAccessLayer.initialized)
				{
					return true;
				}
				if (DataAccessLayer.initOrShutdownPending)
				{
					throw new InvalidOperationException("Initialize or shutdown already pending");
				}
				DataAccessLayer.initOrShutdownPending = true;
			}
			bool flag2 = false;
			try
			{
				if (!ContentAggregationConfig.Start(true))
				{
					return false;
				}
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)172UL, DataAccessLayer.Tracer, "DAL.Initialize: Starting all components.", new object[0]);
				SyncHealthLogManager.Start();
				DataAccessLayer.databaseHandler = new GlobalDatabaseHandler();
				DataAccessLayer.databaseHandler.Initialize();
				ManagerPerfCounterHandler.Instance.StartUpdatingCounters();
				WorkTypeManager.Instance.Initialize();
				if (!SubscriptionCompletionServer.TryStartServer())
				{
					return false;
				}
				DataAccessLayer.dispatchManager = new DispatchManager(ContentAggregationConfig.SyncLogSession, ContentAggregationConfig.TransportSyncDispatchEnabled, ContentAggregationConfig.PrimingDispatchTime, ContentAggregationConfig.MinimumDispatchWaitForFailedSync, ContentAggregationConfig.DispatchOutageThreshold, new Action<EventLogEntry>(ContentAggregationConfig.LogEvent), SyncHealthLogger.Instance, SyncManagerConfiguration.Instance);
				DataAccessLayer.subscriptionProcessPermitter = new SubscriptionProcessPermitter(ContentAggregationConfig.SyncLogSession, DataAccessLayer.Tracer, SyncManagerConfiguration.Instance);
				DataAccessLayer.OnDatabaseDismounted += DataAccessLayer.dispatchManager.SyncQueueManager.OnDatabaseDismounted;
				DataAccessLayer.OnSubscriptionAdded += new EventHandler<SubscriptionInformation>(DataAccessLayer.dispatchManager.SyncQueueManager.OnSubscriptionAddedHandler);
				DataAccessLayer.OnSubscriptionRemoved += new EventHandler<SubscriptionInformation>(DataAccessLayer.dispatchManager.SyncQueueManager.OnSubscriptionRemovedHandler);
				DataAccessLayer.OnSubscriptionSyncNow += new EventHandler<SubscriptionInformation>(DataAccessLayer.dispatchManager.SyncQueueManager.OnSubscriptionSyncNowHandler);
				DataAccessLayer.OnWorkTypeBasedSyncNow += new EventHandler<SubscriptionInformation>(DataAccessLayer.dispatchManager.SyncQueueManager.OnWorkTypeBasedSyncNowHandler);
				DataAccessLayer.OnSubscriptionSyncCompleted += DataAccessLayer.dispatchManager.OnSubscriptionSyncCompletedHandler;
				DataAccessLayer.dispatchManager.SyncQueueManager.SubscriptionAddedOrRemovedEvent += ManagerPerfCounterHandler.Instance.OnSubscriptionAddedOrRemovedEvent;
				DataAccessLayer.dispatchManager.SyncQueueManager.SubscriptionEnqueuedOrDequeuedEvent += ManagerPerfCounterHandler.Instance.OnSubscriptionSyncEnqueuedOrDequeuedEvent;
				DataAccessLayer.dispatchManager.SyncQueueManager.ReportSyncQueueDispatchLagTimeEvent += ManagerPerfCounterHandler.Instance.OnReportSyncQueueDispatchLagTimeEvent;
				if (!SubscriptionNotificationServer.TryStartServer())
				{
					return false;
				}
				if (!SubscriptionCacheServer.TryStartServer())
				{
					return false;
				}
				ContentAggregationConfig.OnConfigurationChanged += DataAccessLayer.HandleConfigurationChange;
				DataAccessLayer.syncDiagnostics = new SyncDiagnostics();
				DataAccessLayer.syncDiagnostics.Register();
				flag2 = true;
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)208UL, DataAccessLayer.Tracer, "DAL.Initialize: All components initialized.", new object[0]);
			}
			finally
			{
				if (!flag2)
				{
					SyncHealthLogManager.Shutdown();
					if (DataAccessLayer.databaseHandler != null)
					{
						DataAccessLayer.databaseHandler.Shutdown();
					}
					ManagerPerfCounterHandler.Instance.StopUpdatingCounters();
					SubscriptionCompletionServer.StopServer();
					if (DataAccessLayer.dispatchManager != null)
					{
						DataAccessLayer.dispatchManager.Dispose();
						DataAccessLayer.dispatchManager = null;
					}
					SubscriptionNotificationServer.StopServer();
					SubscriptionCacheServer.StopServer();
				}
				lock (DataAccessLayer.startupShutdownLock)
				{
					DataAccessLayer.initialized = flag2;
					DataAccessLayer.initOrShutdownPending = false;
				}
			}
			return true;
		}

		internal static void Shutdown()
		{
			lock (DataAccessLayer.startupShutdownLock)
			{
				if (!DataAccessLayer.initialized)
				{
					return;
				}
				if (DataAccessLayer.initOrShutdownPending)
				{
					throw new InvalidOperationException("Initialize or shutdown already pending");
				}
				DataAccessLayer.initOrShutdownPending = true;
			}
			try
			{
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)173UL, DataAccessLayer.Tracer, "DAL.Shutdown: Stopping all components.", new object[0]);
				DataAccessLayer.syncDiagnostics.Unregister();
				DataAccessLayer.syncDiagnostics = null;
				SubscriptionCompletionServer.StopServer();
				SubscriptionNotificationServer.StopServer();
				SubscriptionCacheServer.StopServer();
				DataAccessLayer.OnDatabaseDismounted -= DataAccessLayer.dispatchManager.SyncQueueManager.OnDatabaseDismounted;
				DataAccessLayer.OnSubscriptionAdded -= new EventHandler<SubscriptionInformation>(DataAccessLayer.dispatchManager.SyncQueueManager.OnSubscriptionAddedHandler);
				DataAccessLayer.OnSubscriptionRemoved -= new EventHandler<SubscriptionInformation>(DataAccessLayer.dispatchManager.SyncQueueManager.OnSubscriptionRemovedHandler);
				DataAccessLayer.OnSubscriptionSyncNow -= new EventHandler<SubscriptionInformation>(DataAccessLayer.dispatchManager.SyncQueueManager.OnSubscriptionSyncNowHandler);
				DataAccessLayer.OnWorkTypeBasedSyncNow -= new EventHandler<SubscriptionInformation>(DataAccessLayer.dispatchManager.SyncQueueManager.OnWorkTypeBasedSyncNowHandler);
				DataAccessLayer.OnSubscriptionSyncCompleted -= DataAccessLayer.dispatchManager.OnSubscriptionSyncCompletedHandler;
				DataAccessLayer.dispatchManager.SyncQueueManager.SubscriptionAddedOrRemovedEvent -= ManagerPerfCounterHandler.Instance.OnSubscriptionAddedOrRemovedEvent;
				DataAccessLayer.dispatchManager.SyncQueueManager.SubscriptionEnqueuedOrDequeuedEvent -= ManagerPerfCounterHandler.Instance.OnSubscriptionSyncEnqueuedOrDequeuedEvent;
				DataAccessLayer.dispatchManager.SyncQueueManager.ReportSyncQueueDispatchLagTimeEvent -= ManagerPerfCounterHandler.Instance.OnReportSyncQueueDispatchLagTimeEvent;
				DataAccessLayer.dispatchManager.Dispose();
				DataAccessLayer.dispatchManager = null;
				ManagerPerfCounterHandler.Instance.StopUpdatingCounters();
				ContentAggregationConfig.OnConfigurationChanged -= DataAccessLayer.HandleConfigurationChange;
				DataAccessLayer.databaseHandler.Shutdown();
				DataAccessLayer.databaseHandler = null;
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)211UL, DataAccessLayer.Tracer, "DAL.Shutdown: entering shutdown state.", new object[0]);
				ContentAggregationConfig.Shutdown();
				SyncHealthLogManager.Shutdown();
			}
			finally
			{
				lock (DataAccessLayer.startupShutdownLock)
				{
					DataAccessLayer.initialized = false;
					DataAccessLayer.initOrShutdownPending = false;
				}
			}
		}

		internal static bool TryRebuildMailboxOnDatabase(Guid databaseGuid, Guid userMailboxGuid, out bool hasCacheChanged)
		{
			hasCacheChanged = false;
			ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)174UL, DataAccessLayer.Tracer, Guid.Empty, userMailboxGuid, "TryRebuildMailboxOnDatabase: Rebuilding mailbox in database {0}.", new object[]
			{
				databaseGuid
			});
			DatabaseManager databaseManager = DataAccessLayer.GetDatabaseManager(databaseGuid);
			if (databaseManager == null)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)175UL, DataAccessLayer.Tracer, Guid.Empty, userMailboxGuid, "TryRebuildMailboxOnDatabase: Failed to get database {0}.", new object[]
				{
					databaseGuid
				});
				return false;
			}
			return databaseManager.TryRebuildMailbox(userMailboxGuid, out hasCacheChanged);
		}

		internal static bool TryReportNewMailboxOnDatabase(MailboxSession mailboxSession, Guid databaseGuid)
		{
			DatabaseManager databaseManager = DataAccessLayer.GetDatabaseManager(databaseGuid);
			bool flag;
			return databaseManager != null && databaseManager.TryRebuildMailbox(mailboxSession.MailboxGuid, out flag);
		}

		internal static bool TryReadSubscriptionsInformation(Guid databaseGuid, Guid mailboxGuid, out IDictionary<Guid, SubscriptionInformation> subscriptionsInformation)
		{
			subscriptionsInformation = null;
			ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)181UL, DataAccessLayer.Tracer, Guid.Empty, mailboxGuid, "TryReadSubscriptionsInformation: Trying to read subscription information for subscriptions for mailbox in database {0}.", new object[]
			{
				databaseGuid
			});
			SubscriptionCacheManager cacheManager = DataAccessLayer.GetCacheManager(databaseGuid);
			if (cacheManager == null)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)182UL, DataAccessLayer.Tracer, Guid.Empty, mailboxGuid, "TryReadSubscriptionsInformation: Failed to get database {0}.", new object[]
				{
					databaseGuid
				});
				return false;
			}
			IEnumerable<SubscriptionCacheEntry> enumerable;
			try
			{
				enumerable = cacheManager.ReadAllSubscriptionsFromCache(mailboxGuid);
			}
			catch (CacheTransientException)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)183UL, DataAccessLayer.Tracer, Guid.Empty, mailboxGuid, "TryReadSubscriptionsInformation: Failed due to transient exception in database {0}.", new object[]
				{
					databaseGuid
				});
				return false;
			}
			catch (CacheCorruptException)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)184UL, DataAccessLayer.Tracer, Guid.Empty, mailboxGuid, "TryReadSubscriptionsInformation: Failed due to corruption exception in database {0}.", new object[]
				{
					databaseGuid
				});
				return false;
			}
			catch (CachePermanentException)
			{
				ContentAggregationConfig.SyncLogSession.LogError((TSLID)185UL, DataAccessLayer.Tracer, Guid.Empty, mailboxGuid, "TryReadSubscriptionsInformation: Failed due to permanent exception in database {0}.", new object[]
				{
					databaseGuid
				});
				return true;
			}
			subscriptionsInformation = new Dictionary<Guid, SubscriptionInformation>(DataAccessLayer.AverageSubscriptionsPerUser);
			foreach (SubscriptionCacheEntry subscriptionCacheEntry in enumerable)
			{
				SubscriptionInformation value = new SubscriptionInformation(cacheManager, subscriptionCacheEntry);
				subscriptionsInformation[subscriptionCacheEntry.SubscriptionGuid] = value;
			}
			return true;
		}

		internal static bool TryReadSubscriptionInformation(Guid databaseGuid, Guid mailboxGuid, StoreObjectId subscriptionMessageId, out SubscriptionInformation subscriptionInformation)
		{
			subscriptionInformation = null;
			bool flag = false;
			return DataAccessLayer.InternalTryReadSubscriptionInformation(databaseGuid, mailboxGuid, null, subscriptionMessageId, out subscriptionInformation, out flag);
		}

		internal static bool TryReadSubscriptionInformation(Guid databaseGuid, Guid mailboxGuid, Guid subscriptionGuid, out SubscriptionInformation subscriptionInformation, out bool needsBackOff)
		{
			subscriptionInformation = null;
			needsBackOff = false;
			return DataAccessLayer.InternalTryReadSubscriptionInformation(databaseGuid, mailboxGuid, new Guid?(subscriptionGuid), null, out subscriptionInformation, out needsBackOff);
		}

		internal static bool TryReportMailboxDeleted(Guid databaseGuid, Guid mailboxGuid)
		{
			ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)186UL, DataAccessLayer.Tracer, Guid.Empty, mailboxGuid, "TryReportMailboxDeleted: Reporting mailbox in database {0} was deleted.", new object[]
			{
				databaseGuid
			});
			SubscriptionCacheManager cacheManager = DataAccessLayer.GetCacheManager(databaseGuid);
			if (cacheManager == null)
			{
				return false;
			}
			try
			{
				cacheManager.DeleteCacheMessage(mailboxGuid);
			}
			catch (CacheTransientException)
			{
				return false;
			}
			catch (CachePermanentException)
			{
				return true;
			}
			return true;
		}

		internal static bool TryReportSubscriptionCompleted(SubscriptionCompletionData subscriptionCompletionData)
		{
			ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)188UL, DataAccessLayer.Tracer, subscriptionCompletionData.SubscriptionGuid, subscriptionCompletionData.MailboxGuid, "TryReportSubscriptionCompleted:SubscriptionCompletionStatus:{0},MoreDataToDownload:{1},Database:{2},syncFailed:{3},syncPhase:{4},serializedSubscription:{5},syncWatermark:{6}.", new object[]
			{
				subscriptionCompletionData.SubscriptionCompletionStatus,
				subscriptionCompletionData.MoreDataToDownload,
				subscriptionCompletionData.DatabaseGuid,
				subscriptionCompletionData.SyncFailed,
				subscriptionCompletionData.SyncPhase,
				subscriptionCompletionData.SerializedSubscription,
				subscriptionCompletionData.SyncWatermark ?? "<null>"
			});
			Guid? tenantGuid = null;
			string incomingServerName = string.Empty;
			AggregationSubscriptionType? subscriptionType = null;
			SubscriptionInformation subscriptionInformation;
			if (!DataAccessLayer.TryReadSubscriptionInformation(subscriptionCompletionData.DatabaseGuid, subscriptionCompletionData.MailboxGuid, subscriptionCompletionData.SubscriptionMessageID, out subscriptionInformation) || subscriptionInformation == null)
			{
				ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)193UL, DataAccessLayer.Tracer, subscriptionCompletionData.SubscriptionGuid, subscriptionCompletionData.MailboxGuid, "TryReportSubscriptionCompleted: Subscription not found in cache message: {0}.", new object[]
				{
					subscriptionCompletionData.SubscriptionMessageID
				});
			}
			else
			{
				tenantGuid = new Guid?(subscriptionInformation.TenantGuid);
				incomingServerName = subscriptionInformation.IncomingServerName.ToString();
				subscriptionType = new AggregationSubscriptionType?(subscriptionInformation.SubscriptionType);
				subscriptionCompletionData.LastSuccessfulDispatchTime = subscriptionInformation.LastSuccessfulDispatchTime;
				subscriptionInformation.MarkSyncCompletion(subscriptionCompletionData.DisableSubscription, new SyncPhase?(subscriptionCompletionData.SyncPhase), subscriptionCompletionData.SerializedSubscription, subscriptionCompletionData.SyncWatermark);
				subscriptionInformation.TrySave(null);
			}
			StatefulHubPicker.Instance.ResetHubLoad();
			DataAccessLayer.CollectSubscriptionCompletionDiagnostics(subscriptionCompletionData, tenantGuid, incomingServerName, subscriptionType);
			EventHandler<OnSyncCompletedEventArgs> onSubscriptionSyncCompletedEvent = DataAccessLayer.OnSubscriptionSyncCompletedEvent;
			if (onSubscriptionSyncCompletedEvent != null)
			{
				OnSyncCompletedEventArgs e = new OnSyncCompletedEventArgs(subscriptionCompletionData);
				onSubscriptionSyncCompletedEvent(null, e);
			}
			if (subscriptionCompletionData.InvalidState || subscriptionCompletionData.SubscriptionDeleted)
			{
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)189UL, DataAccessLayer.Tracer, subscriptionCompletionData.SubscriptionGuid, subscriptionCompletionData.MailboxGuid, "TryReportSubscriptionCompleted with:{0}", new object[]
				{
					subscriptionCompletionData.SubscriptionCompletionStatus
				});
				DatabaseManager databaseManager = DataAccessLayer.GetDatabaseManager(subscriptionCompletionData.DatabaseGuid);
				if (databaseManager != null)
				{
					databaseManager.ScheduleMailboxCrawl(subscriptionCompletionData.MailboxGuid);
				}
			}
			return true;
		}

		internal static bool TryReportSubscriptionListChanged(Guid mailboxGuid, Guid databaseGuid)
		{
			ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)198UL, DataAccessLayer.Tracer, Guid.Empty, mailboxGuid, "TryReportSubscriptionListChanged: Reporting subscription list change for mailbox {0} in database {1}.", new object[]
			{
				mailboxGuid,
				databaseGuid
			});
			DatabaseManager.MailboxCrawlerInstance.EnqueueMailboxCrawl(databaseGuid, mailboxGuid);
			return true;
		}

		internal static bool TryLogSubscriptionCreated(Guid mailboxGuid, Guid tenantGuid, AggregationSubscription subscription)
		{
			return SyncHealthLogManager.TryLogSubscriptionCreation(Environment.MachineName, tenantGuid.ToString(), mailboxGuid.ToString(), subscription.SubscriptionGuid.ToString(), subscription.SubscriptionType.ToString(), subscription.CreationType.ToString(), subscription.Domain, subscription.IncomingServerName, subscription.IncomingServerPort, subscription.AuthenticationType, subscription.EncryptionType, subscription.CreationTime, subscription.AggregationType.ToString());
		}

		internal static bool TryLogSubscriptionDeleted(Guid mailboxGuid, Guid tenantGuid, SubscriptionCacheEntry subscriptionCacheEntry, bool wasSubscriptionDeleted)
		{
			return SyncHealthLogManager.TryLogSubscriptionDeletion(Environment.MachineName, tenantGuid.ToString(), mailboxGuid.ToString(), subscriptionCacheEntry.SubscriptionGuid.ToString(), subscriptionCacheEntry.SubscriptionType.ToString(), subscriptionCacheEntry.IncomingServerName, wasSubscriptionDeleted, subscriptionCacheEntry.AggregationType.ToString());
		}

		internal static bool TryReportSyncNowNeeded(Guid databaseGuid, Guid mailboxGuid, Guid subscriptionId)
		{
			ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)212UL, DataAccessLayer.Tracer, subscriptionId, mailboxGuid, "TryReportSyncNowNeeded: Reporting sync now needed for DB: {0}.", new object[]
			{
				databaseGuid
			});
			SubscriptionCacheManager cacheManager = DataAccessLayer.GetCacheManager(databaseGuid);
			if (cacheManager == null)
			{
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)1551UL, DataAccessLayer.Tracer, subscriptionId, mailboxGuid, "TryReportSyncNowNeeded: CacheManager is not available for DB: {0}.", new object[]
				{
					databaseGuid
				});
				return false;
			}
			SubscriptionCacheEntry subscriptionCacheEntry = null;
			try
			{
				subscriptionCacheEntry = cacheManager.ReadSubscriptionFromCache(mailboxGuid, subscriptionId);
			}
			catch (CacheTransientException)
			{
				return false;
			}
			catch (CacheCorruptException)
			{
				return false;
			}
			catch (CacheNotFoundException)
			{
				DataAccessLayer.ScheduleMailboxCrawl(databaseGuid, mailboxGuid);
				return false;
			}
			catch (CachePermanentException)
			{
				return true;
			}
			if (subscriptionCacheEntry == null)
			{
				DataAccessLayer.ScheduleMailboxCrawl(databaseGuid, mailboxGuid);
				return false;
			}
			if (subscriptionCacheEntry.Disabled)
			{
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)209UL, DataAccessLayer.Tracer, subscriptionId, mailboxGuid, "TryIssueSyncNowForSubscription: Subscription in database {0} was disabled. Will enable it.", new object[]
				{
					databaseGuid
				});
				try
				{
					subscriptionCacheEntry.Disabled = false;
					cacheManager.UpdateCacheMessage(subscriptionCacheEntry);
				}
				catch (CacheTransientException)
				{
					return false;
				}
				catch (CacheCorruptException)
				{
					return false;
				}
				catch (CacheNotFoundException)
				{
					DataAccessLayer.ScheduleMailboxCrawl(databaseGuid, mailboxGuid);
					return false;
				}
				catch (CachePermanentException)
				{
					return true;
				}
			}
			SubscriptionInformation subscriptionInformation = new SubscriptionInformation(cacheManager, subscriptionCacheEntry);
			if (subscriptionInformation.AggregationType == AggregationType.Migration && subscriptionInformation.SyncPhase == SyncPhase.Incremental)
			{
				subscriptionInformation.UpdateSyncPhase(SyncPhase.Finalization);
				subscriptionInformation.TrySave(ContentAggregationConfig.SyncLogSession);
			}
			ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)210UL, DataAccessLayer.Tracer, subscriptionId, mailboxGuid, "TryIssueSyncNowForSubscription: Sending sync-now event for subscription in mailbox at database {0}.", new object[]
			{
				databaseGuid
			});
			DataAccessLayer.TriggerSyncNowEvent(subscriptionInformation);
			return true;
		}

		internal static void ReportSubscriptionDispatch(string mailboxServerName, string tenantGuid, string userMailboxGuid, string subscriptionGuid, string incomingServerName, string subscriptionType, string aggregationType, string dispatchedTo, bool successful, bool permanentError, bool transientError, string dispatchError, bool beyondSyncPollingFrequency, int secondsBeyondPollingFrequency, string workType, string dispatchTrigger, string databaseGuid)
		{
			SyncHealthLogManager.TryLogSubscriptionDispatch(mailboxServerName, tenantGuid, userMailboxGuid, subscriptionGuid, incomingServerName, subscriptionType, aggregationType, dispatchedTo, successful, permanentError, transientError, dispatchError, beyondSyncPollingFrequency, secondsBeyondPollingFrequency, workType, dispatchTrigger, databaseGuid);
		}

		internal static XElement GetDatabaseHandlerDiagnosticInfo(SyncDiagnosticMode mode)
		{
			return DataAccessLayer.databaseHandler.GetDiagnosticInfo(mode);
		}

		internal static XElement GetDispatchManagerDiagnosticInfo(SyncDiagnosticMode mode)
		{
			return DataAccessLayer.dispatchManager.GetDiagnosticInfo(mode);
		}

		internal static void OnSubscriptionAddedHandler(object sender, SubscriptionInformation subscriptionInformation)
		{
			if (!DataAccessLayer.subscriptionProcessPermitter.IsSubscriptionPermitted(subscriptionInformation))
			{
				return;
			}
			EventHandler<SubscriptionInformation> onSubscriptionAddedEvent = DataAccessLayer.OnSubscriptionAddedEvent;
			if (onSubscriptionAddedEvent == null)
			{
				ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)213UL, DataAccessLayer.Tracer, subscriptionInformation.SubscriptionGuid, subscriptionInformation.MailboxGuid, "OnSubscriptionAddedHandler: SubscriptionAdded handler is null.", new object[0]);
				return;
			}
			ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)393UL, DataAccessLayer.Tracer, subscriptionInformation.SubscriptionGuid, subscriptionInformation.MailboxGuid, "OnSubscriptionAddedHandler: Calling SubscriptionAdded handler.", new object[0]);
			onSubscriptionAddedEvent(sender, subscriptionInformation);
		}

		internal static void OnSubscriptionRemovedHandler(object sender, SubscriptionInformation subscriptionInformation)
		{
			EventHandler<SubscriptionInformation> onSubscriptionRemovedEvent = DataAccessLayer.OnSubscriptionRemovedEvent;
			if (onSubscriptionRemovedEvent != null)
			{
				onSubscriptionRemovedEvent(sender, subscriptionInformation);
			}
		}

		internal static void OnWorkTypeBasedSyncNowHandler(WorkType workType, SubscriptionInformation subscriptionInformation)
		{
			EventHandler<SubscriptionInformation> onWorkTypeBasedSyncNowEvent = DataAccessLayer.OnWorkTypeBasedSyncNowEvent;
			if (onWorkTypeBasedSyncNowEvent != null)
			{
				onWorkTypeBasedSyncNowEvent(workType, subscriptionInformation);
			}
		}

		internal static bool TryUpdateCacheMessageSyncPhase(Guid databaseGuid, Guid mailboxGuid, Guid subscriptionGuid, SyncPhase syncPhase, out SubscriptionInformation subscriptionInformation)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			subscriptionInformation = null;
			GlobalSyncLogSession syncLogSession = ContentAggregationConfig.SyncLogSession;
			Trace tracer = DataAccessLayer.Tracer;
			SubscriptionCacheManager cacheManager = DataAccessLayer.GetCacheManager(databaseGuid);
			if (cacheManager == null)
			{
				syncLogSession.LogDebugging((TSLID)957UL, tracer, "TryUpdateCacheMessageSyncPhase could not get cache manager.", new object[0]);
				return false;
			}
			try
			{
				SubscriptionCacheEntry subscriptionCacheEntry = cacheManager.ReadSubscriptionFromCache(mailboxGuid, subscriptionGuid);
				if (subscriptionCacheEntry != null)
				{
					subscriptionCacheEntry.SyncPhase = syncPhase;
					cacheManager.UpdateCacheMessage(subscriptionCacheEntry);
					subscriptionInformation = new SubscriptionInformation(cacheManager, subscriptionCacheEntry);
				}
			}
			catch (CacheTransientException ex)
			{
				syncLogSession.LogDebugging((TSLID)963UL, tracer, "TryUpdateCacheMessageSyncPhase ex:{0}", new object[]
				{
					ex
				});
				return false;
			}
			catch (CachePermanentException ex2)
			{
				syncLogSession.LogDebugging((TSLID)999UL, tracer, "TryUpdateCacheMessageSyncPhase ex:{0}", new object[]
				{
					ex2
				});
				return false;
			}
			syncLogSession.LogDebugging((TSLID)1106UL, tracer, "TryUpdateCacheMessageSyncPhase succeeded.", new object[0]);
			return true;
		}

		private static void CollectSubscriptionCompletionDiagnostics(SubscriptionCompletionData subscriptionCompletionData, Guid? tenantGuid, string incomingServerName, AggregationSubscriptionType? subscriptionType)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			TimeSpan timeSpan = TimeSpan.MinValue;
			if (subscriptionCompletionData.LastSuccessfulDispatchTime != null && subscriptionType != null)
			{
				timeSpan = utcNow - subscriptionCompletionData.LastSuccessfulDispatchTime.Value;
				ManagerPerfCounterHandler.Instance.SetLastSubscriptionProcessingTime(subscriptionType.Value, Convert.ToInt64(timeSpan.TotalMilliseconds));
			}
			SyncHealthLogManager.TryLogSubscriptionCompletion(Environment.MachineName, (tenantGuid != null) ? tenantGuid.Value.ToString() : string.Empty, subscriptionCompletionData.MailboxGuid.ToString(), subscriptionCompletionData.SubscriptionGuid.ToString(), incomingServerName, (subscriptionType != null) ? subscriptionType.ToString() : string.Empty, subscriptionCompletionData.AggregationType.ToString(), string.Empty, timeSpan, subscriptionCompletionData.MoreDataToDownload);
			ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)1290UL, DataAccessLayer.Tracer, subscriptionCompletionData.SubscriptionGuid, subscriptionCompletionData.MailboxGuid, "TryReportSubscriptionCompleted: SubscriptionProcessingTime: {0}.", new object[]
			{
				timeSpan
			});
		}

		private static bool InternalTryReadSubscriptionInformation(Guid databaseGuid, Guid mailboxGuid, Guid? subscriptionGuid, StoreObjectId subscriptionMessageId, out SubscriptionInformation subscriptionInformation, out bool needsBackOff)
		{
			subscriptionInformation = null;
			needsBackOff = false;
			SubscriptionCacheManager cacheManager = DataAccessLayer.GetCacheManager(databaseGuid);
			if (cacheManager == null)
			{
				return true;
			}
			bool result;
			try
			{
				SubscriptionCacheEntry subscriptionCacheEntry;
				if (subscriptionGuid != null)
				{
					subscriptionCacheEntry = cacheManager.ReadSubscriptionFromCache(mailboxGuid, subscriptionGuid.Value);
				}
				else
				{
					subscriptionCacheEntry = cacheManager.ReadSubscriptionFromCache(mailboxGuid, subscriptionMessageId);
				}
				if (subscriptionCacheEntry == null)
				{
					result = true;
				}
				else
				{
					subscriptionInformation = new SubscriptionInformation(cacheManager, subscriptionCacheEntry);
					result = true;
				}
			}
			catch (CacheTransientException)
			{
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)190UL, DataAccessLayer.Tracer, (subscriptionGuid != null) ? subscriptionGuid.Value : Guid.Empty, mailboxGuid, "InternalTryReadSubscriptionInformation: Failed due to transient exception in database {0}.", new object[]
				{
					databaseGuid
				});
				needsBackOff = true;
				result = false;
			}
			catch (CacheCorruptException)
			{
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)191UL, DataAccessLayer.Tracer, (subscriptionGuid != null) ? subscriptionGuid.Value : Guid.Empty, mailboxGuid, "InternalTryReadSubscriptionInformation: Failed due to cache corrupt in database {0}.", new object[]
				{
					databaseGuid
				});
				result = true;
			}
			catch (CacheNotFoundException)
			{
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)192UL, DataAccessLayer.Tracer, (subscriptionGuid != null) ? subscriptionGuid.Value : Guid.Empty, mailboxGuid, "InternalTryReadSubscriptionInformation: Failed due to cache not found exception in database {0}.", new object[]
				{
					databaseGuid
				});
				result = true;
			}
			catch (CachePermanentException)
			{
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)194UL, DataAccessLayer.Tracer, (subscriptionGuid != null) ? subscriptionGuid.Value : Guid.Empty, mailboxGuid, "InternalTryReadSubscriptionInformation: Failed due to cachepermanent exception in database {0}.", new object[]
				{
					databaseGuid
				});
				result = true;
			}
			return result;
		}

		private static void TriggerSyncNowEvent(SubscriptionInformation subscriptionInformation)
		{
			EventHandler<SubscriptionInformation> onSubscriptionSyncNowEvent = DataAccessLayer.OnSubscriptionSyncNowEvent;
			if (onSubscriptionSyncNowEvent != null)
			{
				onSubscriptionSyncNowEvent(null, subscriptionInformation);
			}
		}

		private static void HandleConfigurationChange()
		{
			lock (DataAccessLayer.startupShutdownLock)
			{
				if (!DataAccessLayer.initialized)
				{
					ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)216UL, DataAccessLayer.Tracer, "Ignoring Configuration Change as DAL is not initialized.", new object[0]);
				}
				else
				{
					SyncHealthLogManager.TryConfigure(ContentAggregationConfig.SyncMailboxHealthLogConfiguration);
				}
			}
		}

		internal static readonly int AverageSubscriptionsPerUser = 2;

		private static readonly Trace Tracer = ExTraceGlobals.DataAccessLayerTracer;

		private static object startupShutdownLock = new object();

		private static bool initialized;

		private static bool initOrShutdownPending;

		private static DispatchManager dispatchManager;

		private static SyncDiagnostics syncDiagnostics;

		private static GlobalDatabaseHandler databaseHandler;

		private static SubscriptionProcessPermitter subscriptionProcessPermitter;

		private static bool generateWatsonReports = true;
	}
}
