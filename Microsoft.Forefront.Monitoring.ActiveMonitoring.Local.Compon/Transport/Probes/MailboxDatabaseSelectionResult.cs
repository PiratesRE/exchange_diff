using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	public enum MailboxDatabaseSelectionResult
	{
		Success,
		NoMonitoringMDBs,
		NoMonitoringMDBsAreActive,
		NoLocalEndpointManager
	}
}
