using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ModifyTableOptions
	{
		None = 0,
		FreeBusyAware = 1,
		ExtendedPermissionInformation = 2
	}
}
