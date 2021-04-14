using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MailboxTableFlags
	{
		MailboxTableFlagsNone = 0,
		IncludeSoftDeletedMailbox = 1,
		MaintenanceItems = 3,
		MaintenanceItemsWithDS = 4,
		UrgentMaintenanceItems = 5
	}
}
