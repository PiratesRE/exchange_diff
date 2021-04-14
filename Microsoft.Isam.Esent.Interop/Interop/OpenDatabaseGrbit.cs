using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum OpenDatabaseGrbit
	{
		None = 0,
		ReadOnly = 1,
		Exclusive = 2
	}
}
