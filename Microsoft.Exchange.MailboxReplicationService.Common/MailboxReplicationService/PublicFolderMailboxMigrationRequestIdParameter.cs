using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class PublicFolderMailboxMigrationRequestIdParameter : MRSRequestIdParameter
	{
		public PublicFolderMailboxMigrationRequestIdParameter()
		{
		}

		public PublicFolderMailboxMigrationRequestIdParameter(PublicFolderMailboxMigrationRequest request) : base(request)
		{
		}

		public PublicFolderMailboxMigrationRequestIdParameter(RequestJobObjectId requestJobId) : base(requestJobId)
		{
		}

		public PublicFolderMailboxMigrationRequestIdParameter(PublicFolderMailboxMigrationRequestStatistics requestStats) : base(requestStats)
		{
		}

		public PublicFolderMailboxMigrationRequestIdParameter(RequestIndexEntryObjectId identity) : base(identity)
		{
		}

		public PublicFolderMailboxMigrationRequestIdParameter(Guid guid) : base(guid)
		{
		}

		public PublicFolderMailboxMigrationRequestIdParameter(string request) : base(request)
		{
			if (base.MailboxName != null)
			{
				base.OrganizationName = base.MailboxName;
				base.MailboxName = null;
			}
		}

		public static PublicFolderMailboxMigrationRequestIdParameter Parse(string request)
		{
			return new PublicFolderMailboxMigrationRequestIdParameter(request);
		}
	}
}
