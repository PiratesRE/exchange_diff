using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	[Flags]
	public enum CorruptGrbit : uint
	{
		CorruptDatabaseFile = 2147483648U,
		CorruptDatabasePageImage = 1073741824U,
		CorruptPageChksumRand = 1U,
		CorruptPageChksumSafe = 2U,
		CorruptPageSingleFld = 4U,
		CorruptPageRemoveNode = 8U,
		CorruptPageDbtimeDelta = 16U
	}
}
