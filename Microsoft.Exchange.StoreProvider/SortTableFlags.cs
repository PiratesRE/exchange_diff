using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum SortTableFlags
	{
		None = 0,
		Async = 1,
		Batch = 2
	}
}
