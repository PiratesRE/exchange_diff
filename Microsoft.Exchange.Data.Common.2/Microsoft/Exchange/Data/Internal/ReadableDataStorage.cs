using System;
using System.IO;

namespace Microsoft.Exchange.Data.Internal
{
	internal abstract class ReadableDataStorage : DataStorage
	{
		public ReadableDataStorage()
		{
		}

		public abstract long Length { get; }

		public abstract int Read(long position, byte[] buffer, int offset, int count);

		public override Stream OpenReadStream(long start, long end)
		{
			base.ThrowIfDisposed();
			return new StreamOnReadableDataStorage(this, start, end);
		}

		public override long CopyContentToStream(long start, long end, Stream destStream, ref byte[] scratchBuffer)
		{
			base.ThrowIfDisposed();
			if (scratchBuffer == null || scratchBuffer.Length < 16384)
			{
				scratchBuffer = new byte[16384];
			}
			long num = 0L;
			long num2 = (end == long.MaxValue) ? long.MaxValue : (end - start);
			while (num2 != 0L)
			{
				int count = (int)Math.Min(num2, (long)scratchBuffer.Length);
				int num3 = this.Read(start, scratchBuffer, 0, count);
				if (num3 == 0)
				{
					break;
				}
				start += (long)num3;
				destStream.Write(scratchBuffer, 0, num3);
				num += (long)num3;
				if (num2 != 9223372036854775807L)
				{
					num2 -= (long)num3;
				}
			}
			return num;
		}
	}
}
