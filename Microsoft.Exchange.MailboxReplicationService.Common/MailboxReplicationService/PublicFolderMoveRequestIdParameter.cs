using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class PublicFolderMoveRequestIdParameter : MRSRequestIdParameter
	{
		public PublicFolderMoveRequestIdParameter()
		{
		}

		public PublicFolderMoveRequestIdParameter(PublicFolderMoveRequest request) : base(request)
		{
		}

		public PublicFolderMoveRequestIdParameter(RequestJobObjectId requestJobId) : base(requestJobId)
		{
		}

		public PublicFolderMoveRequestIdParameter(PublicFolderMoveRequestStatistics requestStats) : base(requestStats)
		{
		}

		public PublicFolderMoveRequestIdParameter(RequestIndexEntryObjectId identity) : base(identity)
		{
		}

		public PublicFolderMoveRequestIdParameter(Guid guid) : base(guid)
		{
		}

		public PublicFolderMoveRequestIdParameter(string request) : base(request)
		{
			if (base.MailboxName != null)
			{
				base.OrganizationName = base.MailboxName;
				base.MailboxName = null;
			}
		}

		public static PublicFolderMoveRequestIdParameter Parse(string request)
		{
			return new PublicFolderMoveRequestIdParameter(request);
		}
	}
}
