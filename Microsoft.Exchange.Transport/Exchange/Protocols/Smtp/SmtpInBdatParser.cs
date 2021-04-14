using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInBdatParser : SmtpInParser
	{
		public long ChunkSize
		{
			get
			{
				return this.chunkSize;
			}
			set
			{
				this.chunkSize = value;
			}
		}

		public override bool IsEodSeen
		{
			get
			{
				return base.TotalBytesRead >= this.ChunkSize;
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.chunkSize = 0L;
		}

		public override bool Write(byte[] data, int offset, int numBytes, out int numBytesConsumed)
		{
			if (numBytes < 0)
			{
				throw new LocalizedException(Strings.SmtpReceiveParserNegativeBytes);
			}
			long num = this.chunkSize - base.TotalBytesRead;
			if (num <= 0L)
			{
				numBytesConsumed = 0;
				return true;
			}
			int num2 = (int)Math.Min(num, (long)numBytes);
			base.TotalBytesRead += (long)num2;
			if (!base.IsDiscardingData && num2 > 0)
			{
				base.Write(data, offset, num2);
			}
			numBytesConsumed = num2;
			return base.TotalBytesRead >= this.chunkSize;
		}

		private long chunkSize;
	}
}
