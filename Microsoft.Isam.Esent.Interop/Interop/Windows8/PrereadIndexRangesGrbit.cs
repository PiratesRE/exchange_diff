using System;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	[Flags]
	public enum PrereadIndexRangesGrbit
	{
		Forward = 1,
		Backwards = 2,
		FirstPageOnly = 4,
		NormalizedKey = 8
	}
}
