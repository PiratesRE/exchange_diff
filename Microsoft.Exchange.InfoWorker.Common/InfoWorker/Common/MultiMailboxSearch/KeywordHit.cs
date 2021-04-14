using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class KeywordHit : IKeywordHit
	{
		public KeywordHit(string phrase, ulong count, ByteQuantifiedSize size)
		{
			Util.ThrowOnNull(phrase, "phrase");
			if (string.IsNullOrEmpty(phrase))
			{
				throw new ArgumentException("Phrase cannot be empty");
			}
			if (count == 0UL && size != ByteQuantifiedSize.Zero)
			{
				throw new ArgumentException("count is zero but size is not zero");
			}
			this.phrase = phrase;
			this.count = count;
			this.size = size;
		}

		public KeywordHit(string phrase, MailboxInfo mailbox, Exception exception)
		{
			Util.ThrowOnNull(phrase, "phrase");
			Util.ThrowOnNull(mailbox, "mailbox");
			Util.ThrowOnNull(exception, "exception");
			if (string.IsNullOrEmpty(phrase))
			{
				throw new ArgumentException("Phrase cannot be empty");
			}
			this.phrase = phrase;
			this.errors.Add(new Pair<MailboxInfo, Exception>(mailbox, exception));
		}

		public string Phrase
		{
			get
			{
				return this.phrase;
			}
		}

		public ulong Count
		{
			get
			{
				return this.count;
			}
		}

		public ByteQuantifiedSize Size
		{
			get
			{
				return this.size;
			}
		}

		public IList<Pair<MailboxInfo, Exception>> Errors
		{
			get
			{
				return this.errors;
			}
		}

		public void Merge(IKeywordHit hits)
		{
			if (hits == null)
			{
				return;
			}
			if (string.Compare(this.phrase, hits.Phrase, StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new ArgumentException("Keyword hits: Invalid merge");
			}
			checked
			{
				if (hits.Count != 0UL)
				{
					this.count += hits.Count;
					this.size += hits.Size;
				}
				this.errors.AddRange(((KeywordHit)hits).Errors);
			}
		}

		public const string UnsearchablePhrase = "652beee2-75f7-4ca0-8a02-0698a3919cb9";

		private string phrase;

		private ulong count;

		private ByteQuantifiedSize size;

		private List<Pair<MailboxInfo, Exception>> errors = new List<Pair<MailboxInfo, Exception>>(1);
	}
}
