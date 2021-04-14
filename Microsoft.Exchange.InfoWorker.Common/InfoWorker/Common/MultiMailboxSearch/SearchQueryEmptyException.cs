using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchQueryEmptyException : MultiMailboxSearchException
	{
		public SearchQueryEmptyException() : base(Strings.SearchQueryEmpty)
		{
		}

		protected SearchQueryEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
