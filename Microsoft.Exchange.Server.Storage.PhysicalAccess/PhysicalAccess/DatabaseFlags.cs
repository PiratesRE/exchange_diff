using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	[Flags]
	public enum DatabaseFlags : uint
	{
		None = 0U,
		CircularLoggingEnabled = 1U,
		IsMultiRole = 2U,
		BackgroundMaintenanceEnabled = 4U
	}
}
