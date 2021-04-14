using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.AirSync
{
	internal class AirSyncStream : Stream, IDisposeTrackable, IDisposable
	{
		public AirSyncStream()
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.internalStream = Streams.CreateTemporaryStorageStream(new Func<int, byte[]>(AirSyncStream.AcquireBuffer), new Action<byte[]>(AirSyncStream.ReleaseBuffer));
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.RegisterDisposableData(this);
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
				long length = this.internalStream.Length;
				if (this.DoBase64Conversion)
				{
					long num = length / 3L * 4L;
					return num + ((length % 3L != 0L) ? 4L : 0L);
				}
				return length;
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

		public bool DoBase64Conversion { get; set; }

		public int StreamHash
		{
			get
			{
				if (this.streamHash == 0 && this.internalStream.Length > 0L)
				{
					this.streamHash = this.GetStreamHash();
				}
				return this.streamHash;
			}
			set
			{
				this.streamHash = value;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.streamHash = 0;
			this.internalStream.Write(buffer, offset, count);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(string.Format(CultureInfo.InstalledUICulture, "buffer is not long enough: buffer.Length={0}, offset={1}, count={2}", new object[]
				{
					buffer.Length,
					offset,
					count
				}));
			}
			if (this.DoBase64Conversion)
			{
				int byteCount = (count * 3 / 4 < 1) ? 1 : (count * 3 / 4);
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					return StreamHelper.CopyStreamWithBase64Conversion(this.internalStream, memoryStream, byteCount, true);
				}
			}
			return this.internalStream.Read(buffer, offset, count);
		}

		public void CopyStream(Stream outputStream, int count)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (this.DoBase64Conversion)
			{
				StreamHelper.CopyStreamWithBase64Conversion(this.internalStream, outputStream, count, true);
				return;
			}
			StreamHelper.CopyStream(this.internalStream, outputStream, count);
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

		public override string ToString()
		{
			long position = this.Position;
			this.Position = 0L;
			StreamReader streamReader = new StreamReader(this, Encoding.UTF8);
			string result = streamReader.ReadToEnd();
			this.Position = position;
			return result;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AirSyncStream>(this);
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
				if (this.internalStream != null)
				{
					this.internalStream.Dispose();
					this.internalStream = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
			}
			base.Dispose(disposing);
		}

		private static byte[] AcquireBuffer(int bufferSize)
		{
			if (AirSyncStream.streamBufferPool == null)
			{
				lock (AirSyncStream.streamBufferPoolLock)
				{
					if (AirSyncStream.streamBufferPool == null)
					{
						AirSyncStream.streamBufferPool = new BufferPool(bufferSize, AirSyncStream.defaultMaxBufferCountPerProcessor);
					}
				}
			}
			if (bufferSize != AirSyncStream.streamBufferPool.BufferSize)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "bufferSize '{0}' is different from streamBufferPool.BufferSize '{1}'", new object[]
				{
					bufferSize,
					AirSyncStream.streamBufferPool.BufferSize
				}));
			}
			return AirSyncStream.streamBufferPool.Acquire();
		}

		private static void ReleaseBuffer(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			AirSyncStream.streamBufferPool.Release(buffer);
		}

		private int GetStreamHash()
		{
			int num = 0;
			uint num2 = 0U;
			long position = this.internalStream.Position;
			this.internalStream.Position = 0L;
			bool flag = false;
			byte[] array;
			if (AirSyncStream.streamBufferPool != null)
			{
				array = AirSyncStream.streamBufferPool.Acquire();
				flag = true;
				goto IL_46;
			}
			array = new byte[8192];
			try
			{
				do
				{
					IL_46:
					int num3 = this.internalStream.Read(array, 0, array.Length);
					if (num3 == 0)
					{
						break;
					}
					num2 = StreamHelper.UpdateCrc32(num2, array, 0, num3);
					num += num3;
				}
				while ((long)num < this.Length);
				this.internalStream.Position = position;
			}
			finally
			{
				if (flag && array != null)
				{
					AirSyncStream.streamBufferPool.Release(array);
				}
			}
			return (int)num2;
		}

		private static int defaultMaxBufferCountPerProcessor = GlobalSettings.MaxWorkerThreadsPerProc * 16;

		private static object streamBufferPoolLock = new object();

		private static volatile BufferPool streamBufferPool;

		private Stream internalStream;

		private DisposeTracker disposeTracker;

		private int streamHash;
	}
}
