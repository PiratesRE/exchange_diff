using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum AdjacencyOrConflictType
	{
		Precedes = 1,
		Follows = 2,
		Conflicts = 4
	}
}
