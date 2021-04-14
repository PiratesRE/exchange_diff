using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ImportCalendarStream : MemoryStream
	{
		public ImportCalendarStream()
		{
			this.Capacity = 16384;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("Write");
			if (this.isContentCopied)
			{
				throw new NotSupportedException("Can not write after content copied to this stream.");
			}
			int num = offset;
			while (num + offset < count)
			{
				this.WriteByte(buffer[num]);
				num++;
			}
		}

		public override void WriteByte(byte value)
		{
			this.CheckDisposed("WriteByte");
			if (this.isContentCopied)
			{
				throw new NotSupportedException("Can not write after content copied to this stream.");
			}
			if (this.lastByte != 13 && value == 10)
			{
				base.WriteByte(13);
			}
			base.WriteByte(value);
			this.lastByte = new byte?(value);
		}

		public int CopyFrom(Stream stream)
		{
			this.CheckDisposed("CopyFrom");
			this.SetLength(0L);
			if (stream.CanSeek)
			{
				this.Capacity = (int)stream.Length;
				if (this.Capacity > StorageLimits.Instance.CalendarMaxNumberBytesForICalImport)
				{
					return -1;
				}
			}
			byte[] buffer = new byte[4096];
			int i = 0;
			while (i <= StorageLimits.Instance.CalendarMaxNumberBytesForICalImport)
			{
				int num = stream.Read(buffer, 0, 4096);
				this.Write(buffer, 0, num);
				i += num;
				if (num <= 0)
				{
					this.Seek(0L, SeekOrigin.Begin);
					this.isContentCopied = true;
					return i;
				}
			}
			return -1;
		}

		public override void Close()
		{
			this.isClosed = true;
			base.Close();
			GC.SuppressFinalize(this);
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isClosed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private const byte LineFeed = 10;

		private const byte CarriageReturn = 13;

		private const int CopyBufferSize = 4096;

		private const int DefaultCapacity = 16384;

		private byte? lastByte = null;

		private bool isContentCopied;

		private bool isClosed;
	}
}
