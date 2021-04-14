using System;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface IRefinerResult
	{
		string Value { get; }

		long Count { get; }

		void Merge(IRefinerResult result);
	}
}
