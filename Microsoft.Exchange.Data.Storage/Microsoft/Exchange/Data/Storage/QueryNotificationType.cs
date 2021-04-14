using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum QueryNotificationType
	{
		QueryResultChanged = 1,
		Error,
		RowAdded,
		RowDeleted,
		RowModified,
		SortDone,
		RestrictDone,
		SetColumnDone,
		Reload
	}
}
