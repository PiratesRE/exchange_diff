using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Assistants
{
	internal class AssistantEndWorkCycleCheckpointStatistics
	{
		public string DatabaseName { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public int TotalMailboxCount { get; set; }

		public int ProcessedMailboxCount { get; set; }

		public int MailboxErrorCount { get; set; }

		public int FailedToOpenStoreSessionCount { get; set; }

		public int RetriedMailboxCount { get; set; }

		public int MailboxesProcessedSeparatelyCount { get; set; }

		public int MailboxRemainingCount { get; set; }

		public List<KeyValuePair<string, object>> FormatCustomData()
		{
			return new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("DatabaseName", string.Format("{0}", this.DatabaseName)),
				new KeyValuePair<string, object>("StartTime", string.Format("{0}", this.StartTime)),
				new KeyValuePair<string, object>("EndTime", string.Format("{0}", this.EndTime)),
				new KeyValuePair<string, object>("TotalMailboxCount", string.Format("{0}", this.TotalMailboxCount)),
				new KeyValuePair<string, object>("ProcessedMailboxCount", string.Format("{0}", this.ProcessedMailboxCount)),
				new KeyValuePair<string, object>("MailboxErrorCount", string.Format("{0}", this.MailboxErrorCount)),
				new KeyValuePair<string, object>("FailedToOpenStoreSessionCount", string.Format("{0}", this.FailedToOpenStoreSessionCount)),
				new KeyValuePair<string, object>("RetriedMailboxCount", string.Format("{0}", this.RetriedMailboxCount)),
				new KeyValuePair<string, object>("MailboxesProcessedSeparatelyCount", string.Format("{0}", this.MailboxesProcessedSeparatelyCount)),
				new KeyValuePair<string, object>("MailboxRemainingCount", string.Format("{0}", this.MailboxRemainingCount))
			};
		}
	}
}
