using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Transport
{
	internal interface ITransportAppConfig
	{
		TransportAppConfig.ResourceManagerConfig ResourceManager { get; }

		TransportAppConfig.JetDatabaseConfig JetDatabase { get; }

		TransportAppConfig.DumpsterConfig Dumpster { get; }

		TransportAppConfig.ShadowRedundancyConfig ShadowRedundancy { get; }

		TransportAppConfig.RemoteDeliveryConfig RemoteDelivery { get; }

		TransportAppConfig.MapiSubmissionConfig MapiSubmission { get; }

		TransportAppConfig.ResolverConfig Resolver { get; }

		TransportAppConfig.RoutingConfig Routing { get; }

		TransportAppConfig.ContentConversionConfig ContentConversion { get; }

		TransportAppConfig.IPFilteringDatabaseConfig IPFilteringDatabase { get; }

		TransportAppConfig.IMessageResubmissionConfig MessageResubmission { get; }

		TransportAppConfig.QueueDatabaseConfig QueueDatabase { get; }

		TransportAppConfig.WorkerProcessConfig WorkerProcess { get; }

		TransportAppConfig.LatencyTrackerConfig LatencyTracker { get; }

		TransportAppConfig.RecipientValidatorConfig RecipientValidtor { get; }

		TransportAppConfig.PerTenantCacheConfig PerTenantCache { get; }

		TransportAppConfig.MessageThrottlingConfiguration MessageThrottlingConfig { get; }

		TransportAppConfig.SMTPOutConnectionCacheConfig ConnectionCacheConfig { get; }

		TransportAppConfig.IsMemberOfResolverConfiguration TransportIsMemberOfResolverConfig { get; }

		TransportAppConfig.IsMemberOfResolverConfiguration MailboxRulesIsMemberOfResolverConfig { get; }

		TransportAppConfig.SmtpAvailabilityConfig SmtpAvailabilityConfiguration { get; }

		TransportAppConfig.SmtpDataConfig SmtpDataConfiguration { get; }

		TransportAppConfig.SmtpMailCommandConfig SmtpMailCommandConfiguration { get; }

		TransportAppConfig.MessageContextBlobConfig MessageContextBlobConfiguration { get; }

		TransportAppConfig.SmtpReceiveConfig SmtpReceiveConfiguration { get; }

		TransportAppConfig.SmtpSendConfig SmtpSendConfiguration { get; }

		TransportAppConfig.SmtpProxyConfig SmtpProxyConfiguration { get; }

		TransportAppConfig.SmtpInboundProxyConfig SmtpInboundProxyConfiguration { get; }

		TransportAppConfig.SmtpOutboundProxyConfig SmtpOutboundProxyConfiguration { get; }

		TransportAppConfig.DeliveryQueuePrioritizationConfig DeliveryQueuePrioritizationConfiguration { get; }

		TransportAppConfig.QueueConfig QueueConfiguration { get; }

		TransportAppConfig.DeliveryFailureConfig DeliveryFailureConfiguration { get; }

		TransportAppConfig.SecureMailConfig SecureMail { get; }

		TransportAppConfig.LoggingConfig Logging { get; }

		TransportAppConfig.FlowControlLogConfig FlowControlLog { get; }

		TransportAppConfig.ConditionalThrottlingConfig ThrottlingConfig { get; }

		TransportAppConfig.TransportRulesConfig TransportRuleConfig { get; }

		TransportAppConfig.PoisonMessageConfig PoisonMessage { get; }

		TransportAppConfig.SmtpMessageThrottlingAgentConfig SmtpMessageThrottlingConfig { get; }

		TransportAppConfig.StateManagementConfig StateManagement { get; }

		TransportAppConfig.BootLoaderConfig BootLoader { get; }

		TransportAppConfig.ProcessingQuotaConfig ProcessingQuota { get; }

		TransportAppConfig.ADPollingConfig ADPolling { get; }

		XElement GetDiagnosticInfo();
	}
}
