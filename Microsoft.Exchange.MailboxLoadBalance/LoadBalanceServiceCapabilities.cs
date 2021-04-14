using System;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	internal enum LoadBalanceServiceCapabilities
	{
		BandDataRetrieval,
		BandAsMetric,
		SoftDeletedRemoval,
		ConsumerMetrics,
		GenericMetricTransmission,
		CapacitySummary,
		MaxElement
	}
}
