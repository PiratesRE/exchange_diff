using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum OpenPropertyFlags
	{
		None = 0,
		BestAccess = 16,
		Create = 2,
		Modify = 1,
		DeferredErrors = 8,
		ReadOnly = 16
	}
}
