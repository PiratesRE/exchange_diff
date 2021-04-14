using System;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal enum ThrottlingSource
	{
		SmtpThrottlingAgent,
		PrioritizationAgent,
		ConditionalQueuing,
		ProcessingQuota,
		QueueQuota,
		Journaling,
		ResourceManager,
		MailboxDelivery,
		MSExchangeThrottling
	}
}
