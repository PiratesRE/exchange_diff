using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchTransientException : MultiMailboxSearchException
	{
		public SearchTransientException(SearchType searchType, Exception innerException) : base(Strings.SearchTransientError((searchType == SearchType.Preview) ? "Preview" : "Statistics", innerException.Message), innerException)
		{
		}

		protected SearchTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
