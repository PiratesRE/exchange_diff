using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	[Flags]
	public enum JET_LogInfoFlag
	{
		None = 0,
		ReservedLog = 1,
		CircularLoggingCurrent = 2,
		CircularLoggingHistory = 4
	}
}
