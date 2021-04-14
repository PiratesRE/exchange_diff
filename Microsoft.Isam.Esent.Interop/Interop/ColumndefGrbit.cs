using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum ColumndefGrbit
	{
		None = 0,
		ColumnFixed = 1,
		ColumnTagged = 2,
		ColumnNotNULL = 4,
		ColumnVersion = 8,
		ColumnAutoincrement = 16,
		ColumnUpdatable = 32,
		ColumnMultiValued = 1024,
		ColumnEscrowUpdate = 2048,
		ColumnUnversioned = 4096,
		ColumnMaybeNull = 8192,
		ColumnFinalize = 16384,
		ColumnUserDefinedDefault = 32768,
		TTKey = 64,
		TTDescending = 128
	}
}
