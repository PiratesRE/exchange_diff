using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class FullEndOfLog
	{
		public bool PositionInE00 { get; set; }

		public long Generation { get; set; }

		public int Sector { get; set; }

		public int ByteOffset { get; set; }

		public DateTime Utc { get; set; }

		public void CopyTo(FullEndOfLog target)
		{
			target.PositionInE00 = this.PositionInE00;
			target.Generation = this.Generation;
			target.Sector = this.Sector;
			target.ByteOffset = this.ByteOffset;
			target.Utc = this.Utc;
		}

		public override string ToString()
		{
			return string.Format("Gen=0x{0:X} Sector=0x{1:X} E00={2} UTC:{3:s}", new object[]
			{
				this.Generation,
				this.Sector,
				this.PositionInE00,
				this.Utc
			});
		}
	}
}
