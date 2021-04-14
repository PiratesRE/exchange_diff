using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MailboxRelocationRequestIdParameter : MRSRequestIdParameter
	{
		public MailboxRelocationRequestIdParameter()
		{
		}

		public MailboxRelocationRequestIdParameter(MailboxRelocationRequest request) : base(request)
		{
		}

		public MailboxRelocationRequestIdParameter(RequestJobObjectId requestJobId) : base(requestJobId)
		{
		}

		public MailboxRelocationRequestIdParameter(MailboxRelocationRequestStatistics requestStats) : base(requestStats)
		{
		}

		public MailboxRelocationRequestIdParameter(RequestIndexEntryObjectId identity) : base(identity)
		{
		}

		public MailboxRelocationRequestIdParameter(Guid guid) : base(guid)
		{
		}

		public MailboxRelocationRequestIdParameter(string request) : base(request)
		{
		}
	}
}
