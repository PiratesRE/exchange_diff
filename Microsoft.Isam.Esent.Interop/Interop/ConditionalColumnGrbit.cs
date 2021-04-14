using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum ConditionalColumnGrbit
	{
		ColumnMustBeNull = 1,
		ColumnMustBeNonNull = 2
	}
}
