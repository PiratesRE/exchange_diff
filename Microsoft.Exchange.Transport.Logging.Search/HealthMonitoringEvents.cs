using System;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal enum HealthMonitoringEvents
	{
		DeliveryReports_Search_Latencies,
		DeliveryReports_Get_Latencies,
		DeliveryReports_Search_Errors,
		DeliveryReports_Get_Errors,
		TransportSync_DispatchStats,
		TransportSync_LatencyStats,
		TransportSync_ManualSubscriptionStats,
		TransportSync_ProvisionedSubscriptionStats,
		TransportSync_PoisonSubscriptionStats,
		TransportSync_SyncStats,
		TransportSync_SubscriptionStats,
		TransportSync_TenantStats
	}
}
