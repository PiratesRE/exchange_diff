using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum DefragGrbit
	{
		AvailSpaceTreesOnly = 64,
		BatchStart = 1,
		BatchStop = 2
	}
}
