using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum BackupGrbit
	{
		None = 0,
		Incremental = 1,
		Atomic = 4
	}
}
