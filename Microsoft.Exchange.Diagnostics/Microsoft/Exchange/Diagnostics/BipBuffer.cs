using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class BipBuffer
	{
		public BipBuffer(int bufferSize)
		{
			this.buffer = new byte[bufferSize];
			this.regionAHead = 0;
			this.regionATail = 0;
			this.regionBSize = 0;
		}

		public byte[] Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		public int AllocatedSize
		{
			get
			{
				return this.regionATail - this.regionAHead + this.regionBSize;
			}
		}

		public int Allocate(int requestedSize)
		{
			int result = -1;
			if (this.regionBSize == 0 && this.buffer.Length - this.regionATail >= requestedSize)
			{
				result = this.regionATail;
				this.regionATail += requestedSize;
			}
			else if (this.regionAHead - this.regionBSize >= requestedSize)
			{
				result = this.regionBSize;
				this.regionBSize += requestedSize;
			}
			return result;
		}

		public void Release(int releasedSize)
		{
			while (releasedSize > 0)
			{
				int num = Math.Min(this.regionATail - this.regionAHead, releasedSize);
				this.regionAHead += num;
				releasedSize -= num;
				if (this.regionAHead == this.regionATail)
				{
					this.regionAHead = 0;
					this.regionATail = this.regionBSize;
					this.regionBSize = 0;
				}
			}
		}

		public void Extract(byte[] destinationBuffer, int destinationOffset, int extractedSize)
		{
			int num = 0;
			if (extractedSize > 0)
			{
				int num2 = Math.Min(this.regionATail - this.regionAHead, extractedSize);
				Array.Copy(this.buffer, this.regionAHead, destinationBuffer, destinationOffset, num2);
				destinationOffset += num2;
				num += num2;
				extractedSize -= num2;
			}
			if (extractedSize > 0)
			{
				Array.Copy(this.buffer, 0, destinationBuffer, destinationOffset, extractedSize);
				num += extractedSize;
				extractedSize = 0;
			}
		}

		public void Extract(int skipBytes, out byte destinationBuffer)
		{
			this.Release(skipBytes);
			destinationBuffer = this.buffer[this.regionAHead];
			this.Release(1);
		}

		private byte[] buffer;

		private int regionAHead;

		private int regionATail;

		private int regionBSize;
	}
}
