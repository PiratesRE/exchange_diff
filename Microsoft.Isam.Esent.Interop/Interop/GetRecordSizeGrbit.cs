using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum GetRecordSizeGrbit
	{
		None = 0,
		InCopyBuffer = 1,
		RunningTotal = 2,
		Local = 4
	}
}
