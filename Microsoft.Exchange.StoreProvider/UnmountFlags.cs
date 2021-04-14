using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum UnmountFlags
	{
		None = 0,
		ForceDatabaseDeletion = 8,
		SkipCacheFlush = 16
	}
}
