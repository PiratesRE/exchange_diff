using System;
using System.IO;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class ForkToTempStorageReadStream : Stream
	{
		public ForkToTempStorageReadStream(Stream sourceStream)
		{
			this.sourceStream = sourceStream;
			this.storage = new TemporaryDataStorage();
			this.forkStream = this.storage.OpenWriteStream(true);
		}

		public DataStorage Storage
		{
			get
			{
				return this.storage;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.sourceStream != null;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = this.sourceStream.Read(buffer, offset, count);
			this.forkStream.Write(buffer, offset, num);
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.sourceStream != null)
			{
				this.sourceStream.Dispose();
				this.sourceStream = null;
				this.forkStream.Dispose();
				this.forkStream = null;
				this.storage.Release();
				this.storage = null;
			}
			base.Dispose(disposing);
		}

		private Stream sourceStream;

		private Stream forkStream;

		private TemporaryDataStorage storage;
	}
}
