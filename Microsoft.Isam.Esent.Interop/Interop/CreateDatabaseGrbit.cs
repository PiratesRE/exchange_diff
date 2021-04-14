using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum CreateDatabaseGrbit
	{
		None = 0,
		OverwriteExisting = 512,
		RecoveryOff = 8
	}
}
