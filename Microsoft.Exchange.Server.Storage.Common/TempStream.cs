using System;
using System.IO;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class TempStream : Stream, IDisposeTrackable, IDisposable
	{
		private TempStream(Stream stream)
		{
			this.stream = stream;
			this.disposeTracker = this.GetDisposeTracker();
		}

		private Stream WrappedStream
		{
			get
			{
				return this.stream;
			}
		}

		public static BufferPool Pool
		{
			get
			{
				return TempStream.virtualStreamBlockPool;
			}
		}

		public static void Configure(string tempPath)
		{
			TempStream.virtualStreamBlockPool = new BufferPool(8192, 32, ConfigurationSchema.CleanupTempStreamBuffersOnRelease.Value, true);
			Streams.ConfigureTempStorage(131072, 8192, TempStream.GetTempPath(tempPath), new Func<int, byte[]>(TempStream.AcquireBuffer), new Action<byte[]>(TempStream.ReleaseBuffer));
		}

		public static Stream CreateInstance()
		{
			Stream stream = Streams.CreateTemporaryStorageStream();
			return new TempStream(stream);
		}

		public static Stream CloneStream(Stream originalStream)
		{
			TempStream tempStream = originalStream as TempStream;
			if (tempStream == null)
			{
				return Streams.CloneTemporaryStorageStream(originalStream);
			}
			Stream stream = Streams.CloneTemporaryStorageStream(tempStream.WrappedStream);
			return new TempStream(stream);
		}

		public static int CopyStream(Stream source, Stream destination)
		{
			if (source == null)
			{
				return 0;
			}
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(BufferPoolCollection.BufferSize.Size32K);
			byte[] array = bufferPool.Acquire();
			int result;
			try
			{
				int num = 0;
				int num2;
				do
				{
					num2 = source.Read(array, 0, array.Length);
					destination.Write(array, 0, num2);
					num += num2;
				}
				while (num2 > 0);
				result = num;
			}
			finally
			{
				bufferPool.Release(array);
			}
			return result;
		}

		public override bool CanRead
		{
			get
			{
				return this.stream != null && this.stream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.stream != null && this.stream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.stream != null && this.stream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				return this.stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.stream.Position;
			}
			set
			{
				this.stream.Position = value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.stream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.stream.Write(buffer, offset, count);
		}

		public override void Flush()
		{
			this.stream.Flush();
		}

		public override void SetLength(long value)
		{
			this.stream.SetLength(value);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.stream.Seek(offset, origin);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.stream != null)
				{
					this.stream.Dispose();
					this.stream = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<TempStream>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private static string GetTempPath(string tempPath)
		{
			if (!string.IsNullOrEmpty(tempPath))
			{
				return tempPath;
			}
			return Path.GetTempPath();
		}

		private static byte[] AcquireBuffer(int size)
		{
			return TempStream.virtualStreamBlockPool.Acquire();
		}

		private static void ReleaseBuffer(byte[] buffer)
		{
			TempStream.virtualStreamBlockPool.Release(buffer);
		}

		private const int BlockSize = 8192;

		private const int MaximumInMemorySize = 131072;

		private const int MaxPoolBufferCountPerProcessor = 32;

		private static BufferPool virtualStreamBlockPool;

		private Stream stream;

		private DisposeTracker disposeTracker;
	}
}
