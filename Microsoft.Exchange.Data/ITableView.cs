using System;

namespace Microsoft.Exchange.Data
{
	internal interface ITableView : IPagedView
	{
		int EstimatedRowCount { get; }

		int CurrentRow { get; }

		bool SeekToCondition(SeekReference seekReference, QueryFilter seekFilter);

		int SeekToOffset(SeekReference seekReference, int offset);
	}
}
