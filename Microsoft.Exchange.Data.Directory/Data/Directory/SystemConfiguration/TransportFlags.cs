using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum TransportFlags
	{
		None = 0,
		MessageTrackingLogEnabled = 1,
		ExternalDNSAdapterDisabled = 2,
		InternalDNSAdapterDisabled = 4,
		PoisonMessageDetectionEnabled = 8,
		IrmLogEnabled = 1024,
		RecipientValidationCacheEnabled = 2048,
		AntispamAgentsEnabled = 4096,
		ConnectivityLogEnabled = 8192,
		MessageTrackingLogSubjectLoggingEnabled = 16384,
		PipelineTracingEnabled = 32768,
		GatewayEdgeSyncSubscribed = 65536,
		AntispamUpdatesEnabled = 524288,
		ContentConversionTracingEnabled = 1048576,
		UseDowngradedExchangeServerAuth = 2097152,
		MailboxDeliverySmtpUtf8Enabled = 4194304
	}
}
