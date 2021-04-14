using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public enum SmtpFailureCategory
	{
		TransportComponent,
		MonitoringComponent,
		DnsFailure,
		DependentComponent,
		DependentCoveredComponent
	}
}
