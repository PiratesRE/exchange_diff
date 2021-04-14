using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class SearchMailboxResult : ISearchTaskResult, ISearchResult
	{
		public SearchMailboxResult(MailboxInfo mailbox, SortedResultPage result, ulong totalResultCount)
		{
			Util.ThrowOnNull(mailbox, "mailbox");
			Util.ThrowOnNull(result, "result");
			int resultCount = result.ResultCount;
			this.resultType = SearchType.Preview;
			this.mailbox = mailbox;
			this.result = result;
			this.totalResultCount = totalResultCount;
			this.mailboxStats = new List<MailboxStatistics>
			{
				new MailboxStatistics(mailbox, this.totalResultCount, this.TotalResultSize)
			};
			this.success = true;
		}

		public SearchMailboxResult(MailboxInfo mailbox, Exception error)
		{
			Util.ThrowOnNull(mailbox, "mailbox");
			Util.ThrowOnNull(error, "error");
			this.mailbox = mailbox;
			this.success = false;
			this.resultType = SearchType.Preview;
			this.totalResultCount = 0UL;
			this.exception = error;
		}

		public SearchMailboxResult(MailboxInfo mailbox, IKeywordHit keyword)
		{
			Util.ThrowOnNull(mailbox, "mailbox");
			Util.ThrowOnNull(keyword, "keyword");
			this.mailbox = mailbox;
			this.resultType = SearchType.Statistics;
			this.keywordHit = keyword;
			this.success = true;
			if (keyword.Errors.Count != 0)
			{
				this.success = false;
			}
		}

		public MailboxInfo Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public SearchType ResultType
		{
			get
			{
				return this.resultType;
			}
		}

		public SortedResultPage PreviewResult
		{
			get
			{
				return this.result;
			}
		}

		public IProtocolLog ProtocolLog
		{
			get
			{
				return null;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public List<Pair<MailboxInfo, Exception>> PreviewErrors
		{
			get
			{
				List<Pair<MailboxInfo, Exception>> list = null;
				if (this.exception != null)
				{
					list = new List<Pair<MailboxInfo, Exception>>(1);
					list.Add(new Pair<MailboxInfo, Exception>(this.mailbox, this.exception));
				}
				return list;
			}
		}

		public bool Success
		{
			get
			{
				return this.success;
			}
		}

		public IKeywordHit KeywordHit
		{
			get
			{
				return this.keywordHit;
			}
		}

		public ByteQuantifiedSize TotalResultSize
		{
			get
			{
				return ByteQuantifiedSize.Zero;
			}
		}

		public List<MailboxStatistics> MailboxStats
		{
			get
			{
				return this.mailboxStats;
			}
		}

		public IDictionary<string, IKeywordHit> KeywordStatistics
		{
			get
			{
				Dictionary<string, IKeywordHit> dictionary = null;
				if (this.keywordHit != null)
				{
					dictionary = new Dictionary<string, IKeywordHit>(1);
					dictionary.Add(this.keywordHit.Phrase, this.keywordHit);
				}
				return dictionary;
			}
		}

		public ulong TotalResultCount
		{
			get
			{
				return this.totalResultCount;
			}
		}

		public void MergeSearchResult(ISearchResult result)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, List<IRefinerResult>> RefinersResult
		{
			get
			{
				return null;
			}
		}

		private readonly MailboxInfo mailbox;

		private readonly SortedResultPage result;

		private readonly Exception exception;

		private readonly bool success;

		private readonly SearchType resultType;

		private readonly IKeywordHit keywordHit;

		private readonly ulong totalResultCount;

		private List<MailboxStatistics> mailboxStats;
	}
}
