using System;

namespace Microsoft.Isam.Esent.Interop.Windows7
{
	public static class Windows7Param
	{
		public const JET_param WaypointLatency = (JET_param)153;

		public const JET_param LVChunkSizeMost = (JET_param)163;

		public const JET_param MaxCoalesceReadSize = (JET_param)164;

		public const JET_param MaxCoalesceWriteSize = (JET_param)165;

		public const JET_param MaxCoalesceReadGapSize = (JET_param)166;

		public const JET_param MaxCoalesceWriteGapSize = (JET_param)167;

		public const JET_param EnableDbScanInRecovery = (JET_param)169;

		public const JET_param DbScanThrottle = (JET_param)170;

		public const JET_param DbScanIntervalMinSec = (JET_param)171;

		public const JET_param DbScanIntervalMaxSec = (JET_param)172;
	}
}
