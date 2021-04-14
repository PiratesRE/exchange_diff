using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IIndex
	{
		IReadOnlyDictionary<Column, Column> RenameDictionary { get; }

		SortOrder SortOrder { get; }

		SortOrder LogicalSortOrder { get; }

		Table Table { get; }

		Table IndexTable { get; }

		IList<object> IndexKeyPrefix { get; }

		IList<Column> Columns { get; }

		ISet<Column> ConstantColumns { get; }

		bool GetIndexColumn(Column column, bool acceptTruncated, out Column indexColumn);
	}
}
