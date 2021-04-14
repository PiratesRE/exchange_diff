using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum HierarchyTableFlags
	{
		None = 0,
		ConvenientDepth = 1,
		ShowSoftDeletes = 2,
		NoNotifications = 32,
		SuppressNotificationsOnMyActions = 4096,
		DeferredErrors = 8,
		Unicode = -2147483648
	}
}
