using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum SnapshotPrepareGrbit
	{
		None = 0,
		IncrementalSnapshot = 1,
		CopySnapshot = 2
	}
}
