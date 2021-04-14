using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum OpenEntryFlags
	{
		None = 0,
		BestAccess = 16,
		DeferredErrors = 8,
		Modify = 1,
		ShowSoftDeletes = 2,
		DontThrowIfEntryIsMissing = 134217728
	}
}
