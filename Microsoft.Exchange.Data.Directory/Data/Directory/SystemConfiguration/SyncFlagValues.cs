using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum SyncFlagValues
	{
		None = 0,
		Mirrored = 1,
		SyncNow = 2,
		Calendar = 4
	}
}
