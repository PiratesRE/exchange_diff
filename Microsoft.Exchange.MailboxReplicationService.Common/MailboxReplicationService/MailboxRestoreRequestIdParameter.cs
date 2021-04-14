using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MailboxRestoreRequestIdParameter : MRSRequestIdParameter
	{
		public MailboxRestoreRequestIdParameter()
		{
		}

		public MailboxRestoreRequestIdParameter(MailboxRestoreRequest request) : base(request)
		{
		}

		public MailboxRestoreRequestIdParameter(RequestJobObjectId requestJobId) : base(requestJobId)
		{
		}

		public MailboxRestoreRequestIdParameter(MailboxRestoreRequestStatistics requestStats) : base(requestStats)
		{
		}

		public MailboxRestoreRequestIdParameter(RequestIndexEntryObjectId identity) : base(identity)
		{
		}

		public MailboxRestoreRequestIdParameter(Guid guid) : base(guid)
		{
		}

		public MailboxRestoreRequestIdParameter(string request) : base(request)
		{
		}

		public static MailboxRestoreRequestIdParameter Parse(string request)
		{
			return new MailboxRestoreRequestIdParameter(request);
		}
	}
}
