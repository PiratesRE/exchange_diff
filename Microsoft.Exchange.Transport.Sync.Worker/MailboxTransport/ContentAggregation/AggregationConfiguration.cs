using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Net.Logging;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog;
using Microsoft.Exchange.Transport.Sync.Worker.Health;
using Microsoft.Exchange.Transport.Sync.Worker.Throttling;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregationConfiguration : IRemoteServerHealthConfiguration
	{
		protected AggregationConfiguration()
		{
			this.InitializeConfiguration();
		}

		private AggregationConfiguration(ITransportConfiguration transportConfiguration)
		{
			SyncUtilities.ThrowIfArgumentNull("transportConfiguration", transportConfiguration);
			this.transportConfiguration = transportConfiguration;
			this.UpdateConfiguration(this.transportConfiguration.LocalServer.TransportServer);
			this.InitializeConfiguration();
		}

		protected void InitializeConfiguration()
		{
			this.disabledSubscriptionAgents = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			this.initialRetryInMilliseconds = 2000;
			this.retryBackoffFactor = 2;
			this.maxItemsForDBInManualConcurrencyMode = 0;
			this.delayTresholdForAcceptingNewWorkItems = 0;
			this.contentAggregationProxyServer = WebRequest.DefaultWebProxy;
			SyncPoisonHandler.PoisonContextExpiry = TimeSpan.FromDays(2.0);
			this.terminateSlowSyncEnabled = true;
			this.syncDurationThreshold = TimeSpan.FromMinutes(10.0);
			this.remoteRoundtripTimeThreshold = TimeSpan.FromSeconds(4.0);
			this.delayedSubscriptionThreshold = TimeSpan.FromHours(4.0);
			this.disableSubscriptionThreshold = TimeSpan.FromDays(5.0);
			this.extendedDisableSubscriptionThreshold = TimeSpan.FromDays(30.0);
			this.outageDetectionThreshold = TimeSpan.FromHours(28.0);
			this.hubInactivityThreshold = TimeSpan.FromHours(1.0);
			this.healthCheckRetryInterval = TimeSpan.FromMinutes(5.0);
			this.subscriptionNotificationEnabled = true;
			this.maxTransientErrorsPerItem = 3;
			this.maxMappingTableSizeInMemory = ByteQuantifiedSize.FromMB(150UL);
			this.ReadAppConfig();
			this.remoteServerHealthManagementEnabled = AggregationConfiguration.GetConfigBoolValue("RemoteServerHealthManagementEnabled", true);
			this.remoteServerPoisonMarkingEnabled = AggregationConfiguration.GetConfigBoolValue("RemoteServerPoisonMarkingEnabled", false);
			AggregationConfiguration.LoadConfigSlidingCounterValues("RemoteServerSlidingLatencyCounterWindowSize", TimeSpan.FromMinutes(15.0), out this.remoteServerLatencySlidingCounterWindowSize, "RemoteServerSlidingLatencyCounterBucketLength", TimeSpan.FromSeconds(30.0), out this.remoteServerLatencySlidingCounterBucketLength);
			this.remoteServerLatencyThreshold = AggregationConfiguration.GetConfigTimeSpanValue("RemoteServerLatencyThreshold", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromSeconds(3.0));
			this.remoteServerBackoffCountLimit = AggregationConfiguration.GetConfigIntValue("RemoteServerBackoffCountLimit", 0, int.MaxValue, 5);
			this.remoteServerBackoffTimeSpan = AggregationConfiguration.GetConfigTimeSpanValue("RemoteServerBackoffTimeSpan", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromHours(1.5));
			this.remoteServerHealthDataExpiryPeriod = AggregationConfiguration.GetConfigTimeSpanValue("RemoteServerHealthDataExpiryPeriod", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(1.5));
			this.remoteServerHealthDataExpiryAndPersistanceFrequency = AggregationConfiguration.GetConfigTimeSpanValue("RemoteServerHealthDataExpiryAndPersistanceFrequency", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromMinutes(30.0));
			this.remoteServerAllowedCapacityUsagePercentage = AggregationConfiguration.GetConfigDoubleValue("RemoteServerAllowedCapacityUsagePercentage", 0.0, 100.0, 20.0);
			this.alwaysLoadSubscription = AggregationConfiguration.GetConfigBoolValue("AlwaysLoadSubscription", false);
			this.reportDataLoggingDisabled = AggregationConfiguration.GetConfigBoolValue("ReportDataLoggingDisabled", false);
			this.cloudStatisticsCollectionDisabled = AggregationConfiguration.GetConfigBoolValue("CloudStatisticsCollectionDisabled", false);
			this.maxItemsForMailboxServerInUnknownHealthState = AggregationConfiguration.GetConfigIntValue("MaxItemsForMailboxServerInUnknownHealthState", 0, int.MaxValue, 5);
			this.maxDownloadItemsInFirstDeltaSyncConnection = AggregationConfiguration.GetConfigIntValue("MaxDownloadItemsInFirstDeltaSyncConnection", 0, int.MaxValue, AggregationConfiguration.DefaultMaxDownloadItemsInFirstDeltaSyncConnection);
			AggregationConfiguration.ReadEnumCollectionConfigValueAndUpdateHashSet<AggregationType>("AggregationTypesToBeSyncedInRecoveryMode", this.aggregationTypesToBeSyncedInRecoveryMode, new AggregationType[]
			{
				AggregationType.Migration
			});
			AggregationConfiguration.ReadEnumCollectionConfigValueAndUpdateHashSet<AggregationSubscriptionType>("SubscriptionTypesToBeSyncedInRecoveryMode", this.subscriptionTypesToBeSyncedInRecoveryMode, new AggregationSubscriptionType[]
			{
				AggregationSubscriptionType.DeltaSyncMail
			});
			string configName = "SyncPhasesToBeSyncedInRecoveryMode";
			HashSet<SyncPhase> configValues = this.syncPhasesToBeSyncedInRecoveryMode;
			SyncPhase[] defaultValues = new SyncPhase[1];
			AggregationConfiguration.ReadEnumCollectionConfigValueAndUpdateHashSet<SyncPhase>(configName, configValues, defaultValues);
			AggregationConfiguration.ReadEnumCollectionConfigValueAndUpdateHashSet<SyncResourceMonitorType>("SyncResourceMonitorsDisabled", this.syncResourceMonitorsDisabled, new SyncResourceMonitorType[0]);
			this.suggestedConcurrencyOverride = AggregationConfiguration.GetConfigIntValue("SuggestedConcurrencyOverride", int.MinValue, int.MaxValue, int.MinValue);
			this.maxDownloadSizePerConnectionForAggregation = AggregationConfiguration.GetConfigByteQuantifiedSizeValue("MaxDownloadSizePerConnectionForAggregation", ByteQuantifiedSize.FromMB(50UL));
			this.maxDownloadSizePerConnectionForPeopleConnection = AggregationConfiguration.GetConfigByteQuantifiedSizeValue("MaxDownloadSizePerConnectionForPeopleConnection", ByteQuantifiedSize.FromMB(50UL));
			this.minFreeSpaceRequired = AggregationConfiguration.GetConfigByteQuantifiedSizeValue("MinFreeSpaceRequired", ByteQuantifiedSize.FromGB(1024UL));
			this.maxItemsForDBInUnknownHealthState = AggregationConfiguration.GetConfigIntValue("MaxItemsForDBInUnknownHealthState", 0, int.MaxValue, 2);
			this.maxItemsForTransportServerInUnknownHealthState = AggregationConfiguration.GetConfigIntValue("MaxItemsForTransportServerInUnknownHealthState", 0, int.MaxValue, 5);
			this.maxPendingMessagesInTransportQueueForTheServer = AggregationConfiguration.GetConfigIntValue("MaxPendingMessagesInTransportQueueForTheServer", 0, int.MaxValue, 300);
			this.maxPendingMessagesInTransportQueuePerUser = AggregationConfiguration.GetConfigIntValue("MaxPendingMessagesInTransportQueuePerUser", 0, int.MaxValue, 100);
			this.maxDownloadItemsPerConnectionForAggregation = AggregationConfiguration.GetConfigIntValue("MaxDownloadItemsPerConnectionForAggregation", 0, this.maxPendingMessagesInTransportQueuePerUser, 100);
			this.maxDownloadItemsPerConnectionForPeopleConnection = AggregationConfiguration.GetConfigIntValue("MaxDownloadItemsPerConnectionForPeopleConnection", 0, int.MaxValue, 500);
		}

		public static AggregationConfiguration Instance
		{
			get
			{
				if (AggregationConfiguration.instance == null)
				{
					lock (AggregationConfiguration.instanceInitializationLock)
					{
						if (AggregationConfiguration.instance == null)
						{
							AggregationConfiguration.instance = new AggregationConfiguration(Components.Configuration);
						}
					}
				}
				return AggregationConfiguration.instance;
			}
		}

		public static bool IsDatacenterMode
		{
			get
			{
				if (!AggregationConfiguration.checkedDatacenterMode)
				{
					try
					{
						AggregationConfiguration.datacenterMode = (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled || SyncUtilities.IsEnabledInEnterprise());
					}
					catch (CannotDetermineExchangeModeException ex)
					{
						AggregationConfiguration.diag.TraceError<string>(0L, "Failed to determine the datacenter mode. Will Defaulting to Enterprise mode: {0}", ex.Message);
					}
					AggregationConfiguration.checkedDatacenterMode = true;
				}
				return AggregationConfiguration.datacenterMode;
			}
		}

		public virtual Server LocalTransportServer
		{
			get
			{
				return this.transportConfiguration.LocalServer.TransportServer;
			}
		}

		public virtual bool IsEnabled
		{
			get
			{
				return AggregationConfiguration.IsDatacenterMode && this.LocalTransportServer.TransportSyncEnabled;
			}
		}

		public virtual int MaximumPendingWorkItems
		{
			get
			{
				return this.LocalTransportServer.MaxActiveTransportSyncJobsPerProcessor * AggregationConfiguration.processorCount;
			}
		}

		public virtual int MaximumNumberOfAttempts
		{
			get
			{
				return this.LocalTransportServer.MaxNumberOfTransportSyncAttempts;
			}
		}

		public int InitialRetryInMilliseconds
		{
			get
			{
				return this.initialRetryInMilliseconds;
			}
		}

		public int RetryBackoffFactor
		{
			get
			{
				return this.retryBackoffFactor;
			}
		}

		public int DelayTresholdForAcceptingNewWorkItems
		{
			get
			{
				return this.delayTresholdForAcceptingNewWorkItems;
			}
		}

		public int MaxItemsForDBInUnknownHealthState
		{
			get
			{
				return this.maxItemsForDBInUnknownHealthState;
			}
		}

		public int MaxItemsForTransportServerInUnknownHealthState
		{
			get
			{
				return this.maxItemsForTransportServerInUnknownHealthState;
			}
			set
			{
				this.maxItemsForTransportServerInUnknownHealthState = value;
			}
		}

		public int MaxItemsForMailboxServerInUnknownHealthState
		{
			get
			{
				return this.maxItemsForMailboxServerInUnknownHealthState;
			}
		}

		public int MaxItemsForDBInManualConcurrencyMode
		{
			get
			{
				return this.maxItemsForDBInManualConcurrencyMode;
			}
		}

		public AggregationSubscriptionType EnabledAggregationTypes
		{
			get
			{
				return this.enabledAggregationTypes;
			}
		}

		public ProtocolLog HttpProtocolLog
		{
			get
			{
				return this.httpProtocolLog;
			}
		}

		public SyncLog SyncLog
		{
			get
			{
				return this.syncLog;
			}
		}

		public SyncHealthLog SyncHealthLog
		{
			get
			{
				return this.syncHealthLog;
			}
		}

		public int RemoteConnectionTimeout
		{
			get
			{
				return (int)this.LocalTransportServer.TransportSyncRemoteConnectionTimeout.TotalMilliseconds;
			}
		}

		public IWebProxy ContentAggregationProxyServer
		{
			get
			{
				return this.contentAggregationProxyServer;
			}
		}

		public virtual long MaxDownloadSizePerItem
		{
			get
			{
				return (long)this.LocalTransportServer.TransportSyncMaxDownloadSizePerItem.ToBytes();
			}
		}

		public int MaxDownloadItemsInFirstDeltaSyncConnection
		{
			get
			{
				return this.maxDownloadItemsInFirstDeltaSyncConnection;
			}
		}

		public HashSet<AggregationSubscriptionType> SubscriptionTypesToBeSyncedInRecoveryMode
		{
			get
			{
				return this.subscriptionTypesToBeSyncedInRecoveryMode;
			}
		}

		public HashSet<AggregationType> AggregationTypesToBeSyncedInRecoveryMode
		{
			get
			{
				return this.aggregationTypesToBeSyncedInRecoveryMode;
			}
		}

		public HashSet<SyncPhase> SyncPhasesToBeSyncedInRecoveryMode
		{
			get
			{
				return this.syncPhasesToBeSyncedInRecoveryMode;
			}
		}

		public HashSet<SyncResourceMonitorType> SyncResourceMonitorsDisabled
		{
			get
			{
				return this.syncResourceMonitorsDisabled;
			}
		}

		public int? SuggestedConcurrencyOverride
		{
			get
			{
				if (this.suggestedConcurrencyOverride >= 0)
				{
					return new int?(this.suggestedConcurrencyOverride);
				}
				return null;
			}
		}

		public bool TerminateSlowSyncEnabled
		{
			get
			{
				return this.terminateSlowSyncEnabled;
			}
		}

		public TimeSpan SyncDurationThreshold
		{
			get
			{
				return this.syncDurationThreshold;
			}
		}

		public TimeSpan RemoteRoundtripTimeThreshold
		{
			get
			{
				return this.remoteRoundtripTimeThreshold;
			}
		}

		public TimeSpan DelayedSubscriptionThreshold
		{
			get
			{
				return this.delayedSubscriptionThreshold;
			}
		}

		public TimeSpan DisableSubscriptionThreshold
		{
			get
			{
				return this.disableSubscriptionThreshold;
			}
		}

		public TimeSpan ExtendedDisableSubscriptionThreshold
		{
			get
			{
				return this.extendedDisableSubscriptionThreshold;
			}
		}

		public TimeSpan OutageDetectionThreshold
		{
			get
			{
				return this.outageDetectionThreshold;
			}
		}

		public TimeSpan HubInactivityThreshold
		{
			get
			{
				return this.hubInactivityThreshold;
			}
		}

		public TimeSpan HealthCheckRetryInterval
		{
			get
			{
				return this.healthCheckRetryInterval;
			}
		}

		public ICollection<string> DisabledSubscriptionAgents
		{
			get
			{
				return this.disabledSubscriptionAgents;
			}
		}

		public bool SubscriptionNotificationEnabled
		{
			get
			{
				return this.subscriptionNotificationEnabled;
			}
			set
			{
				this.subscriptionNotificationEnabled = value;
			}
		}

		public int MaxTransientErrorsPerItem
		{
			get
			{
				return this.maxTransientErrorsPerItem;
			}
		}

		public TimeSpan RemoteServerLatencySlidingCounterWindowSize
		{
			get
			{
				return this.remoteServerLatencySlidingCounterWindowSize;
			}
		}

		public TimeSpan RemoteServerLatencySlidingCounterBucketLength
		{
			get
			{
				return this.remoteServerLatencySlidingCounterBucketLength;
			}
		}

		public TimeSpan RemoteServerLatencyThreshold
		{
			get
			{
				return this.remoteServerLatencyThreshold;
			}
		}

		public int RemoteServerBackoffCountLimit
		{
			get
			{
				return this.remoteServerBackoffCountLimit;
			}
		}

		public TimeSpan RemoteServerBackoffTimeSpan
		{
			get
			{
				return this.remoteServerBackoffTimeSpan;
			}
		}

		public TimeSpan RemoteServerHealthDataExpiryPeriod
		{
			get
			{
				return this.remoteServerHealthDataExpiryPeriod;
			}
		}

		public TimeSpan RemoteServerHealthDataExpiryAndPersistanceFrequency
		{
			get
			{
				return this.remoteServerHealthDataExpiryAndPersistanceFrequency;
			}
		}

		public double RemoteServerAllowedCapacityUsagePercentage
		{
			get
			{
				return this.remoteServerAllowedCapacityUsagePercentage;
			}
		}

		public TimeSpan RemoteServerCapacityUsageThreshold
		{
			get
			{
				double num = (double)((long)this.MaximumPendingWorkItems * this.RemoteServerLatencySlidingCounterWindowSize.Ticks);
				double num2 = this.RemoteServerAllowedCapacityUsagePercentage / 100.0;
				double num3 = num * num2;
				return TimeSpan.FromTicks((long)num3);
			}
		}

		public bool RemoteServerHealthManagementEnabled
		{
			get
			{
				return this.remoteServerHealthManagementEnabled;
			}
		}

		public bool RemoteServerPoisonMarkingEnabled
		{
			get
			{
				return this.remoteServerPoisonMarkingEnabled;
			}
		}

		public bool ReportDataLoggingDisabled
		{
			get
			{
				return this.reportDataLoggingDisabled;
			}
			set
			{
				this.reportDataLoggingDisabled = value;
			}
		}

		public bool CloudStatisticsCollectionDisabled
		{
			get
			{
				return this.cloudStatisticsCollectionDisabled;
			}
			set
			{
				this.cloudStatisticsCollectionDisabled = value;
			}
		}

		public ByteQuantifiedSize MaxMappingTableSizeInMemory
		{
			get
			{
				return this.maxMappingTableSizeInMemory;
			}
		}

		public ByteQuantifiedSize MinFreeSpaceRequired
		{
			get
			{
				return this.minFreeSpaceRequired;
			}
		}

		public bool AlwaysLoadSubscription
		{
			get
			{
				return this.alwaysLoadSubscription;
			}
		}

		public int MaxPendingMessagesInTransportQueueForTheServer
		{
			get
			{
				return this.maxPendingMessagesInTransportQueueForTheServer;
			}
			set
			{
				this.maxPendingMessagesInTransportQueueForTheServer = value;
			}
		}

		public int MaxPendingMessagesInTransportQueuePerUser
		{
			get
			{
				return this.maxPendingMessagesInTransportQueuePerUser;
			}
			set
			{
				this.maxPendingMessagesInTransportQueuePerUser = value;
			}
		}

		internal int MaxDownloadItemsPerConnectionForAggregation
		{
			get
			{
				return this.maxDownloadItemsPerConnectionForAggregation;
			}
			set
			{
				this.maxDownloadItemsPerConnectionForAggregation = value;
			}
		}

		internal ByteQuantifiedSize GetMaxDownloadSizePerConnection(AggregationType aggregationType)
		{
			if (aggregationType == AggregationType.Aggregation)
			{
				return this.maxDownloadSizePerConnectionForAggregation;
			}
			if (aggregationType == AggregationType.Migration)
			{
				return this.LocalTransportServer.TransportSyncMaxDownloadSizePerConnection;
			}
			if (aggregationType != AggregationType.PeopleConnection)
			{
				throw new InvalidOperationException("Unknown aggregation type: " + aggregationType);
			}
			return this.maxDownloadSizePerConnectionForPeopleConnection;
		}

		internal int GetMaxDownloadItemsPerConnection(AggregationType aggregationType)
		{
			if (aggregationType == AggregationType.Aggregation)
			{
				return this.maxDownloadItemsPerConnectionForAggregation;
			}
			if (aggregationType == AggregationType.Migration)
			{
				return this.LocalTransportServer.TransportSyncMaxDownloadItemsPerConnection;
			}
			if (aggregationType != AggregationType.PeopleConnection)
			{
				throw new InvalidOperationException("Unknown aggregation type: " + aggregationType);
			}
			return this.maxDownloadItemsPerConnectionForPeopleConnection;
		}

		internal void UpdateConfiguration(Server server)
		{
			SyncPoisonHandler.PoisonDetectionEnabled = (AggregationConfiguration.IsDatacenterMode && server.TransportSyncAccountsPoisonDetectionEnabled);
			SyncPoisonHandler.PoisonSubscriptionThreshold = server.TransportSyncAccountsPoisonAccountThreshold;
			SyncPoisonHandler.PoisonItemThreshold = server.TransportSyncAccountsPoisonItemThreshold;
			SyncPoisonHandler.MaxPoisonousItemsPerSubscriptionThreshold = server.TransportSyncAccountsSuccessivePoisonItemThreshold;
			this.enabledAggregationTypes = AggregationConfiguration.CreateAggregationTypes(server);
			string httpTransportSyncProxyServer = this.LocalTransportServer.HttpTransportSyncProxyServer;
			Uri address;
			if (httpTransportSyncProxyServer.Length == 0)
			{
				this.contentAggregationProxyServer = WebRequest.DefaultWebProxy;
			}
			else if (Uri.TryCreate(httpTransportSyncProxyServer, UriKind.Absolute, out address))
			{
				this.contentAggregationProxyServer = new WebProxy(address);
			}
			else
			{
				ExTraceGlobals.CommonTracer.TraceDebug<string>(0L, "Unable to create Uri for '{0}'.  Not modifying contentAggregationProxyServer.", httpTransportSyncProxyServer);
			}
			ProtocolLogConfiguration protocolLogConfiguration = AggregationConfiguration.CreateHttpProtocolLogConfiguration(server);
			if (this.httpProtocolLog == null)
			{
				this.httpProtocolLog = new ProtocolLog(protocolLogConfiguration);
			}
			else
			{
				this.httpProtocolLog.ConfigureLog(protocolLogConfiguration.LogFilePath, protocolLogConfiguration.AgeQuota, protocolLogConfiguration.DirectorySizeQuota, protocolLogConfiguration.PerFileSizeQuota, protocolLogConfiguration.ProtocolLoggingLevel);
			}
			SyncLogConfiguration syncLogConfiguration = AggregationConfiguration.CreateSyncLogConfiguration(server);
			if (this.syncLog == null)
			{
				this.syncLog = new SyncLog(syncLogConfiguration);
			}
			else
			{
				this.syncLog.ConfigureLog(syncLogConfiguration.Enabled, syncLogConfiguration.LogFilePath, syncLogConfiguration.AgeQuotaInHours, syncLogConfiguration.DirectorySizeQuota, syncLogConfiguration.PerFileSizeQuota, syncLogConfiguration.SyncLoggingLevel);
			}
			SyncLogSession syncLogSession = this.SyncLog.OpenSession();
			SyncPoisonHandler.TransportSyncEnabled = this.IsEnabled;
			CommonLoggingHelper.SyncLogSession = syncLogSession;
			SyncHealthLogConfiguration syncHealthLogConfiguration = SyncHealthLogConfiguration.CreateSyncHubHealthLogConfiguration(server);
			if (!AggregationConfiguration.IsDatacenterMode)
			{
				syncHealthLogConfiguration.SyncHealthLogEnabled = false;
			}
			if (this.syncHealthLog == null)
			{
				this.syncHealthLog = new SyncHealthLog(syncHealthLogConfiguration);
				return;
			}
			this.syncHealthLog.Configure(syncHealthLogConfiguration);
		}

		private static AggregationSubscriptionType CreateAggregationTypes(Server server)
		{
			AggregationSubscriptionType aggregationSubscriptionType = AggregationSubscriptionType.Unknown;
			if (server.TransportSyncPopEnabled)
			{
				aggregationSubscriptionType |= AggregationSubscriptionType.Pop;
			}
			if (server.WindowsLiveHotmailTransportSyncEnabled)
			{
				aggregationSubscriptionType |= AggregationSubscriptionType.DeltaSyncMail;
			}
			if (server.TransportSyncImapEnabled)
			{
				aggregationSubscriptionType |= AggregationSubscriptionType.IMAP;
			}
			if (server.TransportSyncFacebookEnabled)
			{
				aggregationSubscriptionType |= AggregationSubscriptionType.Facebook;
			}
			if (server.TransportSyncLinkedInEnabled)
			{
				aggregationSubscriptionType |= AggregationSubscriptionType.LinkedIn;
			}
			return aggregationSubscriptionType;
		}

		private static ProtocolLogConfiguration CreateHttpProtocolLogConfiguration(Server server)
		{
			ProtocolLogConfiguration protocolLogConfiguration = new ProtocolLogConfiguration("HTTP");
			protocolLogConfiguration.IsEnabled = (AggregationConfiguration.IsDatacenterMode && server.HttpProtocolLogEnabled);
			if (server.HttpProtocolLogFilePath != null && !string.IsNullOrEmpty(server.HttpProtocolLogFilePath.PathName))
			{
				protocolLogConfiguration.LogFilePath = server.HttpProtocolLogFilePath.PathName;
			}
			protocolLogConfiguration.AgeQuota = (long)server.HttpProtocolLogMaxAge.TotalHours;
			protocolLogConfiguration.DirectorySizeQuota = (long)server.HttpProtocolLogMaxDirectorySize.ToKB();
			protocolLogConfiguration.PerFileSizeQuota = (long)server.HttpProtocolLogMaxFileSize.ToKB();
			switch (server.HttpProtocolLogLoggingLevel)
			{
			case ProtocolLoggingLevel.None:
				protocolLogConfiguration.ProtocolLoggingLevel = ProtocolLoggingLevel.None;
				break;
			case ProtocolLoggingLevel.Verbose:
				protocolLogConfiguration.ProtocolLoggingLevel = ProtocolLoggingLevel.All;
				break;
			}
			return protocolLogConfiguration;
		}

		private static SyncLogConfiguration CreateSyncLogConfiguration(Server server)
		{
			SyncLogConfiguration syncLogConfiguration = new SyncLogConfiguration();
			syncLogConfiguration.Enabled = (AggregationConfiguration.IsDatacenterMode && server.TransportSyncLogEnabled);
			syncLogConfiguration.SyncLoggingLevel = server.TransportSyncLogLoggingLevel;
			syncLogConfiguration.AgeQuotaInHours = (long)server.TransportSyncLogMaxAge.TotalHours;
			syncLogConfiguration.DirectorySizeQuota = (long)server.TransportSyncLogMaxDirectorySize.ToKB();
			syncLogConfiguration.PerFileSizeQuota = (long)server.TransportSyncLogMaxFileSize.ToKB();
			if (server.TransportSyncLogFilePath != null && !string.IsNullOrEmpty(server.TransportSyncLogFilePath.PathName))
			{
				syncLogConfiguration.LogFilePath = server.TransportSyncLogFilePath.PathName;
			}
			else
			{
				syncLogConfiguration.LogFilePath = AggregationConfiguration.defaultRelativeSyncLogPath;
			}
			return syncLogConfiguration;
		}

		private static bool GetConfigBoolValue(string configName, bool defaultValue)
		{
			return AggregationConfiguration.GetConfigValue<bool>(configName, null, null, defaultValue, new AggregationConfiguration.TryParseValue<bool>(bool.TryParse));
		}

		private static int GetConfigIntValue(string configName, int minValue, int maxValue, int defaultValue)
		{
			return AggregationConfiguration.GetConfigValue<int>(configName, new int?(minValue), new int?(maxValue), defaultValue, new AggregationConfiguration.TryParseValue<int>(int.TryParse));
		}

		private static double GetConfigDoubleValue(string configName, double minValue, double maxValue, double defaultValue)
		{
			return AggregationConfiguration.GetConfigValue<double>(configName, new double?(minValue), new double?(maxValue), defaultValue, new AggregationConfiguration.TryParseValue<double>(double.TryParse));
		}

		private static TimeSpan GetConfigTimeSpanValue(string configName, TimeSpan minValue, TimeSpan maxValue, TimeSpan defaultValue)
		{
			return AggregationConfiguration.GetConfigValue<TimeSpan>(configName, new TimeSpan?(minValue), new TimeSpan?(maxValue), defaultValue, new AggregationConfiguration.TryParseValue<TimeSpan>(TimeSpan.TryParse));
		}

		private static ByteQuantifiedSize GetConfigByteQuantifiedSizeValue(string configName, ByteQuantifiedSize defaultValue)
		{
			return AggregationConfiguration.GetConfigValue<ByteQuantifiedSize>(configName, null, null, defaultValue, new AggregationConfiguration.TryParseValue<ByteQuantifiedSize>(ByteQuantifiedSize.TryParse));
		}

		private static void ReadEnumCollectionConfigValueAndUpdateHashSet<T>(string configName, HashSet<T> configValues, params T[] defaultValues)
		{
			configValues.Clear();
			string text;
			if (!AggregationConfiguration.TryGetStringConfigValue(configName, out text) || text == null)
			{
				if (defaultValues != null)
				{
					configValues.UnionWith(defaultValues);
					return;
				}
			}
			else
			{
				string[] array = text.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text2 in array)
				{
					T item = default(T);
					Exception ex = null;
					try
					{
						item = (T)((object)Enum.Parse(typeof(T), text2, true));
					}
					catch (ArgumentException ex2)
					{
						ex = ex2;
					}
					catch (OverflowException ex3)
					{
						ex = ex3;
					}
					if (ex != null)
					{
						ExTraceGlobals.CommonTracer.TraceWarning<string, string, Exception>(0L, "Ignoring value '{0}' for config {1} due to {2}", text2, configName, ex);
					}
					else
					{
						configValues.Add(item);
					}
				}
			}
		}

		private static bool TryGetStringConfigValue(string configName, out string configValue)
		{
			configValue = null;
			try
			{
				configValue = ConfigurationManager.AppSettings[configName];
				return true;
			}
			catch (ConfigurationErrorsException arg)
			{
				ExTraceGlobals.CommonTracer.TraceWarning<string, ConfigurationErrorsException>(0L, "failed to read config {0}: {1}", configName, arg);
			}
			return false;
		}

		private static T GetConfigValue<T>(string configName, T? minValue, T? maxValue, T defaultValue, AggregationConfiguration.TryParseValue<T> tryParseValue) where T : struct, IComparable<T>
		{
			SyncUtilities.ThrowIfArgumentNull("configName", configName);
			SyncUtilities.ThrowIfArgumentNull("tryParseValue", tryParseValue);
			string text;
			if (!AggregationConfiguration.TryGetStringConfigValue(configName, out text))
			{
				return defaultValue;
			}
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.CommonTracer.TraceDebug<string>(0L, "cannot apply null/empty config {0}", configName);
				return defaultValue;
			}
			T t = defaultValue;
			if (!tryParseValue(text, out t))
			{
				ExTraceGlobals.CommonTracer.TraceWarning<string, string>(0L, "cannot apply config {0} with invalid value: {1}", configName, text);
				return defaultValue;
			}
			if (minValue != null && t.CompareTo(minValue.Value) < 0)
			{
				ExTraceGlobals.CommonTracer.TraceWarning<string, T, T>(0L, "cannot apply config:{0}, value:{1} is less than minValue:{2}", configName, t, minValue.Value);
				return defaultValue;
			}
			if (maxValue != null && t.CompareTo(maxValue.Value) > 0)
			{
				ExTraceGlobals.CommonTracer.TraceWarning<string, T, T>(0L, "cannot apply config:{0}, value:{1} is greater than maxValue:{2}", configName, t, maxValue.Value);
				return defaultValue;
			}
			return t;
		}

		private static void LoadConfigSlidingCounterValues(string windowSizeConfigName, TimeSpan windowSizeDefaultValue, out TimeSpan windowSizeValue, string bucketLengthConfigName, TimeSpan bucketLengthDefaultValue, out TimeSpan bucketLengthValue)
		{
			windowSizeValue = AggregationConfiguration.GetConfigTimeSpanValue(windowSizeConfigName, TimeSpan.Zero, TimeSpan.MaxValue, windowSizeDefaultValue);
			bucketLengthValue = AggregationConfiguration.GetConfigTimeSpanValue(bucketLengthConfigName, TimeSpan.Zero, TimeSpan.MaxValue, bucketLengthDefaultValue);
			try
			{
				SlidingWindow.ValidateSlidingWindowAndBucketLength(windowSizeValue, bucketLengthValue);
			}
			catch (ExAssertException arg)
			{
				ExTraceGlobals.CommonTracer.TraceError<string, string, ExAssertException>(0L, "cannot apply config:{0} and {1}, applying the default ones. Failure Reason:{2}", windowSizeConfigName, bucketLengthConfigName, arg);
				windowSizeValue = windowSizeDefaultValue;
				bucketLengthValue = bucketLengthDefaultValue;
			}
		}

		private void ReadAppConfig()
		{
			string[] array = new string[]
			{
				"DelayedSubscriptionThreshold",
				"DisableSubscriptionThreshold",
				"ExtendedDisableSubscriptionThreshold",
				"SyncPoisonContextExpiry",
				"HubInactivityThreshold",
				"DisabledSubscriptionAgents",
				"InitialRetryInMilliseconds",
				"RetryBackoffFactor",
				"MaxItemsForDBInManualConcurrencyMode",
				"SubscriptionNotificationEnabled",
				"OutageDetectionThreshold",
				"DelayTresholdForAcceptingNewWorkItems",
				"TerminateSlowSyncEnabled",
				"SyncDurationThreshold",
				"RemoteRoundtripTimeThreshold",
				"MaxTransientErrorsPerItem",
				"HealthCheckRetryInterval",
				"MaxMappingTableSizeInMemory",
				"MaxXsoFolderSyncStateCacheSizeInMemory"
			};
			foreach (string text in array)
			{
				string text2 = null;
				try
				{
					text2 = ConfigurationManager.AppSettings[text];
				}
				catch (ConfigurationErrorsException arg)
				{
					ExTraceGlobals.CommonTracer.TraceWarning<string, ConfigurationErrorsException>(0L, "failed to read config {0}: {1}", text, arg);
				}
				if (string.IsNullOrEmpty(text2))
				{
					ExTraceGlobals.CommonTracer.TraceDebug<string>(0L, "cannot apply null/empty config {0}", text);
				}
				else
				{
					bool flag = true;
					string key;
					switch (key = text)
					{
					case "SyncPoisonContextExpiry":
					{
						TimeSpan poisonContextExpiry;
						if (TimeSpan.TryParse(text2, out poisonContextExpiry))
						{
							SyncPoisonHandler.PoisonContextExpiry = poisonContextExpiry;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "TerminateSlowSyncEnabled":
					{
						bool flag2;
						if (bool.TryParse(text2, out flag2))
						{
							this.terminateSlowSyncEnabled = flag2;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "SyncDurationThreshold":
					{
						TimeSpan poisonContextExpiry;
						if (TimeSpan.TryParse(text2, out poisonContextExpiry))
						{
							this.syncDurationThreshold = poisonContextExpiry;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "RemoteRoundtripTimeThreshold":
					{
						TimeSpan poisonContextExpiry;
						if (TimeSpan.TryParse(text2, out poisonContextExpiry))
						{
							this.remoteRoundtripTimeThreshold = poisonContextExpiry;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "DelayedSubscriptionThreshold":
					{
						TimeSpan poisonContextExpiry;
						if (TimeSpan.TryParse(text2, out poisonContextExpiry))
						{
							this.delayedSubscriptionThreshold = poisonContextExpiry;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "DisableSubscriptionThreshold":
					{
						TimeSpan poisonContextExpiry;
						if (TimeSpan.TryParse(text2, out poisonContextExpiry))
						{
							this.disableSubscriptionThreshold = poisonContextExpiry;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "ExtendedDisableSubscriptionThreshold":
					{
						TimeSpan poisonContextExpiry;
						if (TimeSpan.TryParse(text2, out poisonContextExpiry))
						{
							this.extendedDisableSubscriptionThreshold = poisonContextExpiry;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "HubInactivityThreshold":
					{
						TimeSpan poisonContextExpiry;
						if (TimeSpan.TryParse(text2, out poisonContextExpiry))
						{
							this.hubInactivityThreshold = poisonContextExpiry;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "HealthCheckRetryInterval":
					{
						TimeSpan poisonContextExpiry;
						if (TimeSpan.TryParse(text2, out poisonContextExpiry))
						{
							this.healthCheckRetryInterval = poisonContextExpiry;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "DisabledSubscriptionAgents":
					{
						string[] array3 = text2.Split(new char[]
						{
							','
						});
						foreach (string text3 in array3)
						{
							string item = text3.Trim();
							if (!this.disabledSubscriptionAgents.Contains(item))
							{
								this.disabledSubscriptionAgents.Add(item);
							}
						}
						break;
					}
					case "InitialRetryInMilliseconds":
					{
						int num2;
						if (int.TryParse(text2, out num2))
						{
							this.initialRetryInMilliseconds = num2;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "RetryBackoffFactor":
					{
						int num2;
						if (int.TryParse(text2, out num2))
						{
							this.retryBackoffFactor = num2;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "DelayTresholdForAcceptingNewWorkItems":
					{
						int num2;
						if (int.TryParse(text2, out num2))
						{
							this.delayTresholdForAcceptingNewWorkItems = num2;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "MaxItemsForDBInManualConcurrencyMode":
					{
						int num2;
						if (int.TryParse(text2, out num2))
						{
							this.maxItemsForDBInManualConcurrencyMode = num2;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "SubscriptionNotificationEnabled":
					{
						bool flag2;
						if (bool.TryParse(text2, out flag2))
						{
							this.subscriptionNotificationEnabled = flag2;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "MaxTransientErrorsPerItem":
					{
						int num2;
						if (int.TryParse(text2, out num2))
						{
							this.maxTransientErrorsPerItem = num2;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "OutageDetectionThreshold":
					{
						TimeSpan poisonContextExpiry;
						if (TimeSpan.TryParse(text2, out poisonContextExpiry))
						{
							this.outageDetectionThreshold = poisonContextExpiry;
						}
						else
						{
							flag = false;
						}
						break;
					}
					case "MaxMappingTableSizeInMemory":
					{
						ByteQuantifiedSize byteQuantifiedSize;
						if (ByteQuantifiedSize.TryParse(text2, out byteQuantifiedSize))
						{
							this.maxMappingTableSizeInMemory = byteQuantifiedSize;
						}
						else
						{
							flag = false;
						}
						break;
					}
					}
					if (!flag)
					{
						ExTraceGlobals.CommonTracer.TraceWarning<string, string>(0L, "cannot apply config {0} with invalid value: {1}", text, text2);
					}
				}
			}
		}

		internal static readonly int DefaultMaxDownloadItemsInFirstDeltaSyncConnection = 1;

		private static readonly string defaultRelativeSyncLogPath = "TransportRoles\\Logs\\SyncLog\\Hub";

		private static readonly Trace diag = ExTraceGlobals.AggregationConfigurationTracer;

		protected static object instanceInitializationLock = new object();

		protected static AggregationConfiguration instance;

		private static bool datacenterMode;

		private static bool checkedDatacenterMode;

		protected static int processorCount = Environment.ProcessorCount;

		private ITransportConfiguration transportConfiguration;

		private int initialRetryInMilliseconds;

		private int retryBackoffFactor;

		private int maxItemsForDBInUnknownHealthState;

		private int maxItemsForMailboxServerInUnknownHealthState;

		private int maxItemsForTransportServerInUnknownHealthState;

		private int delayTresholdForAcceptingNewWorkItems;

		private int maxItemsForDBInManualConcurrencyMode;

		private AggregationSubscriptionType enabledAggregationTypes;

		private ProtocolLog httpProtocolLog;

		private SyncLog syncLog;

		protected SyncHealthLog syncHealthLog;

		private IWebProxy contentAggregationProxyServer;

		private bool terminateSlowSyncEnabled;

		private TimeSpan syncDurationThreshold;

		private TimeSpan remoteRoundtripTimeThreshold;

		private TimeSpan delayedSubscriptionThreshold;

		private TimeSpan disableSubscriptionThreshold;

		private TimeSpan extendedDisableSubscriptionThreshold;

		private TimeSpan outageDetectionThreshold;

		private TimeSpan hubInactivityThreshold;

		private TimeSpan healthCheckRetryInterval;

		private HashSet<string> disabledSubscriptionAgents;

		private bool subscriptionNotificationEnabled;

		private int maxTransientErrorsPerItem;

		private TimeSpan remoteServerLatencySlidingCounterWindowSize;

		private TimeSpan remoteServerLatencySlidingCounterBucketLength;

		private TimeSpan remoteServerLatencyThreshold;

		private int remoteServerBackoffCountLimit;

		private TimeSpan remoteServerBackoffTimeSpan;

		private TimeSpan remoteServerHealthDataExpiryPeriod;

		private TimeSpan remoteServerHealthDataExpiryAndPersistanceFrequency;

		private double remoteServerAllowedCapacityUsagePercentage;

		private bool remoteServerHealthManagementEnabled;

		private bool remoteServerPoisonMarkingEnabled;

		private bool reportDataLoggingDisabled;

		private bool cloudStatisticsCollectionDisabled;

		private ByteQuantifiedSize maxMappingTableSizeInMemory;

		private ByteQuantifiedSize minFreeSpaceRequired;

		private ByteQuantifiedSize maxDownloadSizePerConnectionForAggregation;

		private ByteQuantifiedSize maxDownloadSizePerConnectionForPeopleConnection;

		private bool alwaysLoadSubscription;

		private int maxDownloadItemsPerConnectionForAggregation;

		private int maxDownloadItemsPerConnectionForPeopleConnection;

		private int maxDownloadItemsInFirstDeltaSyncConnection;

		private HashSet<AggregationType> aggregationTypesToBeSyncedInRecoveryMode = new HashSet<AggregationType>();

		private HashSet<AggregationSubscriptionType> subscriptionTypesToBeSyncedInRecoveryMode = new HashSet<AggregationSubscriptionType>();

		private HashSet<SyncPhase> syncPhasesToBeSyncedInRecoveryMode = new HashSet<SyncPhase>();

		private HashSet<SyncResourceMonitorType> syncResourceMonitorsDisabled = new HashSet<SyncResourceMonitorType>();

		private int suggestedConcurrencyOverride;

		private int maxPendingMessagesInTransportQueueForTheServer;

		private int maxPendingMessagesInTransportQueuePerUser;

		internal delegate bool TryParseValue<T>(string stringValue, out T configValue);
	}
}
