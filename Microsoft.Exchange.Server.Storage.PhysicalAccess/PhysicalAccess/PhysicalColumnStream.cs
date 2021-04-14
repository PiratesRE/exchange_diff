using System;
using System.IO;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class PhysicalColumnStream : Stream
	{
		public PhysicalColumnStream(IColumnStreamAccess columnStreamAccess, PhysicalColumn column, bool readOnly)
		{
			this.columnStreamAccess = columnStreamAccess;
			this.column = column;
			this.valid = true;
			this.length = null;
			this.readOnly = readOnly;
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !this.readOnly;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override long Length
		{
			get
			{
				if (this.length == null)
				{
					this.length = new long?((long)this.columnStreamAccess.GetColumnSize(this.column));
				}
				return this.length.Value;
			}
		}

		public override long Position
		{
			get
			{
				return this.position;
			}
			set
			{
				if (value >= 0L)
				{
					this.position = value;
					return;
				}
				throw new ArgumentOutOfRangeException("Position", value, "Position must be greater than zero");
			}
		}

		internal bool IsValid
		{
			get
			{
				return this.valid;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.AssertValid();
			int num = this.columnStreamAccess.ReadStream(this.column, this.position, buffer, offset, count);
			this.position += (long)num;
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.AssertValid();
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.Position = offset;
				break;
			case SeekOrigin.Current:
				this.Position += offset;
				break;
			case SeekOrigin.End:
				this.Position = this.Length + offset;
				break;
			}
			return this.Position;
		}

		public override void SetLength(long len)
		{
			this.AssertValid();
			throw new NotSupportedException("SetLength is not supported");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.AssertValid();
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Write is not supported");
			}
			this.columnStreamAccess.WriteStream(this.column, this.position, buffer, offset, count);
			this.position += (long)count;
			this.length = null;
		}

		public override void Flush()
		{
			this.AssertValid();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.valid)
			{
				this.valid = false;
			}
		}

		private void AssertValid()
		{
			if (!this.valid)
			{
				throw new ObjectDisposedException("This stream is closed");
			}
		}

		private readonly bool readOnly;

		private readonly PhysicalColumn column;

		private IColumnStreamAccess columnStreamAccess;

		private bool valid;

		private long position;

		private long? length;
	}
}
