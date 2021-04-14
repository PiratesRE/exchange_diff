using System;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal sealed class MailboxCrawlerState : MailboxState
	{
		public MailboxCrawlerState(int mailboxNumber, int lastDocumentIdIndexed, int attemptCount = 0) : base(mailboxNumber, lastDocumentIdIndexed)
		{
			if (lastDocumentIdIndexed == -2)
			{
				this.RecrawlMailbox = true;
			}
			this.AttemptCount = attemptCount;
		}

		private MailboxCrawlerState()
		{
		}

		public static int MaxCrawlAttemptCount
		{
			get
			{
				return MailboxCrawlerState.maxCrawlAttemptCount;
			}
		}

		public int LastDocumentIdIndexed
		{
			get
			{
				return base.RawState;
			}
			set
			{
				base.RawState = value;
			}
		}

		public bool RecrawlMailbox { get; set; }

		public int AttemptCount { get; set; }

		private static int maxCrawlAttemptCount = SearchConfig.Instance.MaxCrawlAttemptCount;
	}
}
