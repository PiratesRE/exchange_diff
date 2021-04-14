using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum TempTableGrbit
	{
		None = 0,
		Indexed = 1,
		Unique = 2,
		Updatable = 4,
		Scrollable = 8,
		SortNullsHigh = 16,
		ForceMaterialization = 32,
		ErrorOnDuplicateInsertion = 32
	}
}
