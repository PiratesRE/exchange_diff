using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	internal class TnefWriterStreamWrapper : Stream
	{
		public TnefWriterStreamWrapper(TnefWriter writer)
		{
			this.Writer = writer;
			this.Writer.Child = this;
		}

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.Writer != null;
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
			if (this.Writer == null)
			{
				throw new ObjectDisposedException("TnefWriterStreamWrapper");
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException(TnefStrings.StreamDoesNotSupportRead);
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(TnefStrings.StreamDoesNotSupportSeek);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.Writer == null)
			{
				throw new ObjectDisposedException("TnefWriterStreamWrapper");
			}
			this.Writer.WritePropertyRawValueImpl(buffer, offset, count, true);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(TnefStrings.StreamDoesNotSupportSeek);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.Writer != null)
			{
				this.Writer.Child = null;
			}
			this.Writer = null;
			base.Dispose(disposing);
		}

		internal TnefWriter Writer;
	}
}
