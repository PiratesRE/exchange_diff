using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PopImap.Core
{
	public class StreamWrapper : Stream, IDisposeTrackable, IDisposable
	{
		public StreamWrapper()
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.disposeTracker = this.GetDisposeTracker();
				this.internalStream = Streams.CreateTemporaryStorageStream(new Func<int, byte[]>(StreamWrapper.AcquireBuffer), new Action<byte[]>(StreamWrapper.ReleaseBuffer));
				disposeGuard.Success();
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.internalStream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.internalStream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.internalStream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				return this.internalStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.internalStream.Position;
			}
			set
			{
				this.internalStream.Position = value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.internalStream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.internalStream.Write(buffer, offset, count);
		}

		public override void SetLength(long value)
		{
			this.internalStream.SetLength(value);
		}

		public override void Flush()
		{
			this.internalStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.internalStream.Seek(offset, origin);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StreamWrapper>(this);
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
			if (disposing)
			{
				try
				{
					if (this.internalStream != null)
					{
						this.internalStream.Dispose();
					}
				}
				finally
				{
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
				}
			}
			base.Dispose(disposing);
		}

		private static byte[] AcquireBuffer(int bufferSize)
		{
			if (StreamWrapper.streamBufferPool == null)
			{
				lock (StreamWrapper.streamBufferPoolLock)
				{
					if (StreamWrapper.streamBufferPool == null)
					{
						StreamWrapper.streamBufferPool = new BufferPool(bufferSize, StreamWrapper.GetBufferCountFromConfig());
					}
				}
			}
			if (bufferSize != StreamWrapper.streamBufferPool.BufferSize)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "bufferSize '{0}' is different from streamBufferPool.BufferSize '{1}'", new object[]
				{
					bufferSize,
					StreamWrapper.streamBufferPool.BufferSize
				}));
			}
			return StreamWrapper.streamBufferPool.Acquire();
		}

		private static void ReleaseBuffer(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			StreamWrapper.streamBufferPool.Release(buffer);
		}

		private static int GetBufferCountFromConfig()
		{
			int num;
			if (!int.TryParse(ConfigurationManager.AppSettings["MaxMimeBufferPoolPerCPU"], out num) || num <= 0)
			{
				num = 100;
			}
			return num;
		}

		private const int DefaultMaxBufferCountPerProcessor = 100;

		private static object streamBufferPoolLock = new object();

		private static BufferPool streamBufferPool;

		private Stream internalStream;

		private DisposeTracker disposeTracker;
	}
}
