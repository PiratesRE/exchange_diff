using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface IKeywordHit
	{
		string Phrase { get; }

		ulong Count { get; }

		ByteQuantifiedSize Size { get; }

		IList<Pair<MailboxInfo, Exception>> Errors { get; }

		void Merge(IKeywordHit hits);
	}
}
