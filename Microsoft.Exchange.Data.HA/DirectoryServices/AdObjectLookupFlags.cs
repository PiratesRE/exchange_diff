using System;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[Flags]
	internal enum AdObjectLookupFlags
	{
		None = 0,
		ReadThrough = 1
	}
}
