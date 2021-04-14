using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInBdatStreamBuilder : SmtpInStreamBuilderBase
	{
		public SmtpInBdatStreamBuilder()
		{
			base.EohPos = -1L;
		}

		public long ChunkSize { get; set; }

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
			this.ChunkSize = 0L;
		}

		public override bool Write(byte[] data, int offset, int numBytes, out int numBytesConsumed)
		{
			if (numBytes < 0)
			{
				throw new LocalizedException(Strings.SmtpReceiveParserNegativeBytes);
			}
			long num = this.ChunkSize - base.TotalBytesRead;
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
			return base.TotalBytesRead >= this.ChunkSize;
		}
	}
}
