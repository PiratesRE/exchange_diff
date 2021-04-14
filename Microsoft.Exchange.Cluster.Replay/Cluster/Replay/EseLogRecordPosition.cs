using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class EseLogRecordPosition
	{
		public EseLogPos LogPos { get; set; }

		public int LogRecordLength { get; set; }

		public int LogSectorSize { get; set; }

		public int ByteOffsetToStartOfRec
		{
			get
			{
				if (this.LogPos == null)
				{
					return 0;
				}
				return this.LogPos.ToBytePos(this.LogSectorSize);
			}
		}

		public override string ToString()
		{
			return string.Format("LgPos=0x{0},RecLen=0x{1:X},SectorSize=0x{2:X}", this.LogPos, this.LogRecordLength, this.LogSectorSize);
		}
	}
}
