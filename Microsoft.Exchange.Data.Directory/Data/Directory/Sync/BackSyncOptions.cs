using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Flags]
	public enum BackSyncOptions
	{
		None = 0,
		IncludeLinks = 1
	}
}
