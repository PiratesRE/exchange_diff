using System;

namespace Microsoft.Exchange.MailboxLoadBalance.Injector
{
	internal enum InjectorServiceCapabilities
	{
		InjectSpecificTarget,
		BandAsMetric,
		ConsumerMetrics,
		GenericMetricTransmission,
		MaxElement
	}
}
