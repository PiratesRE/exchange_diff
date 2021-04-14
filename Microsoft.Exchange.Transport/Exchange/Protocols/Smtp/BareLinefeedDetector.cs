using System;
using System.IO;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class BareLinefeedDetector : Stream, IDisposeTrackable, IDisposable
	{
		public BareLinefeedDetector()
		{
			this.disposeTracker = this.GetDisposeTracker();
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
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanTimeout
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

		public override int ReadTimeout
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

		public override int WriteTimeout
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

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			throw new NotSupportedException();
		}

		public override int ReadByte()
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = offset;
			int num2;
			while ((num2 = MimeInternalHelpers.IndexOf(buffer, 10, num, size - (num - offset))) != -1)
			{
				if (num2 == 0)
				{
					if (!this.lastByteWasCarriageReturn)
					{
						throw new BareLinefeedException(num2 - offset + this.bytesExamined);
					}
				}
				else if (buffer[num2 - 1] != 13)
				{
					throw new BareLinefeedException(num2 - offset + this.bytesExamined);
				}
				num = num2 + 1;
				if (num == buffer.Length)
				{
					break;
				}
			}
			if (buffer.Length == 1)
			{
				this.lastByteWasCarriageReturn = (buffer[0] == 13);
			}
			else if (buffer.Length > 1)
			{
				this.lastByteWasCarriageReturn = (buffer[buffer.Length - 1] == 13);
			}
			this.bytesExamined += buffer.Length;
		}

		public override void WriteByte(byte value)
		{
			throw new NotSupportedException();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<BareLinefeedDetector>(this);
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

		private bool lastByteWasCarriageReturn;

		private int bytesExamined;

		private DisposeTracker disposeTracker;
	}
}
