using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum SeekGrbit
	{
		SeekEQ = 1,
		SeekLT = 2,
		SeekLE = 4,
		SeekGE = 8,
		SeekGT = 16,
		SetIndexRange = 32
	}
}
