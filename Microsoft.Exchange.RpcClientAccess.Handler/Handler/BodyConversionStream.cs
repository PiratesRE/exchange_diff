using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BodyConversionStream : Stream
	{
		internal BodyConversionStream(Func<PropertyTag, BodyHelper, Stream> conversionStreamFactory, Logon logon, StreamSource streamSource, PropertyTag propertyTag, bool seekToEnd, BodyHelper bodyHelper) : base(logon)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				Util.ThrowOnNullArgument(streamSource, "streamSource");
				Util.ThrowOnNullArgument(conversionStreamFactory, "conversionStreamFactory");
				this.conversionStreamFactory = conversionStreamFactory;
				this.streamSource = streamSource;
				this.propertyTag = propertyTag;
				this.seekToEnd = seekToEnd;
				this.bodyHelper = bodyHelper;
				disposeGuard.Success();
			}
		}

		internal long CurrentPosition
		{
			get
			{
				if (this.seekToEnd)
				{
					return this.StreamLength;
				}
				return this.currentPosition;
			}
		}

		internal long StreamLength
		{
			get
			{
				if (this.streamLength == null)
				{
					this.streamLength = new long?(this.ConversionStream.Length);
				}
				return this.streamLength.Value;
			}
		}

		private Stream ConversionStream
		{
			get
			{
				if (this.conversionStream == null)
				{
					this.conversionStream = this.conversionStreamFactory(this.propertyTag, this.bodyHelper);
				}
				return this.conversionStream;
			}
		}

		public override void Commit()
		{
			base.CheckDisposed();
		}

		public override uint GetSize()
		{
			base.CheckDisposed();
			return (uint)this.StreamLength;
		}

		public override ArraySegment<byte> Read(ushort requestedSize)
		{
			base.CheckDisposed();
			this.SeekToEndIfNeeded();
			return this.FillBuffer(new ArraySegment<byte>(new byte[(int)requestedSize]));
		}

		public override long Seek(StreamSeekOrigin streamSeekOrigin, long offset)
		{
			base.CheckDisposed();
			long num = this.CurrentPosition;
			long num2;
			long result;
			switch (streamSeekOrigin)
			{
			case StreamSeekOrigin.Begin:
				if (offset < 0L)
				{
					throw new RopExecutionException("Cannot seek backwards past beginning of stream", (ErrorCode)2147680281U);
				}
				if (this.CurrentPosition > offset)
				{
					this.ResetConversionStream();
				}
				else
				{
					this.SeekToEndIfNeeded();
					if (offset > this.StreamLength)
					{
						offset = this.StreamLength;
					}
				}
				num2 = offset - this.CurrentPosition;
				result = offset;
				break;
			case StreamSeekOrigin.Current:
				if (offset < 0L)
				{
					if (this.CurrentPosition + offset < 0L)
					{
						throw new RopExecutionException("Cannot seek backwards past beginning of stream", (ErrorCode)2147680281U);
					}
					offset = this.CurrentPosition + offset;
					this.ResetConversionStream();
					num2 = offset;
					result = offset;
				}
				else
				{
					this.SeekToEndIfNeeded();
					if (this.CurrentPosition + offset > this.StreamLength)
					{
						num2 = this.StreamLength - this.CurrentPosition;
						result = this.StreamLength;
					}
					else
					{
						num2 = offset;
						result = this.CurrentPosition + offset;
					}
				}
				break;
			case StreamSeekOrigin.End:
				if (offset > 0L)
				{
					this.SeekToEndIfNeeded();
					num2 = this.StreamLength - this.CurrentPosition;
					result = this.StreamLength;
				}
				else
				{
					if (this.StreamLength + offset < 0L)
					{
						throw new RopExecutionException("Cannot seek backwards past beginning of stream", (ErrorCode)2147680281U);
					}
					if (this.StreamLength + offset < this.CurrentPosition)
					{
						offset = this.StreamLength + offset;
						this.ResetConversionStream();
						num2 = offset;
						result = offset;
					}
					else
					{
						this.SeekToEndIfNeeded();
						num2 = this.StreamLength + offset - this.CurrentPosition;
						result = this.CurrentPosition + num2;
					}
				}
				break;
			default:
				throw new RopExecutionException("Unknown seek origin cannot seek in a conversion stream", (ErrorCode)2147680261U);
			}
			if (num2 + this.CurrentPosition > this.StreamLength)
			{
				throw new RopExecutionException("Cannot seek beyond end of conversion stream", (ErrorCode)2147680281U);
			}
			if (num2 > 0L)
			{
				this.AdvanceStream(num2);
			}
			return result;
		}

		public override void SetSize(ulong size)
		{
			base.CheckDisposed();
			throw new RopExecutionException("Cannot change the size of a conversion stream", (ErrorCode)2147680261U);
		}

		public override ushort Write(ArraySegment<byte> data)
		{
			base.CheckDisposed();
			throw new RopExecutionException("Cannot write to a conversion stream", (ErrorCode)2147680261U);
		}

		public override ulong CopyToStream(Stream destinationStream, ulong bytesToCopy)
		{
			base.CheckDisposed();
			this.SeekToEndIfNeeded();
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
			throw new RopExecutionException("Cannot write to a conversion stream", (ErrorCode)2147680261U);
		}

		internal override void OnAccess()
		{
			base.OnAccess();
		}

		private void ResetConversionStream()
		{
			Util.DisposeIfPresent(this.conversionStream);
			this.conversionStream = null;
			this.currentPosition = 0L;
			this.seekToEnd = false;
		}

		private void AdvanceStream(long bytesToAdvance)
		{
			if (bytesToAdvance > 0L)
			{
				int num = (int)Math.Min(65535L, bytesToAdvance);
				byte[] array = new byte[num];
				while (bytesToAdvance > 0L)
				{
					ArraySegment<byte> buffer = new ArraySegment<byte>(array);
					buffer = this.FillBuffer(buffer);
					if (buffer.Count == 0)
					{
						return;
					}
					bytesToAdvance -= (long)buffer.Count;
				}
			}
		}

		private void SeekToEndIfNeeded()
		{
			if (this.seekToEnd)
			{
				this.AdvanceStream(this.StreamLength);
			}
			this.seekToEnd = false;
		}

		private ArraySegment<byte> FillBuffer(ArraySegment<byte> buffer)
		{
			int num = 0;
			int count = buffer.Count;
			int num2;
			do
			{
				num2 = this.ConversionStream.Read(buffer.Array, buffer.Offset + num, count - num);
				num += num2;
			}
			while (num2 > 0 && num < count);
			this.currentPosition += (long)num;
			return new ArraySegment<byte>(buffer.Array, buffer.Offset, num);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<BodyConversionStream>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.streamSource);
			if (this.conversionStream != null)
			{
				try
				{
					this.conversionStream.Dispose();
				}
				finally
				{
					this.conversionStream = null;
				}
			}
			base.InternalDispose();
		}

		private const ulong DefaultCopyToStreamSegmentSize = 65535UL;

		private readonly StreamSource streamSource;

		private readonly PropertyTag propertyTag;

		private readonly BodyHelper bodyHelper;

		private readonly Func<PropertyTag, BodyHelper, Stream> conversionStreamFactory;

		private Stream conversionStream;

		private long currentPosition;

		private long? streamLength = null;

		private bool seekToEnd;
	}
}
