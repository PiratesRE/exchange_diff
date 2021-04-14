using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal interface IRemoteDeliveryHealthPerformanceCounters
	{
		void UpdateOutboundIPPoolPerfCounter(string pool, RiskLevel riskLevel, int percentageQueueErrors);

		void UpdateSmtpResponseSubCodePerfCounter(int code, int messageCount);
	}
}
