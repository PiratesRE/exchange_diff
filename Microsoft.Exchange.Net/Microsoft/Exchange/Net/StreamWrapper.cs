using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Net
{
	internal class StreamWrapper : StreamBase
	{
		internal static StreamBase.Capabilities GetStreamCapabilities(Stream stream)
		{
			if (stream == null)
			{
				return StreamBase.Capabilities.None;
			}
			return (stream.CanRead ? StreamBase.Capabilities.Readable : StreamBase.Capabilities.None) | (stream.CanWrite ? StreamBase.Capabilities.Writable : StreamBase.Capabilities.None) | (stream.CanSeek ? StreamBase.Capabilities.Seekable : StreamBase.Capabilities.None);
		}

		public StreamWrapper(Stream wrappedStream) : base(StreamWrapper.GetStreamCapabilities(wrappedStream))
		{
			this.internalStream = wrappedStream;
			this.canDisposeInternalStream = true;
		}

		public StreamWrapper(Stream wrappedStream, bool canDispose) : base(StreamWrapper.GetStreamCapabilities(wrappedStream))
		{
			this.internalStream = wrappedStream;
			this.canDisposeInternalStream = canDispose;
		}

		public StreamWrapper(Stream wrappedStream, bool canDispose, StreamBase.Capabilities capabilities) : base(capabilities)
		{
			this.internalStream = wrappedStream;
			this.canDisposeInternalStream = canDispose;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && !base.IsClosed && this.canDisposeInternalStream && this.internalStream != null)
				{
					this.internalStream.Dispose();
					this.internalStream = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StreamWrapper>(this);
		}

		public Stream InternalStream
		{
			get
			{
				base.CheckDisposed("InternalStream:get");
				return this.internalStream;
			}
		}

		public override bool CanRead
		{
			get
			{
				base.CheckDisposed("CanRead");
				return base.CanRead && (this.internalStream == null || this.internalStream.CanRead);
			}
		}

		public override bool CanWrite
		{
			get
			{
				base.CheckDisposed("CanWrite");
				return base.CanWrite && (this.internalStream == null || this.internalStream.CanWrite);
			}
		}

		public override bool CanSeek
		{
			get
			{
				base.CheckDisposed("CanSeek");
				return base.CanSeek && (this.internalStream == null || this.internalStream.CanSeek);
			}
		}

		public override long Length
		{
			get
			{
				base.CheckDisposed("Length:get");
				if (this.internalStream == null)
				{
					throw new NotSupportedException();
				}
				return this.internalStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				base.CheckDisposed("Position:get");
				if (!base.CanSeek || this.internalStream == null)
				{
					throw new NotSupportedException();
				}
				return this.internalStream.Position;
			}
			set
			{
				base.CheckDisposed("Position:set");
				if (!base.CanSeek || this.internalStream == null)
				{
					throw new NotSupportedException();
				}
				this.internalStream.Position = value;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			base.CheckDisposed("Write");
			if (!base.CanWrite || this.internalStream == null)
			{
				throw new NotSupportedException();
			}
			this.internalStream.Write(buffer, offset, count);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			base.CheckDisposed("Read");
			if (!base.CanRead || this.internalStream == null)
			{
				throw new NotSupportedException();
			}
			return this.internalStream.Read(buffer, offset, count);
		}

		public override void SetLength(long value)
		{
			base.CheckDisposed("SetLength");
			if (!base.CanWrite || !base.CanSeek || this.internalStream == null)
			{
				throw new NotSupportedException();
			}
			this.internalStream.SetLength(value);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			base.CheckDisposed("Seek");
			if (!base.CanSeek || this.internalStream == null)
			{
				throw new NotSupportedException();
			}
			EnumValidator.ThrowIfInvalid<SeekOrigin>(origin, "origin");
			return this.internalStream.Seek(offset, origin);
		}

		public override void Flush()
		{
			base.CheckDisposed("Flush");
			if (this.internalStream == null)
			{
				throw new NotSupportedException();
			}
			this.internalStream.Flush();
		}

		private readonly bool canDisposeInternalStream;

		private Stream internalStream;
	}
}
