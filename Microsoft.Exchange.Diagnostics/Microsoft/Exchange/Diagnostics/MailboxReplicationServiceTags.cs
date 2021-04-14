using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MailboxReplicationServiceTags
	{
		public const int MailboxReplicationService = 0;

		public const int MailboxReplicationServiceProvider = 1;

		public const int MailboxReplicationProxyClient = 2;

		public const int MailboxReplicationProxyService = 3;

		public const int MailboxReplicationCmdlet = 4;

		public const int MailboxReplicationUpdateMovedMailbox = 5;

		public const int MailboxReplicationServiceThrottling = 6;

		public const int MailboxReplicationAuthorization = 7;

		public const int MailboxReplicationCommon = 8;

		public const int MailboxReplicationResourceHealth = 9;

		public static Guid guid = new Guid("1141b405-c15a-48ef-a440-ab2f44a9cdac");
	}
}
