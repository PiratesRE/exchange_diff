using System;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface ISearchTaskResult : ISearchResult
	{
		SearchType ResultType { get; }

		bool Success { get; }
	}
}
