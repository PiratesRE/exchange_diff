using System;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	public enum QueueDigestGroupBy
	{
		NextHopDomain,
		NextHopCategory,
		NextHopKey,
		DeliveryType,
		Status,
		RiskLevel,
		LastError,
		ServerName,
		OutboundIPPool
	}
}
