using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	[Flags]
	public enum DatabaseScanGrbit
	{
		BatchStart = 16,
		BatchStop = 32,
		BatchStartContinuous = 128
	}
}
