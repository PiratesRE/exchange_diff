using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum SetIndexRangeGrbit
	{
		None = 0,
		RangeInclusive = 1,
		RangeUpperLimit = 2,
		RangeInstantDuration = 4,
		RangeRemove = 8
	}
}
