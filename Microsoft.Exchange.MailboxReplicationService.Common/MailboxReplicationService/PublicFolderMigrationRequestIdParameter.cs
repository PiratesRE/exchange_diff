using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class PublicFolderMigrationRequestIdParameter : MRSRequestIdParameter
	{
		public PublicFolderMigrationRequestIdParameter()
		{
		}

		public PublicFolderMigrationRequestIdParameter(PublicFolderMigrationRequest request) : base(request)
		{
		}

		public PublicFolderMigrationRequestIdParameter(RequestJobObjectId requestJobId) : base(requestJobId)
		{
		}

		public PublicFolderMigrationRequestIdParameter(PublicFolderMigrationRequestStatistics requestStats) : base(requestStats)
		{
		}

		public PublicFolderMigrationRequestIdParameter(RequestIndexEntryObjectId identity) : base(identity)
		{
		}

		public PublicFolderMigrationRequestIdParameter(Guid guid) : base(guid)
		{
		}

		public PublicFolderMigrationRequestIdParameter(string request) : base(request)
		{
			if (base.MailboxName != null)
			{
				base.OrganizationName = base.MailboxName;
				base.MailboxName = null;
			}
		}

		public static PublicFolderMigrationRequestIdParameter Parse(string request)
		{
			return new PublicFolderMigrationRequestIdParameter(request);
		}
	}
}
