using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Notification;
using Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class SyncHealthLogManager
	{
		public static bool TryConfigure(SyncHealthLogConfiguration syncHealthLogConfiguration)
		{
			bool result;
			lock (SyncHealthLogManager.syncObject)
			{
				if (SyncHealthLogManager.initialized)
				{
					SyncHealthLogManager.syncHealthLog.Configure(syncHealthLogConfiguration);
					result = true;
				}
				else
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)227UL, SyncHealthLogManager.Tracer, "TryConfigure failed due to shutdown.", new object[0]);
					result = false;
				}
			}
			return result;
		}

		public static bool TryLogSubscriptionCompletion(string mailboxServerName, string tenantGuid, string userMailboxGuid, string subscriptionGuid, string incomingServerName, string subscriptionType, string aggregationType, string processedBy, TimeSpan syncDuration, bool moreDataAvailable)
		{
			return SyncHealthLogManager.Log(delegate
			{
				SyncHealthLogManager.syncHealthLog.LogSubscriptionCompletion(mailboxServerName, tenantGuid, userMailboxGuid, subscriptionGuid, incomingServerName, subscriptionType, aggregationType, processedBy, syncDuration, moreDataAvailable);
			});
		}

		public static bool TryLogSubscriptionCreation(string mailboxServerName, string tenantGuid, string userMailboxGuid, string subscriptionGuid, string subscriptionType, string creationType, string emailDomain, string incomingServerName, int port, string authenticationType, string encryptionType, DateTime creationTime, string aggregationType)
		{
			return SyncHealthLogManager.Log(delegate
			{
				SyncHealthLogManager.syncHealthLog.LogSubscriptionCreation(mailboxServerName, tenantGuid, userMailboxGuid, subscriptionGuid, subscriptionType, creationType, emailDomain, incomingServerName, port, authenticationType, encryptionType, creationTime, aggregationType);
			});
		}

		public static bool TryLogDatabaseDiscovery(ExDateTime dbPollingTimeStamp, string dbPollingSource, int totalDBCount, int enabledDBCount, string databaseId, string databaseEvent, string currentDatabaseState)
		{
			return SyncHealthLogManager.Log(delegate
			{
				SyncHealthLogManager.syncHealthLog.LogDatabaseDiscovery(dbPollingTimeStamp, dbPollingSource, totalDBCount, enabledDBCount, databaseId, databaseEvent, currentDatabaseState);
			});
		}

		public static bool TryLogMailboxNotification(Guid mailboxGuid, Guid mdbGuid, SubscriptionNotificationRpcMethodCode notificationType)
		{
			bool result;
			lock (SyncHealthLogManager.syncObject)
			{
				if (SyncHealthLogManager.initialized)
				{
					SyncHealthLogManager.syncHealthLog.LogMailboxNotification(mailboxGuid, mdbGuid, notificationType);
					result = true;
				}
				else
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)518UL, SyncHealthLogManager.Tracer, "TryLogMailboxNotification failed due to shutdown.", new object[0]);
					result = false;
				}
			}
			return result;
		}

		public static bool TryLogSubscriptionDeletion(string mailboxServerName, string tenantGuid, string userMailboxGuid, string subscriptionGuid, string subscriptionType, string incomingServerName, bool wasSubscriptionDeleted, string aggregationType)
		{
			return SyncHealthLogManager.Log(delegate
			{
				SyncHealthLogManager.syncHealthLog.LogSubscriptionDeletion(mailboxServerName, tenantGuid, userMailboxGuid, subscriptionGuid, subscriptionType, incomingServerName, wasSubscriptionDeleted, aggregationType);
			});
		}

		public static bool TryLogSubscriptionDispatch(string mailboxServerName, string tenantGuid, string userMailboxGuid, string subscriptionGuid, string incomingServerName, string subscriptionType, string aggregationType, string dispatchedTo, bool isSuccessful, bool isPermanentError, bool isTransientError, string dispatchError, bool isBeyondSyncPollingFrequency, int secondsBeyondPollingFrequency, string workType, string dispatchTrigger, string databaseGuid)
		{
			return SyncHealthLogManager.Log(delegate
			{
				SyncHealthLogManager.syncHealthLog.LogSubscriptionDispatch(mailboxServerName, tenantGuid, userMailboxGuid, subscriptionGuid, incomingServerName, subscriptionType, aggregationType, dispatchedTo, isSuccessful, isPermanentError, isTransientError, dispatchError, isBeyondSyncPollingFrequency, secondsBeyondPollingFrequency, workType, dispatchTrigger, databaseGuid);
			});
		}

		public static void LogWorkTypeBudgets(KeyValuePair<string, object>[] eventData)
		{
			SyncHealthLogManager.Log(delegate
			{
				SyncHealthLogManager.syncHealthLog.LogWorkTypeBudgets(eventData);
			});
		}

		public static void Start()
		{
			lock (SyncHealthLogManager.syncObject)
			{
				if (!SyncHealthLogManager.initialized)
				{
					ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)233UL, SyncHealthLogManager.Tracer, "SyncHealthLogManager.Start: Called.", new object[0]);
					SyncHealthLogManager.syncHealthLog = new SyncHealthLog(ContentAggregationConfig.SyncMailboxHealthLogConfiguration);
					SyncHealthLogManager.initialized = true;
				}
			}
		}

		public static void Shutdown()
		{
			lock (SyncHealthLogManager.syncObject)
			{
				if (SyncHealthLogManager.initialized)
				{
					ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)234UL, SyncHealthLogManager.Tracer, "SyncHealthLogManager.Shutdown: Called.", new object[0]);
					SyncHealthLogManager.syncHealthLog.Dispose();
					SyncHealthLogManager.syncHealthLog = null;
					SyncHealthLogManager.initialized = false;
				}
			}
		}

		private static bool Log(Action action)
		{
			bool result;
			lock (SyncHealthLogManager.syncObject)
			{
				if (SyncHealthLogManager.initialized)
				{
					action();
					result = true;
				}
				else
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)231UL, SyncHealthLogManager.Tracer, "Log failed due to shutdown.", new object[0]);
					result = false;
				}
			}
			return result;
		}

		private static readonly Trace Tracer = ExTraceGlobals.SyncHealthLogManagerTracer;

		private static readonly object syncObject = new object();

		private static bool initialized;

		private static SyncHealthLog syncHealthLog;
	}
}
