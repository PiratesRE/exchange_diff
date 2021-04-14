using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum AttachDatabaseGrbit
	{
		None = 0,
		ReadOnly = 1,
		DeleteCorruptIndexes = 16
	}
}
