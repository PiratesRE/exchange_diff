using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum IdleGrbit
	{
		None = 0,
		FlushBuffers = 1,
		Compact = 2,
		GetStatus = 4
	}
}
