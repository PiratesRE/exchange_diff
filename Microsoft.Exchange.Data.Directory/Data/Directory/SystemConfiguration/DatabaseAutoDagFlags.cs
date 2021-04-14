using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum DatabaseAutoDagFlags
	{
		None = 0,
		ExcludeFromMonitoring = 1,
		ExcludeFromDatabaseCopyLocationAgility = 2
	}
}
