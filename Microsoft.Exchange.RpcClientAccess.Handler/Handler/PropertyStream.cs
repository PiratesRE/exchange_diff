using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PropertyStream : Stream
	{
		internal PropertyStream(Stream propertyStream, PropertyType propertyType, Logon logon, StreamSource streamSource) : base(logon)
		{
			Util.ThrowOnNullArgument(streamSource, "streamSource");
			this.propertyType = propertyType;
			this.streamSource = streamSource;
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (propertyStream == null)
				{
					throw new ArgumentNullException("propertyStream");
				}
				this.systemStream = propertyStream;
				disposeGuard.Success();
			}
		}

		public override void Commit()
		{
			base.CheckDisposed();
			this.systemStream.Flush();
		}

		public override uint GetSize()
		{
			base.CheckDisposed();
			return (uint)this.systemStream.Length;
		}

		public override ArraySegment<byte> Read(ushort requestedSize)
		{
			base.CheckDisposed();
			byte[] array = new byte[(int)requestedSize];
			int num = 0;
			int num2;
			do
			{
				num2 = this.systemStream.Read(array, num, (int)requestedSize - num);
				num += num2;
			}
			while (num2 > 0 && num < (int)requestedSize);
			return new ArraySegment<byte>(array, 0, num);
		}

		public override long Seek(StreamSeekOrigin streamSeekOrigin, long offset)
		{
			base.CheckDisposed();
			return this.systemStream.Seek(offset, PropertyStream.ToStreamSeekOrigin(streamSeekOrigin));
		}

		public override void SetSize(ulong size)
		{
			base.CheckDisposed();
			if (!this.systemStream.CanWrite)
			{
				throw new RopExecutionException("Cannot change the size of a readonly stream", (ErrorCode)2147680261U);
			}
			if (size > 2147483647UL)
			{
				throw new RopExecutionException(string.Format("Requested size for stream is too big: {0}.", size), (ErrorCode)2147746565U);
			}
			this.systemStream.SetLength((long)size);
		}

		public override ushort Write(ArraySegment<byte> data)
		{
			base.CheckDisposed();
			this.CheckCanWrite();
			if (data.Count > 65535)
			{
				throw new RopExecutionException(string.Format("Requested amount of data to write to stream is too big: {0}.", data.Count), (ErrorCode)2147746565U);
			}
			if (this.systemStream.Position + (long)data.Count > 2147483647L)
			{
				throw new RopExecutionException("Size of the stream after writing the requested data is too big.", (ErrorCode)2147746565U);
			}
			this.systemStream.Write(data.Array, data.Offset, data.Count);
			if (this.propertyType == PropertyType.Binary)
			{
				this.systemStream.Flush();
				this.streamChanged = false;
			}
			else
			{
				this.streamChanged = true;
			}
			return (ushort)data.Count;
		}

		public override ulong CopyToStream(Stream destinationStream, ulong bytesToCopy)
		{
			base.CheckDisposed();
			destinationStream.CheckCanWrite();
			ulong num = 0UL;
			while (bytesToCopy > num)
			{
				ushort requestedSize = (ushort)Math.Min(bytesToCopy - num, 65535UL);
				ArraySegment<byte> data = this.Read(requestedSize);
				if (data.Count == 0)
				{
					break;
				}
				if (this != destinationStream)
				{
					destinationStream.Write(data);
				}
				num += (ulong)((long)data.Count);
			}
			return num;
		}

		public override void CheckCanWrite()
		{
			base.CheckDisposed();
			if (!this.systemStream.CanWrite)
			{
				throw new RopExecutionException("Cannot write to a readonly stream", (ErrorCode)2147680261U);
			}
		}

		internal override void OnAccess()
		{
			base.OnAccess();
			this.streamSource.OnAccess();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PropertyStream>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.streamSource);
			if (this.systemStream != null)
			{
				try
				{
					if (this.streamChanged)
					{
						this.streamChanged = false;
						this.systemStream.Flush();
					}
					this.systemStream.Dispose();
				}
				finally
				{
					this.systemStream = null;
				}
			}
			base.InternalDispose();
		}

		private static SeekOrigin ToStreamSeekOrigin(StreamSeekOrigin origin)
		{
			switch (origin)
			{
			case StreamSeekOrigin.Begin:
				return SeekOrigin.Begin;
			case StreamSeekOrigin.Current:
				return SeekOrigin.Current;
			case StreamSeekOrigin.End:
				return SeekOrigin.End;
			default:
				throw new RopExecutionException(string.Format("Invalid StreamSeekOrigin value: {0}", origin), (ErrorCode)2147680343U);
			}
		}

		private const ulong DefaultCopyToStreamSegmentSize = 65535UL;

		private readonly StreamSource streamSource;

		private readonly PropertyType propertyType;

		private Stream systemStream;

		private bool streamChanged;
	}
}
