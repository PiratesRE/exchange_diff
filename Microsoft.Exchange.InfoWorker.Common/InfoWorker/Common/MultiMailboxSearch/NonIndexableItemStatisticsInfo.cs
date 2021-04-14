using System;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class NonIndexableItemStatisticsInfo
	{
		public NonIndexableItemStatisticsInfo(string mailbox, int itemCount, string errorMessage)
		{
			this.Mailbox = mailbox;
			this.ItemCount = itemCount;
			this.ErrorMessage = errorMessage;
		}

		public string Mailbox { get; set; }

		public int ItemCount { get; set; }

		public string ErrorMessage { get; set; }
	}
}
