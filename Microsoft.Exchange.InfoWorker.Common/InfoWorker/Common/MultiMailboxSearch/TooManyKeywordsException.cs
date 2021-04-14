using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class TooManyKeywordsException : DiscoverySearchPermanentException
	{
		public TooManyKeywordsException(int keywordCount, int maxAllowedKeywordCount) : base(Strings.MaxAllowedKeywordsExceeded(keywordCount, maxAllowedKeywordCount))
		{
		}

		protected TooManyKeywordsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
