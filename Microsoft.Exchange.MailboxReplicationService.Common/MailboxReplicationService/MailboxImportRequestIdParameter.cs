using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MailboxImportRequestIdParameter : MRSRequestIdParameter
	{
		public MailboxImportRequestIdParameter()
		{
		}

		public MailboxImportRequestIdParameter(MailboxImportRequest request) : base(request)
		{
		}

		public MailboxImportRequestIdParameter(RequestJobObjectId requestJobId) : base(requestJobId)
		{
		}

		public MailboxImportRequestIdParameter(MailboxImportRequestStatistics requestStats) : base(requestStats)
		{
		}

		public MailboxImportRequestIdParameter(RequestIndexEntryObjectId identity) : base(identity)
		{
		}

		public MailboxImportRequestIdParameter(Guid guid) : base(guid)
		{
		}

		public MailboxImportRequestIdParameter(string request) : base(request)
		{
		}

		public static MailboxImportRequestIdParameter Parse(string request)
		{
			return new MailboxImportRequestIdParameter(request);
		}
	}
}
