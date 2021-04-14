using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal enum DomainSelection
	{
		AnyTargetServiceAnyZone,
		AllTargetServicesAnyZone,
		AnyTargetServiceAllZones,
		Ipv6AnyTargetServiceAnyZone,
		Ipv6AllTargetServicesAnyZone,
		Ipv6AnyTargetServiceAllZones,
		AnyZone,
		AllZones,
		InvalidDomainWithFallback,
		InvalidDomainWithoutFallback,
		InvalidZone,
		CustomQuery
	}
}
