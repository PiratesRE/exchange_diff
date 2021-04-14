using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	[ClassInterface(ClassInterfaceType.None)]
	internal class LockBytesOnStream : Interop.ILockBytes
	{
		public LockBytesOnStream(Stream stream)
		{
			Util.ThrowOnNullArgument(stream, "stream");
			if (!stream.CanSeek)
			{
				throw new ArgumentException(MsgStorageStrings.StreamNotSeakable("LockBytesOnStream::ctr"));
			}
			this.stream = stream;
			this.streamLock = new object();
			this.offset = 0L;
		}

		public void ReadAt(long offset, byte[] buffer, int bufferSize, out int totalBytesRead)
		{
			lock (this.streamLock)
			{
				if (this.offset != offset)
				{
					this.offset = this.stream.Seek(offset, SeekOrigin.Begin);
				}
				totalBytesRead = 0;
				int num = 0;
				int num2 = bufferSize;
				int num3;
				do
				{
					num3 = this.stream.Read(buffer, num, num2);
					totalBytesRead += num3;
					this.offset += (long)num3;
					num += num3;
					num2 -= num3;
				}
				while (totalBytesRead < bufferSize && num3 != 0);
			}
		}

		public void WriteAt(long offset, byte[] buffer, int bufferSize, out int bytesWritten)
		{
			lock (this.streamLock)
			{
				if (this.offset != offset)
				{
					this.offset = this.stream.Seek(offset, SeekOrigin.Begin);
				}
				this.stream.Write(buffer, 0, bufferSize);
				bytesWritten = bufferSize;
				this.offset += (long)bytesWritten;
			}
		}

		public void Flush()
		{
			lock (this.streamLock)
			{
				this.stream.Flush();
			}
		}

		public void SetSize(long newSize)
		{
			lock (this.streamLock)
			{
				this.stream.SetLength(newSize);
			}
		}

		public void LockRegion(long offset, long length, uint lockType)
		{
			throw new NotSupportedException();
		}

		public void UnlockRegion(long offset, long length, int lockType)
		{
			throw new NotSupportedException();
		}

		public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG statStg, uint statFlag)
		{
			System.Runtime.InteropServices.ComTypes.FILETIME filetime = default(System.Runtime.InteropServices.ComTypes.FILETIME);
			filetime.dwHighDateTime = 0;
			filetime.dwLowDateTime = 0;
			statStg = default(System.Runtime.InteropServices.ComTypes.STATSTG);
			statStg.atime = filetime;
			statStg.mtime = filetime;
			statStg.ctime = filetime;
			statStg.type = 2;
			statStg.cbSize = this.stream.Length;
			statStg.grfLocksSupported = 0;
			statStg.clsid = Guid.Empty;
			statStg.pwcsName = null;
		}

		private Stream stream;

		private object streamLock;

		private long offset;
	}
}
