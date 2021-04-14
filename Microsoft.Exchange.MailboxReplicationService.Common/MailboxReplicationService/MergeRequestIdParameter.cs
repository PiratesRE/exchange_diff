using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MergeRequestIdParameter : MRSRequestIdParameter
	{
		public MergeRequestIdParameter()
		{
		}

		public MergeRequestIdParameter(MergeRequest request) : base(request)
		{
		}

		public MergeRequestIdParameter(RequestJobObjectId requestJobId) : base(requestJobId)
		{
		}

		public MergeRequestIdParameter(MergeRequestStatistics requestStats) : base(requestStats)
		{
		}

		public MergeRequestIdParameter(RequestIndexEntryObjectId identity) : base(identity)
		{
		}

		public MergeRequestIdParameter(Guid guid) : base(guid)
		{
		}

		public MergeRequestIdParameter(string request) : base(request)
		{
		}

		public static MergeRequestIdParameter Parse(string request)
		{
			return new MergeRequestIdParameter(request);
		}
	}
}
