using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class EncodedStream : Stream, IDisposeTrackable, IDisposable
	{
		public EncodedStream(Stream unicodeStream, Encoding encoding, IResourceTracker resourceTracker)
		{
			Util.ThrowOnNullArgument(unicodeStream, "unicodeStream");
			Util.ThrowOnNullArgument(encoding, "encoding");
			Util.ThrowOnNullArgument(resourceTracker, "resourceTracker");
			this.unicodeStream = unicodeStream;
			this.encoding = encoding;
			this.resourceTracker = resourceTracker;
			this.disposeTracker = this.GetDisposeTracker();
			using (DisposeGuard disposeGuard = this.Guard())
			{
				byte[] array = new byte[this.unicodeStream.Length];
				unicodeStream.Read(array, 0, array.Length);
				char[] chars = Encoding.Unicode.GetChars(array);
				int byteCount = encoding.GetByteCount(chars);
				this.VerifyCanChangeStreamSize(byteCount);
				byte[] bytes = encoding.GetBytes(chars);
				this.conversionStream = new MemoryStream(bytes.Length);
				this.conversionStream.Write(bytes, 0, bytes.Length);
				this.conversionStream.Seek(0L, SeekOrigin.Begin);
				disposeGuard.Success();
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.unicodeStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.unicodeStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.unicodeStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return this.conversionStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.conversionStream.Position;
			}
			set
			{
				this.conversionStream.Position = value;
			}
		}

		public override void Flush()
		{
			char[] encodedCharacters = EncodedStream.GetEncodedCharacters(this.conversionStream, this.encoding);
			byte[] bytes = Encoding.Unicode.GetBytes(encodedCharacters);
			this.unicodeStream.Position = 0L;
			this.unicodeStream.Write(bytes, 0, bytes.Length);
			this.unicodeStream.SetLength((long)bytes.Length);
			this.unicodeStream.Flush();
			this.conversionStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.conversionStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.conversionStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			if (value > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("value", string.Format("Attempted to set the stream size to {0}.", value));
			}
			this.VerifyCanChangeStreamSize((int)value);
			this.conversionStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			long num = this.conversionStream.Position + (long)count;
			if (num > (long)this.reservedMemory)
			{
				this.VerifyCanChangeStreamSize((int)num);
			}
			this.conversionStream.Write(buffer, offset, count);
		}

		protected override void Dispose(bool isDisposing)
		{
			base.Dispose(isDisposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				if (isDisposing)
				{
					this.resourceTracker.TryReserveMemory(-this.reservedMemory);
					Util.DisposeIfPresent(this.conversionStream);
					Util.DisposeIfPresent(this.unicodeStream);
					Util.DisposeIfPresent(this.disposeTracker);
				}
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EncodedStream>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private void VerifyCanChangeStreamSize(int newStreamSize)
		{
			if (newStreamSize < 0)
			{
				throw new ArgumentOutOfRangeException("newStreamSize", string.Format("Stream size cannot be negative. Requested size = {0}.", newStreamSize));
			}
			if (newStreamSize > this.resourceTracker.StreamSizeLimit)
			{
				throw new RopExecutionException(string.Format("Attempted to allocate a stream that is larger than the allowed size. StreamSizeLimit = {0}. Requested stream size = {1}.", this.resourceTracker.StreamSizeLimit, newStreamSize), ErrorCode.MaxSubmissionExceeded);
			}
			if (!this.resourceTracker.TryReserveMemory(newStreamSize - this.reservedMemory))
			{
				throw new RopExecutionException(string.Format("Attempted to use more memory than allowed. Reserve memory request = {0}.", newStreamSize - this.reservedMemory), ErrorCode.MaxSubmissionExceeded);
			}
			this.reservedMemory = newStreamSize;
		}

		private static char[] GetEncodedCharacters(MemoryStream conversionStream, Encoding encoding)
		{
			return encoding.GetChars(conversionStream.GetBuffer(), 0, (int)conversionStream.Length);
		}

		private readonly Stream unicodeStream;

		private readonly MemoryStream conversionStream;

		private readonly Encoding encoding;

		private readonly DisposeTracker disposeTracker;

		private readonly IResourceTracker resourceTracker;

		private bool isDisposed;

		private int reservedMemory;
	}
}
