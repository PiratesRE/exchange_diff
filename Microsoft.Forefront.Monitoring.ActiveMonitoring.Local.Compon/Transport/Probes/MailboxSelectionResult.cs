using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	public enum MailboxSelectionResult
	{
		Success,
		NoMonitoringMDBs,
		NoMonitoringMDBsAreActive
	}
}
