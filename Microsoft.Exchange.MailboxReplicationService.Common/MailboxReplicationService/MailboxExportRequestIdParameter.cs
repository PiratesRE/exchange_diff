using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MailboxExportRequestIdParameter : MRSRequestIdParameter
	{
		public MailboxExportRequestIdParameter()
		{
		}

		public MailboxExportRequestIdParameter(MailboxExportRequest request) : base(request)
		{
		}

		public MailboxExportRequestIdParameter(RequestJobObjectId requestJobId) : base(requestJobId)
		{
		}

		public MailboxExportRequestIdParameter(MailboxExportRequestStatistics requestStats) : base(requestStats)
		{
		}

		public MailboxExportRequestIdParameter(RequestIndexEntryObjectId identity) : base(identity)
		{
		}

		public MailboxExportRequestIdParameter(Guid guid) : base(guid)
		{
		}

		public MailboxExportRequestIdParameter(string request) : base(request)
		{
		}

		public static MailboxExportRequestIdParameter Parse(string request)
		{
			return new MailboxExportRequestIdParameter(request);
		}
	}
}
