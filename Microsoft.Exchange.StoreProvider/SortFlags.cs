using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum SortFlags
	{
		Ascend = 0,
		Descend = 1,
		CategoryMax = 4,
		CategoryMin = 8
	}
}
