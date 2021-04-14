using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	internal class TnefReaderStreamWrapper : Stream
	{
		public TnefReaderStreamWrapper(TnefReader reader)
		{
			this.Reader = reader;
			this.Reader.Child = this;
		}

		public override bool CanRead
		{
			get
			{
				return this.Reader != null;
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
				throw new NotSupportedException(TnefStrings.StreamDoesNotSupportSeek);
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException(TnefStrings.StreamDoesNotSupportSeek);
			}
			set
			{
				throw new NotSupportedException(TnefStrings.StreamDoesNotSupportSeek);
			}
		}

		public override void Flush()
		{
			throw new NotSupportedException(TnefStrings.StreamDoesNotSupportWrite);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.Reader == null)
			{
				throw new ObjectDisposedException("TnefReaderStreamWrapper");
			}
			return this.Reader.ReadPropertyRawValue(buffer, offset, count, true);
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(TnefStrings.StreamDoesNotSupportSeek);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException(TnefStrings.StreamDoesNotSupportWrite);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(TnefStrings.StreamDoesNotSupportSeek);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.Reader != null)
			{
				this.Reader.Child = null;
			}
			this.Reader = null;
			base.Dispose(disposing);
		}

		internal TnefReader Reader;
	}
}
