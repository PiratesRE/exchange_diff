using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum FolderQueryFlags
	{
		None = 0,
		SoftDeleted = 1,
		DeepTraversal = 2,
		SuppressNotificationsOnMyActions = 4,
		NoNotifications = 8
	}
}
