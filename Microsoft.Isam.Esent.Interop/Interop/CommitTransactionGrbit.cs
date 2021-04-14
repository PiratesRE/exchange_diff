using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum CommitTransactionGrbit
	{
		None = 0,
		LazyFlush = 1,
		WaitLastLevel0Commit = 2
	}
}
