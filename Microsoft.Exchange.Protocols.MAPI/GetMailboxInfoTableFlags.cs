using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public enum GetMailboxInfoTableFlags : uint
	{
		None,
		IncludeSoftDeleted,
		FinalCleanup,
		MaintenanceItems,
		MaintenanceItemsWithDS,
		UrgentMaintenanceItems
	}
}
