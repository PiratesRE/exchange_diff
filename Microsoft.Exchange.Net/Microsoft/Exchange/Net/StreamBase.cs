using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Net
{
	internal abstract class StreamBase : Stream, IDisposeTrackable, IDisposable
	{
		protected StreamBase(StreamBase.Capabilities capabilities)
		{
			EnumValidator.ThrowIfInvalid<StreamBase.Capabilities>(capabilities, "capabilities");
			this.capabilities = capabilities;
			this.disposeTracker = this.GetDisposeTracker();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
				this.isClosed = true;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override bool CanRead
		{
			get
			{
				this.CheckDisposed("CanRead:get");
				return (this.capabilities & StreamBase.Capabilities.Readable) == StreamBase.Capabilities.Readable;
			}
		}

		public override bool CanWrite
		{
			get
			{
				this.CheckDisposed("CanWrite:get");
				return (this.capabilities & StreamBase.Capabilities.Writable) == StreamBase.Capabilities.Writable;
			}
		}

		public override bool CanSeek
		{
			get
			{
				this.CheckDisposed("CanSeek:get");
				return (this.capabilities & StreamBase.Capabilities.Seekable) == StreamBase.Capabilities.Seekable;
			}
		}

		public override long Length
		{
			get
			{
				this.CheckDisposed("Length:Get");
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				this.CheckDisposed("Position:get");
				throw new NotSupportedException();
			}
			set
			{
				this.CheckDisposed("Position:set");
				throw new NotSupportedException();
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("Write");
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("Read");
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			this.CheckDisposed("SetLength");
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed("Seek");
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			this.CheckDisposed("Flush");
			throw new NotSupportedException();
		}

		public abstract DisposeTracker GetDisposeTracker();

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isClosed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		protected bool IsClosed
		{
			get
			{
				return this.isClosed;
			}
		}

		private StreamBase.Capabilities capabilities;

		private bool isClosed;

		private readonly DisposeTracker disposeTracker;

		[Flags]
		internal enum Capabilities
		{
			None = 0,
			Readable = 1,
			Writable = 2,
			Seekable = 4
		}
	}
}
