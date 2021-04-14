using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class EndOfLogInformation
	{
		public bool E00Exists { get; set; }

		public long Generation { get; set; }

		public int ByteOffset { get; set; }

		public DateTime LastWriteUtc { get; set; }

		public EseLogRecordPosition LastLogRecPos { get; set; }
	}
}
