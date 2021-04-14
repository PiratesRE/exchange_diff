using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Query
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstantSearchTransientException : StorageTransientException
	{
		public InstantSearchTransientException(LocalizedString message, QueryStatistics statistics) : base(message)
		{
			this.QueryStatistics = statistics;
		}

		public InstantSearchTransientException(LocalizedString message, Exception innerException, QueryStatistics statistics) : base(message, innerException)
		{
			this.QueryStatistics = statistics;
		}

		public QueryStatistics QueryStatistics { get; private set; }
	}
}
