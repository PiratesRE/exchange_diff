using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class FrontendTransportServerSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<FrontendTransportServerADSchema>();
		}

		public static readonly ADPropertyDefinition AdminDisplayVersion = FrontendTransportServerADSchema.AdminDisplayVersion;

		public static readonly ADPropertyDefinition Edition = FrontendTransportServerADSchema.Edition;

		public static readonly ADPropertyDefinition ExchangeLegacyDN = FrontendTransportServerADSchema.ExchangeLegacyDN;

		public static readonly ADPropertyDefinition NetworkAddress = FrontendTransportServerADSchema.NetworkAddress;

		public static readonly ADPropertyDefinition VersionNumber = FrontendTransportServerADSchema.VersionNumber;

		public static readonly ADPropertyDefinition IsFrontendTransportServer = FrontendTransportServerADSchema.IsFrontendTransportServer;

		public static readonly ADPropertyDefinition IsProvisionedServer = FrontendTransportServerADSchema.IsProvisionedServer;

		public static readonly ADPropertyDefinition IntraOrgConnectorSmtpMaxMessagesPerConnection = FrontendTransportServerADSchema.IntraOrgConnectorSmtpMaxMessagesPerConnection;

		public static readonly ADPropertyDefinition MaxPerDomainOutboundConnections = FrontendTransportServerADSchema.MaxPerDomainOutboundConnections;

		public static readonly ADPropertyDefinition MaxOutboundConnections = FrontendTransportServerADSchema.MaxOutboundConnections;

		public static readonly ADPropertyDefinition TransientFailureRetryInterval = FrontendTransportServerADSchema.TransientFailureRetryInterval;

		public static readonly ADPropertyDefinition TransientFailureRetryCount = FrontendTransportServerADSchema.TransientFailureRetryCount;

		public static readonly ADPropertyDefinition ReceiveProtocolLogPath = FrontendTransportServerADSchema.ReceiveProtocolLogPath;

		public static readonly ADPropertyDefinition SendProtocolLogPath = FrontendTransportServerADSchema.SendProtocolLogPath;

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxAge = FrontendTransportServerADSchema.ReceiveProtocolLogMaxAge;

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxDirectorySize = FrontendTransportServerADSchema.ReceiveProtocolLogMaxDirectorySize;

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxFileSize = FrontendTransportServerADSchema.ReceiveProtocolLogMaxFileSize;

		public static readonly ADPropertyDefinition SendProtocolLogMaxAge = FrontendTransportServerADSchema.SendProtocolLogMaxAge;

		public static readonly ADPropertyDefinition SendProtocolLogMaxDirectorySize = FrontendTransportServerADSchema.SendProtocolLogMaxDirectorySize;

		public static readonly ADPropertyDefinition SendProtocolLogMaxFileSize = FrontendTransportServerADSchema.SendProtocolLogMaxFileSize;

		public static readonly ADPropertyDefinition InternalDNSAdapterDisabled = FrontendTransportServerADSchema.InternalDNSAdapterDisabled;

		public static readonly ADPropertyDefinition InternalDNSAdapterGuid = FrontendTransportServerADSchema.InternalDNSAdapterGuid;

		public static readonly ADPropertyDefinition InternalDNSServers = FrontendTransportServerADSchema.InternalDNSServers;

		public static readonly ADPropertyDefinition InternalDNSProtocolOption = FrontendTransportServerADSchema.InternalDNSProtocolOption;

		public static readonly ADPropertyDefinition IntraOrgConnectorProtocolLoggingLevel = FrontendTransportServerADSchema.IntraOrgConnectorProtocolLoggingLevel;

		public static readonly ADPropertyDefinition ExternalDNSAdapterDisabled = FrontendTransportServerADSchema.ExternalDNSAdapterDisabled;

		public static readonly ADPropertyDefinition ExternalDNSAdapterGuid = FrontendTransportServerADSchema.ExternalDNSAdapterGuid;

		public static readonly ADPropertyDefinition ExternalDNSServers = FrontendTransportServerADSchema.ExternalDNSServers;

		public static readonly ADPropertyDefinition ExternalDNSProtocolOption = FrontendTransportServerADSchema.ExternalDNSProtocolOption;

		public static readonly ADPropertyDefinition ExternalIPAddress = FrontendTransportServerADSchema.ExternalIPAddress;

		public static readonly ADPropertyDefinition ConnectivityLogEnabled = FrontendTransportServerADSchema.ConnectivityLogEnabled;

		public static readonly ADPropertyDefinition ConnectivityLogPath = FrontendTransportServerADSchema.ConnectivityLogPath;

		public static readonly ADPropertyDefinition ConnectivityLogMaxAge = FrontendTransportServerADSchema.ConnectivityLogMaxAge;

		public static readonly ADPropertyDefinition ConnectivityLogMaxDirectorySize = FrontendTransportServerADSchema.ConnectivityLogMaxDirectorySize;

		public static readonly ADPropertyDefinition ConnectivityLogMaxFileSize = FrontendTransportServerADSchema.ConnectivityLogMaxFileSize;

		public static readonly ADPropertyDefinition AntispamAgentsEnabled = FrontendTransportServerADSchema.AntispamAgentsEnabled;

		public static readonly ADPropertyDefinition CurrentServerRole = FrontendTransportServerADSchema.CurrentServerRole;

		public static readonly ADPropertyDefinition MaxConnectionRatePerMinute = FrontendTransportServerADSchema.MaxConnectionRatePerMinute;

		public static readonly ADPropertyDefinition AgentLogEnabled = FrontendTransportServerADSchema.AgentLogEnabled;

		public static readonly ADPropertyDefinition AgentLogMaxAge = FrontendTransportServerADSchema.AgentLogMaxAge;

		public static readonly ADPropertyDefinition AgentLogMaxDirectorySize = FrontendTransportServerADSchema.AgentLogMaxDirectorySize;

		public static readonly ADPropertyDefinition AgentLogMaxFileSize = FrontendTransportServerADSchema.AgentLogMaxFileSize;

		public static readonly ADPropertyDefinition AgentLogPath = FrontendTransportServerADSchema.AgentLogPath;

		public static readonly ADPropertyDefinition DnsLogEnabled = FrontendTransportServerADSchema.DnsLogEnabled;

		public static readonly ADPropertyDefinition DnsLogMaxAge = FrontendTransportServerADSchema.DnsLogMaxAge;

		public static readonly ADPropertyDefinition DnsLogMaxDirectorySize = FrontendTransportServerADSchema.DnsLogMaxDirectorySize;

		public static readonly ADPropertyDefinition DnsLogMaxFileSize = FrontendTransportServerADSchema.DnsLogMaxFileSize;

		public static readonly ADPropertyDefinition DnsLogPath = FrontendTransportServerADSchema.DnsLogPath;

		public static readonly ADPropertyDefinition ResourceLogEnabled = FrontendTransportServerADSchema.ResourceLogEnabled;

		public static readonly ADPropertyDefinition ResourceLogMaxAge = FrontendTransportServerADSchema.ResourceLogMaxAge;

		public static readonly ADPropertyDefinition ResourceLogMaxDirectorySize = FrontendTransportServerADSchema.ResourceLogMaxDirectorySize;

		public static readonly ADPropertyDefinition ResourceLogMaxFileSize = FrontendTransportServerADSchema.ResourceLogMaxFileSize;

		public static readonly ADPropertyDefinition ResourceLogPath = FrontendTransportServerADSchema.ResourceLogPath;

		public static readonly ADPropertyDefinition AttributionLogEnabled = FrontendTransportServerADSchema.AttributionLogEnabled;

		public static readonly ADPropertyDefinition AttributionLogMaxAge = FrontendTransportServerADSchema.AttributionLogMaxAge;

		public static readonly ADPropertyDefinition AttributionLogMaxDirectorySize = FrontendTransportServerADSchema.AttributionLogMaxDirectorySize;

		public static readonly ADPropertyDefinition AttributionLogMaxFileSize = FrontendTransportServerADSchema.AttributionLogMaxFileSize;

		public static readonly ADPropertyDefinition AttributionLogPath = FrontendTransportServerADSchema.AttributionLogPath;

		public static readonly ADPropertyDefinition MaxReceiveTlsRatePerMinute = FrontendTransportServerADSchema.MaxReceiveTlsRatePerMinute;
	}
}
