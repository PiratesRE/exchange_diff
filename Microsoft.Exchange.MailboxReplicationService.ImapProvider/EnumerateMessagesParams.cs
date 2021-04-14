using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class EnumerateMessagesParams
	{
		public EnumerateMessagesParams(int highFetchValue, int lowFetchValue, FetchMessagesFlags inputFetchFlags = FetchMessagesFlags.None)
		{
			this.FetchMessagesFlags = (FetchMessagesFlags.FetchBySeqNum | inputFetchFlags);
			this.HighFetchValue = highFetchValue;
			this.LowFetchValue = lowFetchValue;
		}

		public FetchMessagesFlags FetchMessagesFlags { get; private set; }

		public int HighFetchValue { get; private set; }

		public int LowFetchValue { get; private set; }

		private const FetchMessagesFlags FetchFlags = FetchMessagesFlags.FetchBySeqNum;
	}
}
