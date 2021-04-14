using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.RpcClientAccess
{
	public static class RootCause
	{
		public const string Unknown = "UnknownIssue";

		public const string HighLatency = "HighLatency";

		public const string Passive = "Passive";

		public const string SecureChannelFailure = "SecureChannel";

		public const string NetworkingFailure = "Networking";

		public const string MailboxUpgrade = "MailboxUpgrade";

		public const string AccountIssue = "AccountIssue";

		public const string Unauthorized = "Unauthorized";

		public const string MapiHttpVersionMismatch = "MapiHttpVersionMismatch";

		public const string StoreFailure = "StoreFailure";
	}
}
