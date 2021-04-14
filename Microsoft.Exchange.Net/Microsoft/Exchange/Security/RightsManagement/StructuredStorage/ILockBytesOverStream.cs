using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Exchange.Security.RightsManagement.StructuredStorage
{
	internal class ILockBytesOverStream : ILockBytes
	{
		public ILockBytesOverStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanSeek)
			{
				throw new ArgumentException("The passed in stream must be seekable", "stream");
			}
			this.stream = stream;
		}

		public void ReadAt(ulong offset, byte[] buffer, int count, out int bytesRead)
		{
			if (buffer.Length < count)
			{
				throw new ArgumentException("Requesting more bytes from the stream than will fit in the supplied buffer", "count");
			}
			int i = count;
			bytesRead = 0;
			this.stream.Seek((long)offset, SeekOrigin.Begin);
			while (i > 0)
			{
				int num = this.stream.Read(buffer, bytesRead, i);
				if (num == 0)
				{
					return;
				}
				i -= num;
				bytesRead += num;
			}
		}

		public void WriteAt(ulong offset, byte[] buffer, int count, out int written)
		{
			this.stream.Seek((long)offset, SeekOrigin.Begin);
			this.stream.Write(buffer, 0, count);
			written = count;
		}

		public void Flush()
		{
			this.stream.Flush();
		}

		public void SetSize(ulong length)
		{
			this.stream.SetLength((long)length);
		}

		public void LockRegion(ulong libOffset, ulong cb, int dwLockType)
		{
		}

		public void UnlockRegion(ulong libOffset, ulong cb, int dwLockType)
		{
		}

		public void Stat(out STATSTG pstatstg, STATFLAG grfStatFlag)
		{
			pstatstg = default(STATSTG);
			pstatstg.type = 2;
			pstatstg.cbSize = this.stream.Length;
			pstatstg.grfLocksSupported = 2;
		}

		private Stream stream;
	}
}
