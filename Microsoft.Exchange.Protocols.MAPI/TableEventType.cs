using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public enum TableEventType
	{
		Changed = 1,
		Error,
		RowAdded,
		RowDeleted,
		RowModified,
		SortDone,
		RestrictDone,
		SetcolDone,
		Reload
	}
}
