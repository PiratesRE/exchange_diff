using System;

namespace Microsoft.Exchange.Diagnostics.CommonLog
{
	public static class PublicLogFields
	{
		public const string Time = "date-time";

		public const string EventID = "event-id";

		public const string ClientIP = "client-ip";

		public const string ServerIP = "server-ip";

		public const string MessageID = "message-id";

		public const string Subject = "message-subject";

		public const string Sender = "sender-address";

		public const string RecipientAddresses = "recipient-address";

		public const string RecipientStatuses = "recipient-status";

		public const string TotalBytes = "total-bytes";

		public const string RecipientCount = "recipient-count";

		public const string TenantID = "tenant-id";

		public const string OriginalClientIP = "original-client-ip";

		public const string OriginalServerIP = "original-server-ip";
	}
}
