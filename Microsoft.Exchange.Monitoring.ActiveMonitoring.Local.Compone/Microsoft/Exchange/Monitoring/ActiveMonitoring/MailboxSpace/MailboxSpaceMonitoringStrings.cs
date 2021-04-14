using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.MailboxSpace
{
	public static class MailboxSpaceMonitoringStrings
	{
		public const string DatabaseSpaceProbeName = "DatabaseSpaceProbe";

		public const string DatabaseSizeMonitorName = "DatabaseSizeMonitor";

		public const string DatabaseSizeEscalationProcessingMonitorName = "DatabaseSizeEscalationProcessingMonitor";

		public const string StorageLogicalDriveSpaceMonitorName = "StorageLogicalDriveSpaceMonitor";

		public const string DatabaseSizeProvisioningResponderName = "DatabaseSizeProvisioning";

		public const string DatabaseSizeEscalateResponderName = "DatabaseSizeEscalate";

		public const string DatabaseSizeEscalationNotificationResponderName = "DatabaseSizeEscalationNotification";

		public const string StorageLogicalDriveSpaceEscalateResponderName = "StorageLogicalDriveSpaceEscalate";

		public const string DatabaseLogicalPhysicalSizeRatioMonitorName = "DatabaseLogicalPhysicalSizeRatioMonitor";

		public const string SetDatabaseLogicalPhysicalSizeRatioMonitorStateRepairingResponderName = "SetDatabaseLogicalPhysicalSizeRatioMonitorStateRepairing";

		public const string DatabaseLogicalPhysicalSizeRatioEscalationProcessingMonitorName = "DatabaseLogicalPhysicalSizeRatioEscalationProcessingMonitor";

		public const string SetDatabaseLogicalPhysicalSizeRatioEscalationProcessingMonitorStateRepairingResponderName = "SetDatabaseLogicalPhysicalSizeRatioEscalationProcessingMonitorStateRepairing";

		public const string DatabaseLogicalPhysicalSizeRatioEscalationNotificationResponderName = "DatabaseLogicalPhysicalSizeRatioEscalationNotification";

		public const string DatabaseLogicalPhysicalSizeRatioEscalateResponderName = "DatabaseLogicalPhysicalSizeRatioEscalate";

		public const string UnableToGetDiskCapacity = "DiskCapacityForDatabase{0}NotPopulatedByDatabaseSpaceProbe";

		public const string InvokeMonitoringProbeCommand = "Invoke-MonitoringProbe -Identity '{0}\\{1}\\{2}' -Server {3}";

		public const string GetAllUnhealthyMonitors = "Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}";
	}
}
