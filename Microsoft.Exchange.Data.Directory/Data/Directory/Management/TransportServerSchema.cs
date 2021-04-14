using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class TransportServerSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ServerSchema>();
		}

		public static readonly ADPropertyDefinition DelayNotificationTimeout = ServerSchema.DelayNotificationTimeout;

		public static readonly ADPropertyDefinition MessageExpirationTimeout = ServerSchema.MessageExpirationTimeout;

		public static readonly ADPropertyDefinition QueueMaxIdleTime = ServerSchema.QueueMaxIdleTime;

		public static readonly ADPropertyDefinition MessageRetryInterval = ServerSchema.MessageRetryInterval;

		public static readonly ADPropertyDefinition TransientFailureRetryInterval = ServerSchema.TransientFailureRetryInterval;

		public static readonly ADPropertyDefinition TransientFailureRetryCount = ServerSchema.TransientFailureRetryCount;

		public static readonly ADPropertyDefinition MaxOutboundConnections = ServerSchema.MaxOutboundConnections;

		public static readonly ADPropertyDefinition MaxPerDomainOutboundConnections = ServerSchema.MaxPerDomainOutboundConnections;

		public static readonly ADPropertyDefinition MaxConnectionRatePerMinute = ServerSchema.MaxConnectionRatePerMinute;

		public static readonly ADPropertyDefinition ReceiveProtocolLogPath = ServerSchema.ReceiveProtocolLogPath;

		public static readonly ADPropertyDefinition SendProtocolLogPath = ServerSchema.SendProtocolLogPath;

		public static readonly ADPropertyDefinition OutboundConnectionFailureRetryInterval = ServerSchema.OutboundConnectionFailureRetryInterval;

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxAge = ServerSchema.ReceiveProtocolLogMaxAge;

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxDirectorySize = ServerSchema.ReceiveProtocolLogMaxDirectorySize;

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxFileSize = ServerSchema.ReceiveProtocolLogMaxFileSize;

		public static readonly ADPropertyDefinition SendProtocolLogMaxAge = ServerSchema.SendProtocolLogMaxAge;

		public static readonly ADPropertyDefinition SendProtocolLogMaxDirectorySize = ServerSchema.SendProtocolLogMaxDirectorySize;

		public static readonly ADPropertyDefinition SendProtocolLogMaxFileSize = ServerSchema.SendProtocolLogMaxFileSize;

		public static readonly ADPropertyDefinition InternalDNSAdapterDisabled = ServerSchema.InternalDNSAdapterDisabled;

		public static readonly ADPropertyDefinition InternalDNSAdapterGuid = ServerSchema.InternalDNSAdapterGuid;

		public static readonly ADPropertyDefinition InternalDNSServers = ServerSchema.InternalDNSServers;

		public static readonly ADPropertyDefinition InternalDNSProtocolOption = ServerSchema.InternalDNSProtocolOption;

		public static readonly ADPropertyDefinition ExternalDNSAdapterDisabled = ServerSchema.ExternalDNSAdapterDisabled;

		public static readonly ADPropertyDefinition ExternalDNSAdapterGuid = ServerSchema.ExternalDNSAdapterGuid;

		public static readonly ADPropertyDefinition ExternalDNSServers = ServerSchema.ExternalDNSServers;

		public static readonly ADPropertyDefinition ExternalDNSProtocolOption = ServerSchema.ExternalDNSProtocolOption;

		public static readonly ADPropertyDefinition ExternalIPAddress = ServerSchema.ExternalIPAddress;

		public static readonly ADPropertyDefinition MaxConcurrentMailboxDeliveries = ServerSchema.MaxConcurrentMailboxDeliveries;

		public static readonly ADPropertyDefinition PoisonThreshold = ServerSchema.PoisonThreshold;

		public static readonly ADPropertyDefinition MessageTrackingLogPath = ServerSchema.MessageTrackingLogPath;

		public static readonly ADPropertyDefinition MessageTrackingLogMaxAge = ServerSchema.MessageTrackingLogMaxAge;

		public static readonly ADPropertyDefinition MessageTrackingLogMaxDirectorySize = ServerSchema.MessageTrackingLogMaxDirectorySize;

		public static readonly ADPropertyDefinition MessageTrackingLogMaxFileSize = ServerSchema.MessageTrackingLogMaxFileSize;

		public static readonly ADPropertyDefinition IrmLogPath = ServerSchema.IrmLogPath;

		public static readonly ADPropertyDefinition IrmLogMaxAge = ServerSchema.IrmLogMaxAge;

		public static readonly ADPropertyDefinition IrmLogMaxDirectorySize = ServerSchema.IrmLogMaxDirectorySize;

		public static readonly ADPropertyDefinition IrmLogMaxFileSize = ServerSchema.IrmLogMaxFileSize;

		public static readonly ADPropertyDefinition ActiveUserStatisticsLogPath = ServerSchema.ActiveUserStatisticsLogPath;

		public static readonly ADPropertyDefinition ActiveUserStatisticsLogMaxAge = ServerSchema.ActiveUserStatisticsLogMaxAge;

		public static readonly ADPropertyDefinition ActiveUserStatisticsLogMaxDirectorySize = ServerSchema.ActiveUserStatisticsLogMaxDirectorySize;

		public static readonly ADPropertyDefinition ActiveUserStatisticsLogMaxFileSize = ServerSchema.ActiveUserStatisticsLogMaxFileSize;

		public static readonly ADPropertyDefinition ServerStatisticsLogPath = ServerSchema.ServerStatisticsLogPath;

		public static readonly ADPropertyDefinition ServerStatisticsLogMaxAge = ServerSchema.ServerStatisticsLogMaxAge;

		public static readonly ADPropertyDefinition ServerStatisticsLogMaxDirectorySize = ServerSchema.ServerStatisticsLogMaxDirectorySize;

		public static readonly ADPropertyDefinition ServerStatisticsLogMaxFileSize = ServerSchema.ServerStatisticsLogMaxFileSize;

		public static readonly ADPropertyDefinition MessageTrackingLogSubjectLoggingEnabled = ServerSchema.MessageTrackingLogSubjectLoggingEnabled;

		public static readonly ADPropertyDefinition PipelineTracingEnabled = ServerSchema.PipelineTracingEnabled;

		public static readonly ADPropertyDefinition ContentConversionTracingEnabled = ServerSchema.ContentConversionTracingEnabled;

		public static readonly ADPropertyDefinition PipelineTracingPath = ServerSchema.PipelineTracingPath;

		public static readonly ADPropertyDefinition PipelineTracingSenderAddress = ServerSchema.PipelineTracingSenderAddress;

		public static readonly ADPropertyDefinition ConnectivityLogEnabled = ServerSchema.ConnectivityLogEnabled;

		public static readonly ADPropertyDefinition ConnectivityLogPath = ServerSchema.ConnectivityLogPath;

		public static readonly ADPropertyDefinition ConnectivityLogMaxAge = ServerSchema.ConnectivityLogMaxAge;

		public static readonly ADPropertyDefinition ConnectivityLogMaxDirectorySize = ServerSchema.ConnectivityLogMaxDirectorySize;

		public static readonly ADPropertyDefinition ConnectivityLogMaxFileSize = ServerSchema.ConnectivityLogMaxFileSize;

		public static readonly ADPropertyDefinition PickupDirectoryPath = ServerSchema.PickupDirectoryPath;

		public static readonly ADPropertyDefinition ReplayDirectoryPath = ServerSchema.ReplayDirectoryPath;

		public static readonly ADPropertyDefinition PickupDirectoryMaxMessagesPerMinute = ServerSchema.PickupDirectoryMaxMessagesPerMinute;

		public static readonly ADPropertyDefinition PickupDirectoryMaxHeaderSize = ServerSchema.PickupDirectoryMaxHeaderSize;

		public static readonly ADPropertyDefinition PickupDirectoryMaxRecipientsPerMessage = ServerSchema.PickupDirectoryMaxRecipientsPerMessage;

		public static readonly ADPropertyDefinition RoutingTableLogPath = ServerSchema.RoutingTableLogPath;

		public static readonly ADPropertyDefinition RoutingTableLogMaxAge = ServerSchema.RoutingTableLogMaxAge;

		public static readonly ADPropertyDefinition RoutingTableLogMaxDirectorySize = ServerSchema.RoutingTableLogMaxDirectorySize;

		public static readonly ADPropertyDefinition IntraOrgConnectorProtocolLoggingLevel = ServerSchema.IntraOrgConnectorProtocolLoggingLevel;

		public static readonly ADPropertyDefinition MessageTrackingLogEnabled = ServerSchema.MessageTrackingLogEnabled;

		public static readonly ADPropertyDefinition IrmLogEnabled = ServerSchema.IrmLogEnabled;

		public static readonly ADPropertyDefinition PoisonMessageDetectionEnabled = ServerSchema.PoisonMessageDetectionEnabled;

		public static readonly ADPropertyDefinition AntispamAgentsEnabled = ServerSchema.AntispamAgentsEnabled;

		public static readonly ADPropertyDefinition RootDropDirectoryPath = ServerSchema.RootDropDirectoryPath;

		public static readonly ADPropertyDefinition RecipientValidationCacheEnabled = ServerSchema.RecipientValidationCacheEnabled;

		public static readonly ADPropertyDefinition AntispamUpdatesEnabled = ServerSchema.AntispamUpdatesEnabled;

		public static readonly ADPropertyDefinition InternalTransportCertificateThumbprint = ServerSchema.InternalTransportCertificateThumbprint;

		public static readonly ADPropertyDefinition TransportSyncEnabled = ServerSchema.TransportSyncEnabled;

		public static readonly ADPropertyDefinition TransportSyncPopEnabled = ServerSchema.TransportSyncPopEnabled;

		public static readonly ADPropertyDefinition WindowsLiveHotmailTransportSyncEnabled = ServerSchema.WindowsLiveHotmailTransportSyncEnabled;

		public static readonly ADPropertyDefinition TransportSyncExchangeEnabled = ServerSchema.TransportSyncExchangeEnabled;

		public static readonly ADPropertyDefinition TransportSyncImapEnabled = ServerSchema.TransportSyncImapEnabled;

		public static readonly ADPropertyDefinition MaxNumberOfTransportSyncAttempts = ServerSchema.MaxNumberOfTransportSyncAttempts;

		public static readonly ADPropertyDefinition MaxActiveTransportSyncJobsPerProcessor = ServerSchema.MaxActiveTransportSyncJobsPerProcessor;

		public static readonly ADPropertyDefinition HttpTransportSyncProxyServer = ServerSchema.HttpTransportSyncProxyServer;

		public static readonly ADPropertyDefinition HttpProtocolLogEnabled = ServerSchema.HttpProtocolLogEnabled;

		public static readonly ADPropertyDefinition HttpProtocolLogFilePath = ServerSchema.HttpProtocolLogFilePath;

		public static readonly ADPropertyDefinition HttpProtocolLogMaxAge = ServerSchema.HttpProtocolLogMaxAge;

		public static readonly ADPropertyDefinition HttpProtocolLogMaxDirectorySize = ServerSchema.HttpProtocolLogMaxDirectorySize;

		public static readonly ADPropertyDefinition HttpProtocolLogMaxFileSize = ServerSchema.HttpProtocolLogMaxFileSize;

		public static readonly ADPropertyDefinition HttpProtocolLogLoggingLevel = ServerSchema.HttpProtocolLogLoggingLevel;

		public static readonly ADPropertyDefinition TransportSyncLogEnabled = ServerSchema.TransportSyncLogEnabled;

		public static readonly ADPropertyDefinition TransportSyncLogFilePath = ServerSchema.TransportSyncLogFilePath;

		public static readonly ADPropertyDefinition TransportSyncLogLoggingLevel = ServerSchema.TransportSyncLogLoggingLevel;

		public static readonly ADPropertyDefinition TransportSyncLogMaxAge = ServerSchema.TransportSyncLogMaxAge;

		public static readonly ADPropertyDefinition TransportSyncLogMaxDirectorySize = ServerSchema.TransportSyncLogMaxDirectorySize;

		public static readonly ADPropertyDefinition TransportSyncLogMaxFileSize = ServerSchema.TransportSyncLogMaxFileSize;

		public static readonly ADPropertyDefinition TransportSyncAccountsPoisonDetectionEnabled = ServerSchema.TransportSyncAccountsPoisonDetectionEnabled;

		public static readonly ADPropertyDefinition TransportSyncAccountsPoisonAccountThreshold = ServerSchema.TransportSyncAccountsPoisonAccountThreshold;

		public static readonly ADPropertyDefinition TransportSyncAccountsPoisonItemThreshold = ServerSchema.TransportSyncAccountsPoisonItemThreshold;

		public static readonly ADPropertyDefinition TransportSyncRemoteConnectionTimeout = ServerSchema.TransportSyncRemoteConnectionTimeout;

		public static readonly ADPropertyDefinition TransportSyncMaxDownloadSizePerItem = ServerSchema.TransportSyncMaxDownloadSizePerItem;

		public static readonly ADPropertyDefinition TransportSyncMaxDownloadSizePerConnection = ServerSchema.TransportSyncMaxDownloadSizePerConnection;

		public static readonly ADPropertyDefinition TransportSyncMaxDownloadItemsPerConnection = ServerSchema.TransportSyncMaxDownloadItemsPerConnection;

		public static readonly ADPropertyDefinition DeltaSyncClientCertificateThumbprint = ServerSchema.DeltaSyncClientCertificateThumbprint;

		public static readonly ADPropertyDefinition IntraOrgConnectorSmtpMaxMessagesPerConnection = ServerSchema.IntraOrgConnectorSmtpMaxMessagesPerConnection;

		public static readonly ADPropertyDefinition TransportSyncLinkedInEnabled = ServerSchema.TransportSyncLinkedInEnabled;

		public static readonly ADPropertyDefinition TransportSyncFacebookEnabled = ServerSchema.TransportSyncFacebookEnabled;
	}
}
