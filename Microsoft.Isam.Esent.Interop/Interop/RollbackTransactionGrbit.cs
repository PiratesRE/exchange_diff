using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum RollbackTransactionGrbit
	{
		None = 0,
		RollbackAll = 1
	}
}
