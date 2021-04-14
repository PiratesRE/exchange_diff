using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum DatabaseCopyAutoDagFlags
	{
		None = 0,
		BeingRelocated = 1,
		HostServerUnlinked = 2
	}
}
