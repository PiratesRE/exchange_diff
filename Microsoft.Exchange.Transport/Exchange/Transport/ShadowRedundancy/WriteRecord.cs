using System;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal class WriteRecord
	{
		public byte[] WriteBuffer { get; private set; }

		public bool Eod { get; set; }

		public WriteRecord(byte[] buffer, int offset, int count, bool seenEod)
		{
			this.WriteBuffer = new byte[count];
			this.Eod = seenEod;
			Buffer.BlockCopy(buffer, offset, this.WriteBuffer, 0, count);
		}
	}
}
