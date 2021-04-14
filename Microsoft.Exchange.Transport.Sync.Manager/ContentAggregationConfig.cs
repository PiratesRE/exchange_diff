using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ContentAggregationConfig
	{
		internal static event ContentAggregationConfig.ConfigurationChangedEventHandler OnConfigurationChanged
		{
			add
			{
				lock (ContentAggregationConfig.eventHandlers)
				{
					ContentAggregationConfig.eventHandlers.Add(value);
				}
			}
			remove
			{
				lock (ContentAggregationConfig.eventHandlers)
				{
					if (!ContentAggregationConfig.eventHandlers.Remove(value))
					{
						throw new ArgumentException("Event handler is not registered", "value");
					}
				}
			}
		}

		internal static Server LocalServer
		{
			get
			{
				return ContentAggregationConfig.localServer;
			}
		}

		internal static ExEventLog EventLogger
		{
			get
			{
				return ContentAggregationConfig.eventLogger;
			}
		}

		internal static SyncLog SyncLog
		{
			get
			{
				return ContentAggregationConfig.syncLog;
			}
		}

		internal static GlobalSyncLogSession SyncLogSession
		{
			get
			{
				return ContentAggregationConfig.syncLogSession;
			}
		}

		internal static TimeSpan AggregationIncrementalSyncInterval
		{
			get
			{
				return ContentAggregationConfig.aggregationIncrementalSyncInterval;
			}
		}

		internal static TimeSpan MigrationIncrementalSyncInterval
		{
			get
			{
				return ContentAggregationConfig.migrationIncrementalSyncInterval;
			}
		}

		internal static TimeSpan PeopleConnectionInitialSyncInterval
		{
			get
			{
				return ContentAggregationConfig.peopleConnectionInitialSyncInterval;
			}
		}

		internal static TimeSpan PeopleConnectionTriggeredSyncInterval
		{
			get
			{
				return ContentAggregationConfig.peopleConnectionTriggeredSyncInterval;
			}
		}

		internal static TimeSpan PeopleConnectionIncrementalSyncInterval
		{
			get
			{
				return ContentAggregationConfig.peopleConnectionIncrementalSyncInterval;
			}
		}

		internal static TimeSpan OwaMailboxPolicyInducedDeleteInterval
		{
			get
			{
				return ContentAggregationConfig.owaMailboxPolicyInducedDeleteInterval;
			}
		}

		internal static TimeSpan OwaMailboxPolicyProbeInterval
		{
			get
			{
				return ContentAggregationConfig.owaMailboxPolicyProbeInterval;
			}
		}

		internal static byte AggregationSubscriptionSavedSyncWeight
		{
			get
			{
				return ContentAggregationConfig.aggregationSubscriptionSavedSyncWeight;
			}
		}

		internal static byte AggregationIncrementalSyncWeight
		{
			get
			{
				return ContentAggregationConfig.aggregationIncrementalSyncWeight;
			}
		}

		internal static byte OwaLogonTriggeredSyncWeight
		{
			get
			{
				return ContentAggregationConfig.owaLogonTriggeredSyncWeight;
			}
		}

		internal static byte OwaRefreshButtonTriggeredSyncWeight
		{
			get
			{
				return ContentAggregationConfig.owaRefreshButtonTriggeredSyncWeight;
			}
		}

		internal static byte OwaSessionTriggeredSyncWeight
		{
			get
			{
				return ContentAggregationConfig.owaSessionTriggeredSyncWeight;
			}
		}

		internal static byte MigrationInitialSyncWeight
		{
			get
			{
				return ContentAggregationConfig.migrationInitialSyncWeight;
			}
		}

		internal static byte AggregationInitialSyncWeight
		{
			get
			{
				return ContentAggregationConfig.aggregationInitialSyncWeight;
			}
		}

		internal static byte MigrationFinalizationSyncWeight
		{
			get
			{
				return ContentAggregationConfig.migrationFinalizationSyncWeight;
			}
		}

		internal static byte MigrationIncrementalSyncWeight
		{
			get
			{
				return ContentAggregationConfig.migrationIncrementalSyncWeight;
			}
		}

		internal static byte PeopleConnectionInitialSyncWeight
		{
			get
			{
				return ContentAggregationConfig.peopleConnectionInitialSyncWeight;
			}
		}

		internal static byte PeopleConnectionTriggeredSyncWeight
		{
			get
			{
				return ContentAggregationConfig.peopleConnectionTriggeredSyncWeight;
			}
		}

		internal static byte PeopleConnectionIncrementalSyncWeight
		{
			get
			{
				return ContentAggregationConfig.peopleConnectionIncrementalSyncWeight;
			}
		}

		internal static byte OwaMailboxPolicyInducedDeleteWeight
		{
			get
			{
				return ContentAggregationConfig.owaMailboxPolicyInducedDeleteWeight;
			}
		}

		internal static TimeSpan DispatchEntryExpirationTime
		{
			get
			{
				return ContentAggregationConfig.dispatchEntryExpirationTime;
			}
		}

		internal static TimeSpan DispatchEntryExpirationCheckFrequency
		{
			get
			{
				return ContentAggregationConfig.dispatchEntryExpirationCheckFrequency;
			}
		}

		internal static TimeSpan DatabasePollingInterval
		{
			get
			{
				return ContentAggregationConfig.databasePollingInterval;
			}
			set
			{
				ContentAggregationConfig.databasePollingInterval = value;
			}
		}

		internal static TimeSpan PrimingDispatchTime
		{
			get
			{
				return ContentAggregationConfig.primingDispatchTime;
			}
		}

		internal static TimeSpan MigrationInitialSyncInterval
		{
			get
			{
				return ContentAggregationConfig.migrationInitialSyncInterval;
			}
		}

		internal static TimeSpan AggregationInitialSyncInterval
		{
			get
			{
				return ContentAggregationConfig.aggregationInitialSyncInterval;
			}
		}

		internal static TimeSpan SyncNowTime
		{
			get
			{
				return ContentAggregationConfig.syncNowTime;
			}
		}

		internal static TimeSpan OwaTriggeredSyncNowTime
		{
			get
			{
				return ContentAggregationConfig.owaTriggeredSyncNowTime;
			}
		}

		internal static TimeSpan MailboxTablePollingInterval
		{
			get
			{
				return ContentAggregationConfig.mailboxTablePollingInterval;
			}
			set
			{
				ContentAggregationConfig.mailboxTablePollingInterval = value;
			}
		}

		internal static TimeSpan MailboxTableRetryPollingInterval
		{
			get
			{
				return ContentAggregationConfig.mailboxTableRetryPollingInterval;
			}
		}

		internal static TimeSpan MailboxTableTwoWayPollingInterval
		{
			get
			{
				return ContentAggregationConfig.mailboxTableTwoWayPollingInterval;
			}
			set
			{
				ContentAggregationConfig.mailboxTableTwoWayPollingInterval = value;
			}
		}

		internal static TimeSpan DelayBeforeMailboxTablePollingStarts
		{
			get
			{
				return ContentAggregationConfig.delayBeforeMailboxTablePollingStarts;
			}
			set
			{
				ContentAggregationConfig.delayBeforeMailboxTablePollingStarts = value;
			}
		}

		internal static bool PopAggregationEnabled
		{
			get
			{
				return ContentAggregationConfig.popAggregationEnabled;
			}
		}

		internal static bool ImapAggregationEnabled
		{
			get
			{
				return ContentAggregationConfig.imapAggregationEnabled;
			}
		}

		internal static TimeSpan DispatcherDatabaseRefreshFrequency
		{
			get
			{
				return ContentAggregationConfig.dispatcherDatabaseRefreshFrequency;
			}
		}

		internal static bool FacebookAggregationEnabled
		{
			get
			{
				return ContentAggregationConfig.facebookAggregationEnabled;
			}
		}

		internal static bool DeltaSyncAggregationEnabled
		{
			get
			{
				return ContentAggregationConfig.deltaSyncAggregationEnabled;
			}
		}

		internal static bool LinkedInAggregationEnabled
		{
			get
			{
				return ContentAggregationConfig.linkedInAggregationEnabled;
			}
		}

		internal static bool OwaMailboxPolicyConstraintEnabled
		{
			get
			{
				return ContentAggregationConfig.owaMailboxPolicyConstraintEnabled;
			}
		}

		internal static AggregationSubscriptionType SubscriptionTypesAllowed
		{
			get
			{
				AggregationSubscriptionType aggregationSubscriptionType = AggregationSubscriptionType.All;
				if (!ContentAggregationConfig.PopAggregationEnabled)
				{
					aggregationSubscriptionType &= ~AggregationSubscriptionType.Pop;
				}
				if (!ContentAggregationConfig.ImapAggregationEnabled)
				{
					aggregationSubscriptionType &= ~AggregationSubscriptionType.IMAP;
				}
				if (!ContentAggregationConfig.DeltaSyncAggregationEnabled)
				{
					aggregationSubscriptionType &= ~AggregationSubscriptionType.DeltaSyncMail;
				}
				if (!ContentAggregationConfig.FacebookAggregationEnabled)
				{
					aggregationSubscriptionType &= ~AggregationSubscriptionType.Facebook;
				}
				if (!ContentAggregationConfig.LinkedInAggregationEnabled)
				{
					aggregationSubscriptionType &= ~AggregationSubscriptionType.LinkedIn;
				}
				return aggregationSubscriptionType;
			}
		}

		internal static TimeSpan HubBusyPeriod
		{
			get
			{
				return ContentAggregationConfig.hubBusyPeriod;
			}
		}

		internal static TimeSpan HubSubscriptionTypeNotSupportedPeriod
		{
			get
			{
				return ContentAggregationConfig.hubSubscriptionTypeNotSupportedPeriod;
			}
		}

		internal static TimeSpan DatabaseBackoffTime
		{
			get
			{
				return ContentAggregationConfig.databaseBackoffTime;
			}
		}

		internal static TimeSpan MinimumDispatchWaitForFailedSync
		{
			get
			{
				return ContentAggregationConfig.minimumDispatchWaitForFailedSync;
			}
		}

		internal static TimeSpan WorkTypeBudgetManagerSlidingWindowLength
		{
			get
			{
				return ContentAggregationConfig.workTypeBudgetManagerSlidingWindowLength;
			}
		}

		internal static TimeSpan WorkTypeBudgetManagerSlidingBucketLength
		{
			get
			{
				return ContentAggregationConfig.workTypeBudgetManagerSlidingBucketLength;
			}
		}

		internal static TimeSpan WorkTypeBudgetManagerSampleDispatchedWorkFrequency
		{
			get
			{
				return ContentAggregationConfig.workTypeBudgetManagerSampleDispatchedWorkFrequency;
			}
		}

		internal static TimeSpan HubInactivityPeriod
		{
			get
			{
				return ContentAggregationConfig.hubInactivityPeriod;
			}
		}

		internal static int MaxCompletionThreads
		{
			get
			{
				return ContentAggregationConfig.maxCompletionThreads;
			}
		}

		internal static int MaxCacheRpcThreads
		{
			get
			{
				return ContentAggregationConfig.maxCacheRpcThreads;
			}
		}

		internal static int MaxNotificationThreads
		{
			get
			{
				return ContentAggregationConfig.maxNotificationThreads;
			}
		}

		internal static int MaxManualResetEventsInResourcePool
		{
			get
			{
				return ContentAggregationConfig.maxManualResetEventsInResourcePool;
			}
		}

		internal static int MaxMailboxSessionsInResourcePool
		{
			get
			{
				return ContentAggregationConfig.maxMailboxSessionsInResourcePool;
			}
		}

		internal static TimeSpan TokenWaitTimeOutInterval
		{
			get
			{
				return ContentAggregationConfig.tokenWaitTimeOutInterval;
			}
			set
			{
				ContentAggregationConfig.tokenWaitTimeOutInterval = value;
			}
		}

		internal static TimeSpan DispatchOutageThreshold
		{
			get
			{
				return ContentAggregationConfig.dispatchOutageThreshold;
			}
		}

		internal static TimeSpan PoolBackOffTimeInterval
		{
			get
			{
				return ContentAggregationConfig.poolBackOffTimeInterval;
			}
		}

		internal static TimeSpan PoolExpiryCheckInterval
		{
			get
			{
				return ContentAggregationConfig.poolExpiryCheckInterval;
			}
		}

		internal static TimeSpan MaxSystemMailboxSessionsUnusedPeriod
		{
			get
			{
				return ContentAggregationConfig.maxSystemMailboxSessionsUnusedPeriod;
			}
		}

		internal static int DispatcherBackOffTimeInSeconds
		{
			get
			{
				return ContentAggregationConfig.dispatcherBackOffTimeInSeconds;
			}
		}

		internal static int MaxNumberOfAttemptsBeforePoolBackOff
		{
			get
			{
				return ContentAggregationConfig.maxNumberOfAttemptsBeforePoolBackOff;
			}
		}

		internal static TimeSpan SLAPerfCounterUpdateInterval
		{
			get
			{
				return ContentAggregationConfig.sLAPerfCounterUpdateInterval;
			}
		}

		internal static int SLAExpiryBuckets
		{
			get
			{
				return ContentAggregationConfig.slaExpiryBuckets;
			}
		}

		internal static int SLADataBuckets
		{
			get
			{
				return ContentAggregationConfig.slaDataBuckets;
			}
		}

		internal static TimeSpan PCExpiryInterval
		{
			get
			{
				return ContentAggregationConfig.pCExpiryInterval;
			}
		}

		internal static int MaxSyncsPerDB
		{
			get
			{
				return ContentAggregationConfig.maxSyncsPerDB;
			}
		}

		internal static bool CacheRepairEnabled
		{
			get
			{
				return ContentAggregationConfig.cacheRepairEnabled;
			}
		}

		internal static int MaxCacheMessageRepairAttempts
		{
			get
			{
				return ContentAggregationConfig.maxCacheMessageRepairAttempts;
			}
		}

		internal static TimeSpan DelayBeforeRepairThreadStarts
		{
			get
			{
				return ContentAggregationConfig.delayBeforeRepairThreadStarts;
			}
		}

		internal static TimeSpan DelayBetweenDispatchQueueBuilds
		{
			get
			{
				return ContentAggregationConfig.delayBetweenDispatchQueueBuilds;
			}
		}

		internal static bool AggregationSubscriptionsEnabled
		{
			get
			{
				return ContentAggregationConfig.aggregationSubscriptionsEnabled;
			}
		}

		internal static bool MigrationSubscriptionsEnabled
		{
			get
			{
				return ContentAggregationConfig.migrationSubscriptionsEnabled;
			}
		}

		internal static bool PeopleConnectionSubscriptionsEnabled
		{
			get
			{
				return ContentAggregationConfig.peopleConnectionSubscriptionsEnabled;
			}
		}

		internal static bool IsDatacenterMode
		{
			get
			{
				if (!ContentAggregationConfig.checkedDatacenterMode)
				{
					try
					{
						ContentAggregationConfig.datacenterMode = (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled || SyncUtilities.IsEnabledInEnterprise());
					}
					catch (CannotDetermineExchangeModeException ex)
					{
						ContentAggregationConfig.Tracer.TraceError<string>(0L, "Failed to determine the datacenter mode. Will Defaulting to Enterprise mode: {0}", ex.Message);
					}
					ContentAggregationConfig.checkedDatacenterMode = true;
				}
				return ContentAggregationConfig.datacenterMode;
			}
		}

		internal static int MaxDispatcherThreads
		{
			get
			{
				if (ContentAggregationConfig.localServer != null)
				{
					return ContentAggregationConfig.localServer.MaxTransportSyncDispatchers;
				}
				return 5;
			}
		}

		internal static bool TransportSyncDispatchEnabled
		{
			get
			{
				return ContentAggregationConfig.IsDatacenterMode && (ContentAggregationConfig.localServer == null || ContentAggregationConfig.localServer.TransportSyncDispatchEnabled);
			}
		}

		internal static SyncHealthLogConfiguration SyncMailboxHealthLogConfiguration
		{
			get
			{
				if (ContentAggregationConfig.localServer != null)
				{
					return SyncHealthLogConfiguration.CreateSyncMailboxHealthLogConfiguration(ContentAggregationConfig.localServer);
				}
				return ContentAggregationConfig.defaultSyncMailboxHealthLogConfiguration;
			}
		}

		private static bool SyncLogEnabled
		{
			get
			{
				if (ContentAggregationConfig.localServer != null)
				{
					return ContentAggregationConfig.localServer.TransportSyncMailboxLogEnabled;
				}
				return ContentAggregationConfig.defaultSyncLogEnabled;
			}
		}

		private static SyncLoggingLevel SyncLogLoggingLevel
		{
			get
			{
				if (ContentAggregationConfig.localServer != null)
				{
					return ContentAggregationConfig.localServer.TransportSyncMailboxLogLoggingLevel;
				}
				return ContentAggregationConfig.defaultSyncLoggingLevel;
			}
		}

		private static string SyncLogFilePath
		{
			get
			{
				if (ContentAggregationConfig.localServer == null || ContentAggregationConfig.localServer.TransportSyncMailboxLogFilePath == null)
				{
					return ContentAggregationConfig.defaultRelativeSyncLogPath;
				}
				string text = ContentAggregationConfig.localServer.TransportSyncMailboxLogFilePath.ToString();
				if (string.IsNullOrEmpty(text))
				{
					return ContentAggregationConfig.defaultRelativeSyncLogPath;
				}
				return text;
			}
		}

		private static long SyncLogMaxAgeInHours
		{
			get
			{
				if (ContentAggregationConfig.localServer != null)
				{
					return (long)ContentAggregationConfig.localServer.TransportSyncMailboxLogMaxAge.TotalHours;
				}
				return 720L;
			}
		}

		private static long SyncLogMaxDirectorySizeInKb
		{
			get
			{
				if (ContentAggregationConfig.localServer != null)
				{
					return (long)ContentAggregationConfig.localServer.TransportSyncMailboxLogMaxDirectorySize.ToKB();
				}
				return (long)ByteQuantifiedSize.FromGB(10UL).ToKB();
			}
		}

		private static long SyncLogMaxFileSizeInKb
		{
			get
			{
				if (ContentAggregationConfig.localServer != null)
				{
					return (long)ContentAggregationConfig.localServer.TransportSyncMailboxLogMaxFileSize.ToKB();
				}
				return 10240L;
			}
		}

		internal static bool Start(bool includeADConfig)
		{
			bool result;
			lock (ContentAggregationConfig.syncRoot)
			{
				ContentAggregationConfig.Tracer.TraceDebug(0L, "Loading initial configuration for subscription manager.");
				string exePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Microsoft.Exchange.TransportSyncManagerSvc.exe");
				ContentAggregationConfig.configuration = ConfigurationManager.OpenExeConfiguration(exePath);
				if (includeADConfig)
				{
					Exception ex;
					if (!ContentAggregationConfig.TryLoad(true, out ex))
					{
						ContentAggregationConfig.Tracer.TraceError<Exception>(0L, "Failed to load initial AD configuration for subscription manager {0}.", ex);
						ContentAggregationConfig.LogEvent(TransportSyncManagerEventLogConstants.Tuple_SyncManagerConfigLoadFailed, null, new object[]
						{
							ex
						});
						return false;
					}
					ContentAggregationConfig.Tracer.TraceDebug(0L, "Successfully loaded initial AD configuration for subscription manager.");
					ContentAggregationConfig.LogEvent(TransportSyncManagerEventLogConstants.Tuple_SyncManagerConfigLoadSucceeded, null, new object[0]);
				}
				ContentAggregationConfig.OpenGlobalLogSession();
				CommonLoggingHelper.SyncLogSession = ContentAggregationConfig.SyncLogSession;
				ContentAggregationConfig.LoadConfigSettings();
				ContentAggregationConfig.LogADConfigDetails();
				result = true;
			}
			return result;
		}

		internal static void Shutdown()
		{
			lock (ContentAggregationConfig.eventHandlers)
			{
				ContentAggregationConfig.eventHandlers.Clear();
			}
			lock (ContentAggregationConfig.syncRoot)
			{
				if (ContentAggregationConfig.notificationCookie != null)
				{
					ADNotificationAdapter.UnregisterChangeNotification(ContentAggregationConfig.notificationCookie);
					ContentAggregationConfig.notificationCookie = null;
				}
			}
			ContentAggregationConfig.syncLogSession.LogDebugging((TSLID)81UL, ContentAggregationConfig.Tracer, 0L, "Shutdown completed for subscription manager configuration.", new object[0]);
		}

		internal static void LogEvent(EventLogEntry eventLogEntry)
		{
			SyncUtilities.ThrowIfArgumentNull("eventLogEntry", eventLogEntry);
			ContentAggregationConfig.LogEvent(eventLogEntry.Tuple, eventLogEntry.PeriodicKey, eventLogEntry.MessageArgs);
		}

		internal static bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			if (ContentAggregationConfig.SyncLogSession != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (messageArgs != null && messageArgs.Length > 0)
				{
					foreach (object value in messageArgs)
					{
						stringBuilder.Append(value);
						stringBuilder.Append(';');
					}
				}
				uint num = tuple.EventId & 65535U;
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)82UL, "Logged {0} event {1} with periodic key [{2}], args: [{3}]", new object[]
				{
					tuple.EntryType,
					num,
					periodicKey,
					stringBuilder.ToString()
				});
			}
			return ContentAggregationConfig.eventLogger.LogEvent(tuple, periodicKey, messageArgs);
		}

		internal static void LoadConfigSettings()
		{
			ContentAggregationConfig.popAggregationEnabled = ContentAggregationConfig.GetConfigBool("PopAggregationEnabled", true);
			ContentAggregationConfig.deltaSyncAggregationEnabled = ContentAggregationConfig.GetConfigBool("DeltaSyncAggregationEnabled", true);
			ContentAggregationConfig.imapAggregationEnabled = ContentAggregationConfig.GetConfigBool("ImapAggregationEnabled", true);
			ContentAggregationConfig.facebookAggregationEnabled = ContentAggregationConfig.GetConfigBool("FacebookAggregationEnabled", true);
			ContentAggregationConfig.linkedInAggregationEnabled = ContentAggregationConfig.GetConfigBool("LinkedInAggregationEnabled", true);
			ContentAggregationConfig.owaMailboxPolicyConstraintEnabled = ContentAggregationConfig.GetConfigBool("OwaMailboxPolicyConstraintEnabled", true);
			ContentAggregationConfig.aggregationIncrementalSyncInterval = ContentAggregationConfig.GetConfigTimeSpan("AggregationIncrementalSyncInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(7.0), TimeSpan.FromHours(1.0));
			ContentAggregationConfig.migrationIncrementalSyncInterval = ContentAggregationConfig.GetConfigTimeSpan("MigrationIncrementalSyncInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(100.0), TimeSpan.FromDays(1.0));
			ContentAggregationConfig.peopleConnectionInitialSyncInterval = ContentAggregationConfig.GetConfigTimeSpan("PeopleConnectionInitialSyncInterval", TimeSpan.FromSeconds(0.0), TimeSpan.FromDays(100.0), TimeSpan.FromSeconds(0.0));
			ContentAggregationConfig.peopleConnectionTriggeredSyncInterval = ContentAggregationConfig.GetConfigTimeSpan("PeopleConnectionTriggeredSyncInterval", TimeSpan.FromSeconds(0.0), TimeSpan.FromDays(100.0), TimeSpan.FromSeconds(0.0));
			ContentAggregationConfig.peopleConnectionIncrementalSyncInterval = ContentAggregationConfig.GetConfigTimeSpan("PeopleConnectionIncrementalSyncInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(15.0), TimeSpan.FromHours(12.0));
			ContentAggregationConfig.owaMailboxPolicyInducedDeleteInterval = ContentAggregationConfig.GetConfigTimeSpan("OwaMailboxPolicyInducedDeleteInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(7.0), TimeSpan.FromHours(1.0));
			ContentAggregationConfig.owaMailboxPolicyProbeInterval = ContentAggregationConfig.GetConfigTimeSpan("OwaMailboxPolicyProbeInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(365.0), TimeSpan.FromDays(14.0));
			ContentAggregationConfig.dispatchEntryExpirationTime = ContentAggregationConfig.GetConfigTimeSpan("DispatchEntryExpirationTime", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(7.0), TimeSpan.FromMinutes(30.0));
			ContentAggregationConfig.dispatchEntryExpirationCheckFrequency = ContentAggregationConfig.GetConfigTimeSpan("DispatchEntryExpirationCheckFrequency", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(5.0));
			ContentAggregationConfig.primingDispatchTime = ContentAggregationConfig.GetConfigTimeSpan("PrimingDispatchTime", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromSeconds(5.0));
			ContentAggregationConfig.migrationInitialSyncInterval = ContentAggregationConfig.GetConfigTimeSpan("MigrationInitialSyncInterval", TimeSpan.FromSeconds(0.0), TimeSpan.FromHours(24.0), TimeSpan.FromSeconds(0.0));
			ContentAggregationConfig.aggregationInitialSyncInterval = ContentAggregationConfig.GetConfigTimeSpan("AggregationInitialSyncInterval", TimeSpan.FromSeconds(0.0), TimeSpan.FromHours(24.0), TimeSpan.FromSeconds(0.0));
			ContentAggregationConfig.aggregationInitialSyncWeight = ContentAggregationConfig.GetConfigByte("AggregationInitialSyncWeight", 1, 100, 5);
			ContentAggregationConfig.aggregationSubscriptionSavedSyncWeight = ContentAggregationConfig.GetConfigByte("AggregationSubscriptionSavedSyncWeight", 1, 100, 2);
			ContentAggregationConfig.aggregationIncrementalSyncWeight = ContentAggregationConfig.GetConfigByte("AggregationIncrementalSyncWeight", 1, 100, 18);
			ContentAggregationConfig.migrationInitialSyncWeight = ContentAggregationConfig.GetConfigByte("MigrationInitialSyncWeight", 1, 100, 16);
			ContentAggregationConfig.migrationIncrementalSyncWeight = ContentAggregationConfig.GetConfigByte("MigrationIncrementalSyncWeight", 1, 100, 1);
			ContentAggregationConfig.migrationFinalizationSyncWeight = ContentAggregationConfig.GetConfigByte("MigrationFinalizationSyncWeight", 1, 100, 17);
			ContentAggregationConfig.owaLogonTriggeredSyncWeight = ContentAggregationConfig.GetConfigByte("OwaLogonTriggeredSyncWeight", 1, 100, 2);
			ContentAggregationConfig.owaRefreshButtonTriggeredSyncWeight = ContentAggregationConfig.GetConfigByte("OwaRefreshButtonTriggeredSyncWeight", 1, 100, 4);
			ContentAggregationConfig.owaSessionTriggeredSyncWeight = ContentAggregationConfig.GetConfigByte("OwaSessionTriggeredSyncWeight", 1, 100, 2);
			ContentAggregationConfig.owaTriggeredSyncNowTime = ContentAggregationConfig.GetConfigTimeSpan("OwaTriggeredSyncNowTime", TimeSpan.FromSeconds(0.0), TimeSpan.FromHours(24.0), TimeSpan.FromSeconds(0.0));
			ContentAggregationConfig.peopleConnectionInitialSyncWeight = ContentAggregationConfig.GetConfigByte("PeopleConnectionInitialSyncWeight", 1, 100, 20);
			ContentAggregationConfig.peopleConnectionTriggeredSyncWeight = ContentAggregationConfig.GetConfigByte("PeopleConnectionTriggerSyncWeight", 1, 100, 3);
			ContentAggregationConfig.peopleConnectionIncrementalSyncWeight = ContentAggregationConfig.GetConfigByte("PeopleConnectionIncrementalSyncWeight", 1, 100, 8);
			ContentAggregationConfig.owaMailboxPolicyInducedDeleteWeight = ContentAggregationConfig.GetConfigByte("OwaMailboxPolicyInducedDeleteWeight", 1, 100, 2);
			ContentAggregationConfig.syncNowTime = ContentAggregationConfig.GetConfigTimeSpan("SyncNowTime", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(24.0), TimeSpan.FromSeconds(1.0));
			ContentAggregationConfig.databasePollingInterval = ContentAggregationConfig.GetConfigTimeSpan("DatabasePollingInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(5.0));
			ContentAggregationConfig.mailboxTablePollingInterval = ContentAggregationConfig.GetConfigTimeSpan("MailboxTablePollingInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(45.0));
			ContentAggregationConfig.mailboxTableRetryPollingInterval = ContentAggregationConfig.GetConfigTimeSpan("MailboxTableRetryPollingInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(5.0));
			ContentAggregationConfig.mailboxTableTwoWayPollingInterval = ContentAggregationConfig.GetConfigTimeSpan("MailboxTableTwoWayPollingInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(1.0), TimeSpan.FromHours(24.0));
			ContentAggregationConfig.DelayBeforeMailboxTablePollingStarts = ContentAggregationConfig.GetConfigTimeSpan("DelayBeforeMailboxTablePollingStarts", TimeSpan.FromSeconds(0.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(15.0));
			ContentAggregationConfig.hubBusyPeriod = ContentAggregationConfig.GetConfigTimeSpan("HubBusyPeriod", TimeSpan.Zero, TimeSpan.FromDays(1.0), TimeSpan.FromSeconds(5.0));
			ContentAggregationConfig.hubInactivityPeriod = ContentAggregationConfig.GetConfigTimeSpan("HubInactivityPeriod", TimeSpan.Zero, TimeSpan.FromDays(1.0), TimeSpan.FromSeconds(15.0));
			ContentAggregationConfig.hubSubscriptionTypeNotSupportedPeriod = ContentAggregationConfig.GetConfigTimeSpan("HubSubscriptionTypeNotSupportedPeriod", TimeSpan.Zero, TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(1.0));
			ContentAggregationConfig.databaseBackoffTime = ContentAggregationConfig.GetConfigTimeSpan("DatabaseBackoffTime", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(1.0));
			ContentAggregationConfig.minimumDispatchWaitForFailedSync = ContentAggregationConfig.GetConfigTimeSpan("MinimumDispatchWaitForFailedSync", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(7.0), TimeSpan.FromMinutes(5.0));
			ContentAggregationConfig.workTypeBudgetManagerSlidingWindowLength = ContentAggregationConfig.GetConfigTimeSpan("WorkTypeBudgetManagerSlidingWindowLength", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(24.0), TimeSpan.FromMinutes(10.0));
			ContentAggregationConfig.workTypeBudgetManagerSlidingBucketLength = ContentAggregationConfig.GetConfigTimeSpan("WorkTypeBudgetManagerSlidingBucketLength", TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(60.0), TimeSpan.FromSeconds(5.0));
			ContentAggregationConfig.workTypeBudgetManagerSampleDispatchedWorkFrequency = ContentAggregationConfig.GetConfigTimeSpan("WorkTypeBudgetManagerSampleDispatchedWorkFrequency", TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(60.0), TimeSpan.FromSeconds(1.0));
			ContentAggregationConfig.maxCompletionThreads = ContentAggregationConfig.GetConfigInt("MaxCompletionThreads", 1, 1024, 16);
			ContentAggregationConfig.maxCacheRpcThreads = ContentAggregationConfig.GetConfigInt("MaxCacheRpcThreads", 1, 128, 4);
			ContentAggregationConfig.maxNotificationThreads = ContentAggregationConfig.GetConfigInt("MaxNotificationThreads", 1, 128, 4);
			ContentAggregationConfig.maxManualResetEventsInResourcePool = ContentAggregationConfig.GetConfigInt("MaxManualResetEventsInResourcePool", 0, 262144, 1024);
			ContentAggregationConfig.maxMailboxSessionsInResourcePool = ContentAggregationConfig.GetConfigInt("MaxMailboxSessionsInResourcePool", 0, 262144, 1024);
			ContentAggregationConfig.tokenWaitTimeOutInterval = ContentAggregationConfig.GetConfigTimeSpan("TokenWaitTimeOutInterval", TimeSpan.Zero, TimeSpan.FromDays(365.0), TimeSpan.FromMinutes(30.0));
			ContentAggregationConfig.dispatchOutageThreshold = ContentAggregationConfig.GetConfigTimeSpan("DispatchOutageThreshold", TimeSpan.Zero, TimeSpan.FromDays(365.0), TimeSpan.FromHours(1.0));
			ContentAggregationConfig.poolBackOffTimeInterval = ContentAggregationConfig.GetConfigTimeSpan("PoolBackOffTimeInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(5.0));
			ContentAggregationConfig.poolExpiryCheckInterval = ContentAggregationConfig.GetConfigTimeSpan("PoolExpiryCheckInterval", TimeSpan.FromMilliseconds(-1.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(1.0));
			ContentAggregationConfig.maxSystemMailboxSessionsUnusedPeriod = ContentAggregationConfig.GetConfigTimeSpan("MaxSystemMailboxSessionsUnusedPeriod", TimeSpan.FromMilliseconds(-1.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(5.0));
			ContentAggregationConfig.dispatcherBackOffTimeInSeconds = ContentAggregationConfig.GetConfigInt("DispatcherBackOffTimeInSeconds", 1, 60, 5);
			ContentAggregationConfig.maxNumberOfAttemptsBeforePoolBackOff = ContentAggregationConfig.GetConfigInt("MaxNumberOfAttemptsBeforePoolBackOff", 1, 60, 3);
			ContentAggregationConfig.sLAPerfCounterUpdateInterval = ContentAggregationConfig.GetConfigTimeSpan("SLAPerfCounterUpdateInterval", TimeSpan.FromMinutes(1.0), TimeSpan.FromDays(7.0), TimeSpan.FromMinutes(1.0));
			ContentAggregationConfig.slaExpiryBuckets = ContentAggregationConfig.GetConfigInt("SLAExpiryBuckets", 1, 50, 5);
			ContentAggregationConfig.slaDataBuckets = ContentAggregationConfig.GetConfigInt("SLADataBuckets", 1, 1000, 100);
			ContentAggregationConfig.pCExpiryInterval = ContentAggregationConfig.GetConfigTimeSpan("PercentileCountersExpiryInterval", TimeSpan.FromMinutes(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(5.0));
			ContentAggregationConfig.maxSyncsPerDB = ContentAggregationConfig.GetConfigInt("MaxSyncsPerDB", 0, 1000, 40);
			ContentAggregationConfig.cacheRepairEnabled = ContentAggregationConfig.GetConfigBool("CacheRepairEnabled", true);
			ContentAggregationConfig.maxCacheMessageRepairAttempts = ContentAggregationConfig.GetConfigInt("MaxCacheMessageRepairAttempts", 1, 1000, 5);
			ContentAggregationConfig.delayBeforeRepairThreadStarts = ContentAggregationConfig.GetConfigTimeSpan("DelayBeforeRepairThreadStarts", TimeSpan.FromMilliseconds(1.0), TimeSpan.FromDays(7.0), TimeSpan.FromSeconds(30.0));
			ContentAggregationConfig.delayBetweenDispatchQueueBuilds = ContentAggregationConfig.GetConfigTimeSpan("DelayBetweenDispatchQueueBuilds", TimeSpan.FromMilliseconds(1.0), TimeSpan.FromDays(7.0), TimeSpan.FromSeconds(30.0));
			ContentAggregationConfig.dispatcherDatabaseRefreshFrequency = ContentAggregationConfig.GetConfigTimeSpan("DispatcherDatabaseRefreshFrequency", TimeSpan.FromMilliseconds(1.0), TimeSpan.FromDays(7.0), TimeSpan.FromMinutes(1.0));
			ContentAggregationConfig.aggregationSubscriptionsEnabled = ContentAggregationConfig.GetConfigBool("AggregationSubscriptionsEnabled", true);
			ContentAggregationConfig.migrationSubscriptionsEnabled = ContentAggregationConfig.GetConfigBool("MigrationSubscriptionsEnabled", true);
			ContentAggregationConfig.peopleConnectionSubscriptionsEnabled = ContentAggregationConfig.GetConfigBool("PeopleConnectionSubscriptionsEnabled", true);
		}

		private static void LogADConfigDetails()
		{
			ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)464UL, "Config {0}:{1}.", new object[]
			{
				ServerSchema.TransportSyncDispatchEnabled.Name,
				ContentAggregationConfig.TransportSyncDispatchEnabled.ToString()
			});
			ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)516UL, "Config {0}:{1}.", new object[]
			{
				ServerSchema.MaxTransportSyncDispatchers.Name,
				ContentAggregationConfig.MaxDispatcherThreads.ToString()
			});
		}

		private static void LogSetting<T>(string label, T min, T max, T @default, T actual)
		{
			ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)83UL, "Config {0}:{1}.", new object[]
			{
				label,
				actual
			});
		}

		private static void LogSetting<T>(string label, T @default, T actual)
		{
			ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)84UL, "Config {0}:{1}.", new object[]
			{
				label,
				actual
			});
		}

		private static string GetAppSetting(string label)
		{
			if (ContentAggregationConfig.configuration.AppSettings.Settings[label] == null)
			{
				return null;
			}
			return ContentAggregationConfig.configuration.AppSettings.Settings[label].Value;
		}

		private static byte GetConfigByte(string label, byte min, byte max, byte defaultValue)
		{
			return (byte)ContentAggregationConfig.GetConfigInt(label, (int)min, (int)max, (int)defaultValue);
		}

		private static int GetConfigInt(string label, int min, int max, int defaultValue)
		{
			string text = null;
			try
			{
				text = ContentAggregationConfig.GetAppSetting(label);
			}
			catch (ConfigurationErrorsException)
			{
			}
			int num = defaultValue;
			if (string.IsNullOrEmpty(text) || !int.TryParse(text, out num) || num < min || num > max)
			{
				num = defaultValue;
			}
			ContentAggregationConfig.LogSetting<int>(label, min, max, defaultValue, num);
			return num;
		}

		private static decimal GetConfigDecimal(string label, decimal min, decimal max, decimal defaultValue)
		{
			string text = null;
			try
			{
				text = ContentAggregationConfig.GetAppSetting(label);
			}
			catch (ConfigurationErrorsException)
			{
			}
			decimal num = defaultValue;
			if (string.IsNullOrEmpty(text) || !decimal.TryParse(text, out num) || num < min || num > max)
			{
				num = defaultValue;
			}
			ContentAggregationConfig.LogSetting<decimal>(label, min, max, defaultValue, num);
			return num;
		}

		private static bool GetConfigBool(string label, bool defaultValue)
		{
			string value = null;
			try
			{
				value = ContentAggregationConfig.GetAppSetting(label);
			}
			catch (ConfigurationErrorsException)
			{
			}
			bool flag = defaultValue;
			if (string.IsNullOrEmpty(value) || !bool.TryParse(value, out flag))
			{
				flag = defaultValue;
			}
			ContentAggregationConfig.LogSetting<bool>(label, defaultValue, flag);
			return flag;
		}

		private static TimeSpan GetConfigTimeSpan(string label, TimeSpan min, TimeSpan max, TimeSpan defaultValue)
		{
			string text = null;
			try
			{
				text = ContentAggregationConfig.GetAppSetting(label);
			}
			catch (ConfigurationErrorsException)
			{
			}
			TimeSpan timeSpan = defaultValue;
			if (string.IsNullOrEmpty(text) || !TimeSpan.TryParse(text, out timeSpan) || timeSpan < min || timeSpan > max)
			{
				timeSpan = defaultValue;
			}
			ContentAggregationConfig.LogSetting<TimeSpan>(label, min, max, defaultValue, timeSpan);
			return timeSpan;
		}

		private static float GetConfigFloat(string label, float min, float max, float defaultValue)
		{
			string text = null;
			try
			{
				text = ContentAggregationConfig.GetAppSetting(label);
			}
			catch (ConfigurationErrorsException)
			{
			}
			float num = defaultValue;
			if (string.IsNullOrEmpty(text) || !float.TryParse(text, out num) || num < min || num > max)
			{
				num = defaultValue;
			}
			ContentAggregationConfig.LogSetting<float>(label, min, max, defaultValue, num);
			return num;
		}

		private static string GetConfigString(string label, string defaultValue)
		{
			string text = null;
			try
			{
				text = ContentAggregationConfig.GetAppSetting(label);
			}
			catch (ConfigurationErrorsException)
			{
			}
			if (string.IsNullOrEmpty(text))
			{
				text = defaultValue;
			}
			ContentAggregationConfig.LogSetting<string>(label, defaultValue, text);
			return text;
		}

		private static bool TryLoad(bool subscribeForNotifications, out Exception exception)
		{
			exception = null;
			Server tmpLocalServer = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2685, "TryLoad", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Manager\\ContentAggregationConfig.cs");
				tmpLocalServer = topologyConfigurationSession.FindLocalServer();
				ContentAggregationConfig.Tracer.TraceDebug(0L, "Loaded local server object.");
				if (subscribeForNotifications)
				{
					ContentAggregationConfig.notificationCookie = ADNotificationAdapter.RegisterChangeNotification<Server>(tmpLocalServer.Id, new ADNotificationCallback(ContentAggregationConfig.HandleConfigurationChange));
					ContentAggregationConfig.Tracer.TraceDebug(0L, "Subscribed to AD change notifications for local server object.");
				}
			});
			if (adoperationResult.Succeeded)
			{
				ContentAggregationConfig.localServer = tmpLocalServer;
				return true;
			}
			exception = adoperationResult.Exception;
			ContentAggregationConfig.Tracer.TraceError(0L, "AD operation failed; details: {0}", new object[]
			{
				adoperationResult.Exception ?? "<null>"
			});
			return false;
		}

		private static void HandleConfigurationChange(ADNotificationEventArgs args)
		{
			Exception ex;
			bool flag2;
			lock (ContentAggregationConfig.syncRoot)
			{
				if (ContentAggregationConfig.eventHandlers == null)
				{
					return;
				}
				ContentAggregationConfig.syncLogSession.LogDebugging((TSLID)85UL, ContentAggregationConfig.Tracer, "Detected configuration change.", new object[0]);
				flag2 = ContentAggregationConfig.TryLoad(false, out ex);
				if (flag2)
				{
					ContentAggregationConfig.UpdateConfiguration();
				}
			}
			if (flag2)
			{
				ContentAggregationConfig.syncLogSession.LogDebugging((TSLID)86UL, ContentAggregationConfig.Tracer, "Successfully updated configuration; invoking custom handlers.", new object[0]);
				ContentAggregationConfig.InvokeEventHandlers();
				ContentAggregationConfig.LogEvent(TransportSyncManagerEventLogConstants.Tuple_SyncManagerConfigUpdateSucceeded, null, new object[0]);
				return;
			}
			ContentAggregationConfig.syncLogSession.LogError((TSLID)87UL, ContentAggregationConfig.Tracer, "Failed to updated configuration; continue using previous version or defaults. {0}", new object[]
			{
				ex
			});
			ContentAggregationConfig.LogEvent(TransportSyncManagerEventLogConstants.Tuple_SyncManagerConfigUpdateFailed, null, new object[0]);
		}

		private static void InvokeEventHandlers()
		{
			ContentAggregationConfig.ConfigurationChangedEventHandler[] array;
			lock (ContentAggregationConfig.eventHandlers)
			{
				array = ContentAggregationConfig.eventHandlers.ToArray();
			}
			foreach (ContentAggregationConfig.ConfigurationChangedEventHandler configurationChangedEventHandler in array)
			{
				configurationChangedEventHandler();
			}
		}

		private static void OpenGlobalLogSession()
		{
			if (ContentAggregationConfig.syncLog == null)
			{
				SyncLogConfiguration syncLogConfiguration = ContentAggregationConfig.LoadSyncLogConfig();
				ContentAggregationConfig.syncLog = new SyncLog(syncLogConfiguration);
				ContentAggregationConfig.syncLogSession = ContentAggregationConfig.syncLog.OpenGlobalSession();
				return;
			}
			ContentAggregationConfig.UpdateConfiguration();
		}

		private static SyncLogConfiguration LoadSyncLogConfig()
		{
			return new SyncLogConfiguration
			{
				Enabled = (ContentAggregationConfig.IsDatacenterMode && ContentAggregationConfig.SyncLogEnabled),
				LogFilePath = ContentAggregationConfig.SyncLogFilePath,
				AgeQuotaInHours = ContentAggregationConfig.SyncLogMaxAgeInHours,
				DirectorySizeQuota = ContentAggregationConfig.SyncLogMaxDirectorySizeInKb,
				PerFileSizeQuota = ContentAggregationConfig.SyncLogMaxFileSizeInKb,
				SyncLoggingLevel = ContentAggregationConfig.SyncLogLoggingLevel
			};
		}

		private static void UpdateConfiguration()
		{
			ContentAggregationConfig.syncLog.ConfigureLog(ContentAggregationConfig.IsDatacenterMode && ContentAggregationConfig.SyncLogEnabled, ContentAggregationConfig.SyncLogFilePath, ContentAggregationConfig.SyncLogMaxAgeInHours, ContentAggregationConfig.SyncLogMaxDirectorySizeInKb, ContentAggregationConfig.SyncLogMaxFileSizeInKb, ContentAggregationConfig.SyncLogLoggingLevel);
		}

		public const string PopAggregationEnabledConfig = "PopAggregationEnabled";

		public const string DeltaSyncAggregationEnabledConfig = "DeltaSyncAggregationEnabled";

		public const string ImapAggregationEnabledConfig = "ImapAggregationEnabled";

		public const string FacebookAggregationEnabledConfig = "FacebookAggregationEnabled";

		public const string LinkedInAggregationEnabledConfig = "LinkedInAggregationEnabled";

		public const string OwaMailboxPolicyConstraintEnabledConfig = "OwaMailboxPolicyConstraintEnabled";

		public const string AggregationIncrementalSyncIntervalConfig = "AggregationIncrementalSyncInterval";

		public const string MigrationIncrementalSyncIntervalConfig = "MigrationIncrementalSyncInterval";

		public const string PeopleConnectionInitialSyncIntervalConfig = "PeopleConnectionInitialSyncInterval";

		public const string PeopleConnectionTriggeredSyncIntervalConfig = "PeopleConnectionTriggeredSyncInterval";

		public const string PeopleConnectionIncrementalSyncIntervalConfig = "PeopleConnectionIncrementalSyncInterval";

		public const string OwaMailboxPolicyInducedDeleteIntervalConfig = "OwaMailboxPolicyInducedDeleteInterval";

		public const string OwaMailboxPolicyProbeIntervalConfig = "OwaMailboxPolicyProbeInterval";

		public const string DispatchEntryExpirationTimeConfig = "DispatchEntryExpirationTime";

		public const string DispatchEntryExpirationCheckFrequencyConfig = "DispatchEntryExpirationCheckFrequency";

		public const string PrimingDispatchTimeConfig = "PrimingDispatchTime";

		public const string MigrationInitialSyncIntervalConfig = "MigrationInitialSyncInterval";

		public const string AggregationInitialSyncIntervalConfig = "AggregationInitialSyncInterval";

		public const string AggregationInitialSyncWeightConfig = "AggregationInitialSyncWeight";

		public const string AggregationSubscriptionSavedSyncWeightConfig = "AggregationSubscriptionSavedSyncWeight";

		public const string AggregationIncrementalSyncWeightConfig = "AggregationIncrementalSyncWeight";

		public const string MigrationInitialSyncWeightConfig = "MigrationInitialSyncWeight";

		public const string MigrationIncrementalSyncWeightConfig = "MigrationIncrementalSyncWeight";

		public const string MigrationFinalizationSyncWeightConfig = "MigrationFinalizationSyncWeight";

		public const string SyncNowTimeConfig = "SyncNowTime";

		public const string DatabasePollingIntervalConfig = "DatabasePollingInterval";

		public const string MailboxTablePollingIntervalConfig = "MailboxTablePollingInterval";

		public const string MailboxTableRetryPollingIntervalConfig = "MailboxTableRetryPollingInterval";

		public const string MailboxTableTwoWayPollingIntervalConfig = "MailboxTableTwoWayPollingInterval";

		public const string DelayBeforeMailboxTablePollingStartsConfig = "DelayBeforeMailboxTablePollingStarts";

		public const string SubscriptionSyncTimeOutIntervalConfig = "SubscriptionSyncTimeOutInterval";

		public const string HubBusyPeriodConfig = "HubBusyPeriod";

		public const string HubInactivityPeriodConfig = "HubInactivityPeriod";

		public const string HubSubscriptionTypeNotSupportedPeriodConfig = "HubSubscriptionTypeNotSupportedPeriod";

		public const string DatabaseBackoffTimeConfig = "DatabaseBackoffTime";

		public const string MinimumDispatchWaitForFailedSyncConfig = "MinimumDispatchWaitForFailedSync";

		public const string WorkTypeBudgetManagerSlidingWindowLengthConfig = "WorkTypeBudgetManagerSlidingWindowLength";

		public const string WorkTypeBudgetManagerSlidingBucketLengthConfig = "WorkTypeBudgetManagerSlidingBucketLength";

		public const string WorkTypeBudgetManagerSampleDispatchedWorkFrequencyConfig = "WorkTypeBudgetManagerSampleDispatchedWorkFrequency";

		public const string MaxCompletionThreadsConfig = "MaxCompletionThreads";

		public const string MaxCacheRpcThreadsConfig = "MaxCacheRpcThreads";

		public const string MaxNotificationThreadsConfig = "MaxNotificationThreads";

		public const string MaxManualResetEventsInResourcePoolConfig = "MaxManualResetEventsInResourcePool";

		public const string MaxMailboxSessionsInResourcePoolConfig = "MaxMailboxSessionsInResourcePool";

		public const string TokenWaitTimeOutIntervalConfig = "TokenWaitTimeOutInterval";

		public const string DispatchOutageThresholdConfig = "DispatchOutageThreshold";

		public const string PoolBackOffTimeIntervalConfig = "PoolBackOffTimeInterval";

		public const string PoolExpiryCheckIntervalConfig = "PoolExpiryCheckInterval";

		public const string MaxSystemMailboxSessionsUnusedPeriodConfig = "MaxSystemMailboxSessionsUnusedPeriod";

		public const string DispatcherBackOffTimeInSecondsConfig = "DispatcherBackOffTimeInSeconds";

		public const string MaxNumberOfAttemptsBeforePoolBackOffConfig = "MaxNumberOfAttemptsBeforePoolBackOff";

		public const string SLAPerfCounterUpdateIntervalConfig = "SLAPerfCounterUpdateInterval";

		public const string SlaExpiryBucketsConfig = "SLAExpiryBuckets";

		public const string SlaDataBucketsConfig = "SLADataBuckets";

		public const string PCExpiryIntervalConfig = "PercentileCountersExpiryInterval";

		public const string MaxSyncsPerDBConfig = "MaxSyncsPerDB";

		public const string CacheRepairEnabledConfig = "CacheRepairEnabled";

		public const string MaxCacheMessageRepairAttemptsConfig = "MaxCacheMessageRepairAttempts";

		public const string DelayBeforeRepairThreadStartsConfig = "DelayBeforeRepairThreadStarts";

		public const string DelayBetweenDispatchQueueBuildsConfig = "DelayBetweenDispatchQueueBuilds";

		public const string DispatcherDatabaseRefreshFrequencyConfig = "DispatcherDatabaseRefreshFrequency";

		private const string ConfigDetailFormat = "Config {0}:{1}.";

		private const string ServiceFileName = "Microsoft.Exchange.TransportSyncManagerSvc.exe";

		private static readonly Trace Tracer = ExTraceGlobals.ContentAggregationConfigTracer;

		private static readonly ExEventLog eventLogger = new ExEventLog(new Guid("{DF4B5565-53E9-4776-A824-185F22FB3CA6}"), "MSExchangeTransportSyncManager");

		private static readonly string defaultRelativeSyncLogPath = "TransportRoles\\Logs\\SyncLog\\Mailbox";

		private static readonly bool defaultSyncLogEnabled = false;

		private static readonly SyncLoggingLevel defaultSyncLoggingLevel = SyncLoggingLevel.None;

		private static readonly SyncHealthLogConfiguration defaultSyncMailboxHealthLogConfiguration = new SyncHealthLogConfiguration();

		private static TimeSpan migrationInitialSyncInterval;

		private static TimeSpan aggregationInitialSyncInterval;

		private static TimeSpan aggregationIncrementalSyncInterval;

		private static TimeSpan migrationIncrementalSyncInterval;

		private static TimeSpan peopleConnectionInitialSyncInterval;

		private static TimeSpan peopleConnectionTriggeredSyncInterval;

		private static TimeSpan peopleConnectionIncrementalSyncInterval;

		private static TimeSpan owaMailboxPolicyInducedDeleteInterval;

		private static TimeSpan owaMailboxPolicyProbeInterval;

		private static byte aggregationSubscriptionSavedSyncWeight;

		private static byte aggregationIncrementalSyncWeight;

		private static byte migrationInitialSyncWeight;

		private static byte aggregationInitialSyncWeight;

		private static byte migrationFinalizationSyncWeight;

		private static byte migrationIncrementalSyncWeight;

		private static byte owaLogonTriggeredSyncWeight;

		private static byte owaRefreshButtonTriggeredSyncWeight;

		private static byte owaSessionTriggeredSyncWeight;

		private static byte peopleConnectionInitialSyncWeight;

		private static byte peopleConnectionTriggeredSyncWeight;

		private static byte peopleConnectionIncrementalSyncWeight;

		private static byte owaMailboxPolicyInducedDeleteWeight;

		private static TimeSpan dispatchEntryExpirationTime;

		private static TimeSpan dispatchEntryExpirationCheckFrequency;

		private static TimeSpan primingDispatchTime;

		private static TimeSpan syncNowTime;

		private static TimeSpan owaTriggeredSyncNowTime;

		private static TimeSpan databasePollingInterval;

		private static TimeSpan mailboxTablePollingInterval;

		private static TimeSpan mailboxTableRetryPollingInterval;

		private static TimeSpan mailboxTableTwoWayPollingInterval;

		private static TimeSpan delayBeforeMailboxTablePollingStarts;

		private static TimeSpan hubBusyPeriod;

		private static TimeSpan hubInactivityPeriod;

		private static TimeSpan hubSubscriptionTypeNotSupportedPeriod;

		private static TimeSpan databaseBackoffTime;

		private static TimeSpan minimumDispatchWaitForFailedSync;

		private static TimeSpan workTypeBudgetManagerSlidingWindowLength;

		private static TimeSpan workTypeBudgetManagerSlidingBucketLength;

		private static TimeSpan workTypeBudgetManagerSampleDispatchedWorkFrequency;

		private static int maxCompletionThreads;

		private static int maxCacheRpcThreads;

		private static int maxNotificationThreads;

		private static int maxManualResetEventsInResourcePool;

		private static int maxMailboxSessionsInResourcePool;

		private static TimeSpan tokenWaitTimeOutInterval;

		private static TimeSpan dispatchOutageThreshold;

		private static TimeSpan poolBackOffTimeInterval;

		private static TimeSpan poolExpiryCheckInterval;

		private static TimeSpan maxSystemMailboxSessionsUnusedPeriod;

		private static int dispatcherBackOffTimeInSeconds;

		private static int maxNumberOfAttemptsBeforePoolBackOff;

		private static TimeSpan sLAPerfCounterUpdateInterval;

		private static int slaExpiryBuckets;

		private static int slaDataBuckets;

		private static TimeSpan pCExpiryInterval;

		private static int maxSyncsPerDB;

		private static bool cacheRepairEnabled;

		private static int maxCacheMessageRepairAttempts;

		private static TimeSpan delayBeforeRepairThreadStarts;

		private static TimeSpan delayBetweenDispatchQueueBuilds;

		private static bool popAggregationEnabled;

		private static bool deltaSyncAggregationEnabled;

		private static bool imapAggregationEnabled;

		private static bool facebookAggregationEnabled;

		private static bool linkedInAggregationEnabled;

		private static bool owaMailboxPolicyConstraintEnabled;

		private static TimeSpan dispatcherDatabaseRefreshFrequency;

		private static bool aggregationSubscriptionsEnabled;

		private static bool migrationSubscriptionsEnabled;

		private static bool peopleConnectionSubscriptionsEnabled;

		private static bool datacenterMode;

		private static bool checkedDatacenterMode;

		private static volatile Server localServer;

		private static ADNotificationRequestCookie notificationCookie;

		private static List<ContentAggregationConfig.ConfigurationChangedEventHandler> eventHandlers = new List<ContentAggregationConfig.ConfigurationChangedEventHandler>();

		private static object syncRoot = new object();

		private static SyncLog syncLog;

		private static GlobalSyncLogSession syncLogSession;

		private static Configuration configuration = null;

		internal delegate void ConfigurationChangedEventHandler();
	}
}
