using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum SpaceHintsGrbit
	{
		None = 0,
		SpaceHintUtilizeParentSpace = 1,
		CreateHintAppendSequential = 2,
		CreateHintHotpointSequential = 4,
		RetrieveHintReserve1 = 8,
		RetrieveHintTableScanForward = 16,
		RetrieveHintTableScanBackward = 32,
		RetrieveHintReserve2 = 64,
		RetrieveHintReserve3 = 128,
		DeleteHintTableSequential = 256
	}
}
