using System;
using System.IO;
using System.Threading;

namespace Microsoft.Exchange.Data.Internal
{
	internal abstract class DataStorage : RefCountable
	{
		internal bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		public static long CopyStreamToStream(Stream srcStream, Stream destStream, long lengthToCopy, ref byte[] scratchBuffer)
		{
			if (scratchBuffer == null || scratchBuffer.Length < 16384)
			{
				scratchBuffer = new byte[16384];
			}
			long num = 0L;
			while (lengthToCopy != 0L)
			{
				int count = (int)Math.Min(lengthToCopy, (long)scratchBuffer.Length);
				int num2 = srcStream.Read(scratchBuffer, 0, count);
				if (num2 == 0)
				{
					break;
				}
				if (destStream != null)
				{
					destStream.Write(scratchBuffer, 0, num2);
				}
				num += (long)num2;
				if (lengthToCopy != 9223372036854775807L)
				{
					lengthToCopy -= (long)num2;
				}
			}
			return num;
		}

		public static Stream NewEmptyReadStream()
		{
			return new StreamOnReadableDataStorage(null, 0L, 0L);
		}

		public abstract Stream OpenReadStream(long start, long end);

		public virtual long CopyContentToStream(long start, long end, Stream destStream, ref byte[] scratchBuffer)
		{
			base.ThrowIfDisposed();
			if (destStream == null && end != 9223372036854775807L)
			{
				return end - start;
			}
			long result;
			using (Stream stream = this.OpenReadStream(start, end))
			{
				result = DataStorage.CopyStreamToStream(stream, destStream, long.MaxValue, ref scratchBuffer);
			}
			return result;
		}

		internal virtual void SetReadOnly(bool makeReadOnly)
		{
			base.ThrowIfDisposed();
			if (makeReadOnly == this.isReadOnly)
			{
				return;
			}
			if (makeReadOnly)
			{
				this.readOnlySemaphore = new SemaphoreSlim(1);
			}
			else
			{
				this.readOnlySemaphore = null;
			}
			this.isReadOnly = makeReadOnly;
		}

		protected bool isReadOnly;

		protected SemaphoreSlim readOnlySemaphore;
	}
}
