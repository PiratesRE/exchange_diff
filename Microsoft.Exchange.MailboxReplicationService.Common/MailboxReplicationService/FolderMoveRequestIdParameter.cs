using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class FolderMoveRequestIdParameter : MRSRequestIdParameter
	{
		public FolderMoveRequestIdParameter()
		{
		}

		public FolderMoveRequestIdParameter(FolderMoveRequest request) : base(request)
		{
		}

		public FolderMoveRequestIdParameter(RequestJobObjectId requestJobId) : base(requestJobId)
		{
		}

		public FolderMoveRequestIdParameter(FolderMoveRequestStatistics requestStats) : base(requestStats)
		{
		}

		public FolderMoveRequestIdParameter(RequestIndexEntryObjectId identity) : base(identity)
		{
		}

		public FolderMoveRequestIdParameter(Guid guid) : base(guid)
		{
		}

		public FolderMoveRequestIdParameter(string request) : base(request)
		{
			if (base.MailboxName != null)
			{
				base.OrganizationName = base.MailboxName;
				base.MailboxName = null;
			}
		}

		public static FolderMoveRequestIdParameter Parse(string request)
		{
			return new FolderMoveRequestIdParameter(request);
		}
	}
}
