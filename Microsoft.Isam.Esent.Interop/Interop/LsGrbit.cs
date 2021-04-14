using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum LsGrbit
	{
		None = 0,
		Reset = 1,
		Cursor = 2,
		Table = 4
	}
}
