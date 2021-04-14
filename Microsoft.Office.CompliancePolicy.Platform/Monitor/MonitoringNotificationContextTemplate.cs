using System;

namespace Microsoft.Office.CompliancePolicy.Monitor
{
	internal static class MonitoringNotificationContextTemplate
	{
		public const string PolicySyncTimeExceededEventError = "UnifiedPolicySync.PolicySyncTimeExceededError";

		public const string PolicySyncPermanentError = "UnifiedPolicySync.PermanentError";

		public const string PolicySyncPermanentStatusPublishError = "UnifiedPolicySync.PermanentStatusPublishError";

		public const string PolicySyncSendNotificationError = "UnifiedPolicySync.SendNotificationError";

		public const string PermanentSyncFailure = "NotificationId={0}\r\nTimestamp={1}\r\n{2}";

		public const string PermanentSyncStatusFailure = "Timestamp={0};Sync status notification Id={1}";

		public const string ExpectedSyncTimeExceeded = "NotificationId={0};Timestamp={1}\r\nLatencies:\r\nTotalProcessTime={2},TryCount={3}\r\nCurrentCycle:NotifyPickUpDelay={4};Initialization={5};{6}";

		public const string SendNotificationError = "Workload={0};Timestamp={1}";
	}
}
