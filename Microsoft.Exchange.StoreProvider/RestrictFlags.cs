using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum RestrictFlags
	{
		None = 0,
		Async = 1,
		Batch = 2,
		Static = 4
	}
}
