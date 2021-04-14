using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class MailboxTransportServerSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<MailboxTransportServerADSchema>();
		}

		public static readonly ADPropertyDefinition AdminDisplayVersion = MailboxTransportServerADSchema.AdminDisplayVersion;

		public static readonly ADPropertyDefinition ConnectivityLogEnabled = MailboxTransportServerADSchema.ConnectivityLogEnabled;

		public static readonly ADPropertyDefinition ConnectivityLogPath = MailboxTransportServerADSchema.ConnectivityLogPath;

		public static readonly ADPropertyDefinition ConnectivityLogMaxAge = MailboxTransportServerADSchema.ConnectivityLogMaxAge;

		public static readonly ADPropertyDefinition ConnectivityLogMaxDirectorySize = MailboxTransportServerADSchema.ConnectivityLogMaxDirectorySize;

		public static readonly ADPropertyDefinition ConnectivityLogMaxFileSize = MailboxTransportServerADSchema.ConnectivityLogMaxFileSize;

		public static readonly ADPropertyDefinition ContentConversionTracingEnabled = MailboxTransportServerADSchema.ContentConversionTracingEnabled;

		public static readonly ADPropertyDefinition CurrentServerRole = MailboxTransportServerADSchema.CurrentServerRole;

		public static readonly ADPropertyDefinition Edition = MailboxTransportServerADSchema.Edition;

		public static readonly ADPropertyDefinition ExchangeLegacyDN = MailboxTransportServerADSchema.ExchangeLegacyDN;

		public static readonly ADPropertyDefinition IsMailboxServer = MailboxTransportServerADSchema.IsMailboxServer;

		public static readonly ADPropertyDefinition IsProvisionedServer = MailboxTransportServerADSchema.IsProvisionedServer;

		public static readonly ADPropertyDefinition InMemoryReceiveConnectorProtocolLoggingLevel = MailboxTransportServerADSchema.InMemoryReceiveConnectorProtocolLoggingLevel;

		public static readonly ADPropertyDefinition InMemoryReceiveConnectorSmtpUtf8Enabled = MailboxTransportServerADSchema.InMemoryReceiveConnectorSmtpUtf8Enabled;

		public static readonly ADPropertyDefinition MailboxDeliveryAgentLogEnabled = MailboxTransportServerADSchema.MailboxDeliveryAgentLogEnabled;

		public static readonly ADPropertyDefinition MailboxDeliveryAgentLogMaxAge = MailboxTransportServerADSchema.MailboxDeliveryAgentLogMaxAge;

		public static readonly ADPropertyDefinition MailboxDeliveryAgentLogMaxDirectorySize = MailboxTransportServerADSchema.MailboxDeliveryAgentLogMaxDirectorySize;

		public static readonly ADPropertyDefinition MailboxDeliveryAgentLogMaxFileSize = MailboxTransportServerADSchema.MailboxDeliveryAgentLogMaxFileSize;

		public static readonly ADPropertyDefinition MailboxDeliveryAgentLogPath = MailboxTransportServerADSchema.MailboxDeliveryAgentLogPath;

		public static readonly ADPropertyDefinition MailboxDeliveryThrottlingLogEnabled = MailboxTransportServerADSchema.MailboxDeliveryThrottlingLogEnabled;

		public static readonly ADPropertyDefinition MailboxDeliveryThrottlingLogMaxAge = MailboxTransportServerADSchema.MailboxDeliveryThrottlingLogMaxAge;

		public static readonly ADPropertyDefinition MailboxDeliveryThrottlingLogMaxDirectorySize = MailboxTransportServerADSchema.MailboxDeliveryThrottlingLogMaxDirectorySize;

		public static readonly ADPropertyDefinition MailboxDeliveryThrottlingLogMaxFileSize = MailboxTransportServerADSchema.MailboxDeliveryThrottlingLogMaxFileSize;

		public static readonly ADPropertyDefinition MailboxDeliveryThrottlingLogPath = MailboxTransportServerADSchema.MailboxDeliveryThrottlingLogPath;

		public static readonly ADPropertyDefinition MailboxDeliveryConnectorProtocolLoggingLevel = MailboxTransportServerADSchema.InMemoryReceiveConnectorProtocolLoggingLevel;

		public static readonly ADPropertyDefinition MailboxDeliveryConnectorSmtpUtf8Enabled = MailboxTransportServerADSchema.InMemoryReceiveConnectorSmtpUtf8Enabled;

		public static readonly ADPropertyDefinition MailboxSubmissionAgentLogEnabled = MailboxTransportServerADSchema.MailboxSubmissionAgentLogEnabled;

		public static readonly ADPropertyDefinition MailboxSubmissionAgentLogMaxAge = MailboxTransportServerADSchema.MailboxSubmissionAgentLogMaxAge;

		public static readonly ADPropertyDefinition MailboxSubmissionAgentLogMaxDirectorySize = MailboxTransportServerADSchema.MailboxSubmissionAgentLogMaxDirectorySize;

		public static readonly ADPropertyDefinition MailboxSubmissionAgentLogMaxFileSize = MailboxTransportServerADSchema.MailboxSubmissionAgentLogMaxFileSize;

		public static readonly ADPropertyDefinition MailboxSubmissionAgentLogPath = MailboxTransportServerADSchema.MailboxSubmissionAgentLogPath;

		public static readonly ADPropertyDefinition MaxConcurrentMailboxSubmissions = MailboxTransportServerADSchema.MaxConcurrentMailboxSubmissions;

		public static readonly ADPropertyDefinition MaxConcurrentMailboxDeliveries = MailboxTransportServerADSchema.MaxConcurrentMailboxDeliveries;

		public static readonly ADPropertyDefinition NetworkAddress = MailboxTransportServerADSchema.NetworkAddress;

		public static readonly ADPropertyDefinition PipelineTracingEnabled = MailboxTransportServerADSchema.PipelineTracingEnabled;

		public static readonly ADPropertyDefinition PipelineTracingPath = MailboxTransportServerADSchema.PipelineTracingPath;

		public static readonly ADPropertyDefinition PipelineTracingSenderAddress = MailboxTransportServerADSchema.PipelineTracingSenderAddress;

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxAge = MailboxTransportServerADSchema.ReceiveProtocolLogMaxAge;

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxDirectorySize = MailboxTransportServerADSchema.ReceiveProtocolLogMaxDirectorySize;

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxFileSize = MailboxTransportServerADSchema.ReceiveProtocolLogMaxFileSize;

		public static readonly ADPropertyDefinition ReceiveProtocolLogPath = MailboxTransportServerADSchema.ReceiveProtocolLogPath;

		public static readonly ADPropertyDefinition SendProtocolLogMaxAge = MailboxTransportServerADSchema.SendProtocolLogMaxAge;

		public static readonly ADPropertyDefinition SendProtocolLogMaxDirectorySize = MailboxTransportServerADSchema.SendProtocolLogMaxDirectorySize;

		public static readonly ADPropertyDefinition SendProtocolLogMaxFileSize = MailboxTransportServerADSchema.SendProtocolLogMaxFileSize;

		public static readonly ADPropertyDefinition SendProtocolLogPath = MailboxTransportServerADSchema.SendProtocolLogPath;

		public static readonly ADPropertyDefinition VersionNumber = MailboxTransportServerADSchema.VersionNumber;
	}
}
