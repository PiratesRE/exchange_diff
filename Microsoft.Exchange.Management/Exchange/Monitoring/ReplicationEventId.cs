using System;

namespace Microsoft.Exchange.Monitoring
{
	internal static class ReplicationEventId
	{
		public const int NoMonitoringError = 10000;

		public const int CannotReadRoleFromRegistry = 10001;

		public const int NoMailboxRoleInstalled = 10002;

		public const int ServerConfigurationError = 10003;

		public const int HighPriorityCheckFailedError = 10004;

		public const int HighPriorityCheckWarningError = 10005;

		public const int DatabaseCheckFailedError = 10006;

		public const int DatabaseCheckWarningError = 10007;

		public const int MediumPriorityCheckFailedError = 10008;

		public const int MediumPriorityCheckWarningError = 10009;

		public const int CannotLocateServer = 10010;

		public const int CannotRunMonitoringTaskRemotely = 10011;
	}
}
