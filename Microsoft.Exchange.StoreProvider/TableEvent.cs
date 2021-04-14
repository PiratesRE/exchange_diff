using System;

namespace Microsoft.Mapi
{
	internal enum TableEvent
	{
		TableChanged = 1,
		TableError,
		TableRowAdded,
		TableRowDeleted,
		TableRowModified,
		TableSortDone,
		TableRestrictDone,
		TableSetColDone,
		TableReload,
		TableRowDeletedExtended
	}
}
