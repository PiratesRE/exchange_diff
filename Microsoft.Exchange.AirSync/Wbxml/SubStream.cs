using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync.Wbxml
{
	internal class SubStream : Stream, IDisposeTrackable, IDisposable
	{
		public SubStream(Stream stream, long startPosition, long length)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.stream = stream;
			this.startPosition = startPosition;
			this.length = length;
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.RegisterDisposableData(this);
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.stream.CanSeek;
			}
		}

		public override bool CanWrite
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
				return this.length;
			}
		}

		public override long Position
		{
			get
			{
				return this.stream.Position - this.startPosition;
			}
			set
			{
				if (value > this.length)
				{
					this.stream.Position = this.startPosition + this.length;
					return;
				}
				this.stream.Position = this.startPosition + value;
			}
		}

		public override void Flush()
		{
			this.stream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long num;
			if (origin == SeekOrigin.Begin)
			{
				num = this.startPosition + offset;
			}
			else if (origin == SeekOrigin.Current)
			{
				num = this.stream.Position + offset;
			}
			else
			{
				if (origin != SeekOrigin.End)
				{
					throw new ArgumentException();
				}
				num = this.length + offset;
			}
			if (num < this.startPosition)
			{
				num = this.startPosition;
			}
			if (num > this.startPosition + this.length)
			{
				num = this.startPosition + this.length;
			}
			return this.stream.Seek(num, SeekOrigin.Begin);
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException("The SubStream class doesn't allow changing the length");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.stream.Position + (long)count > this.startPosition + this.length)
			{
				count = (int)(this.startPosition + this.length - this.stream.Position);
			}
			if (count > 0)
			{
				return this.stream.Read(buffer, offset, count);
			}
			return 0;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException("The SubStream class doesn't support writing");
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SubStream>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
				this.disposeTracker = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			base.Dispose(disposing);
		}

		private Stream stream;

		private long startPosition;

		private long length;

		private DisposeTracker disposeTracker;
	}
}
