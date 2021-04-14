using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum SortOrderFlags : byte
	{
		Ascending = 0,
		Descending = 1,
		Combine = 2,
		CategoryMaximum = 4,
		CategoryMinimum = 8
	}
}
