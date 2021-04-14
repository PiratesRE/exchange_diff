using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public abstract class TraceBuffer : DisposableBase
	{
		protected TraceBuffer(Guid recordGuid, byte[] data)
		{
			this.recordGuid = recordGuid;
			this.data = data;
			this.offset = 0;
		}

		public Guid RecordGuid
		{
			get
			{
				return this.recordGuid;
			}
		}

		public byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		public int Length
		{
			get
			{
				return this.offset;
			}
		}

		public static TraceBuffer Create(Guid recordGuid, int length, bool useBufferPool)
		{
			if (useBufferPool)
			{
				return TraceBuffer.TraceBufferPool.Create(recordGuid, length);
			}
			return TraceBuffer.TraceBufferInstance.Create(recordGuid, length);
		}

		public void WriteByte(byte value)
		{
			if (this.offset + 1 <= this.data.Length)
			{
				this.data[this.offset] = value;
				this.offset++;
			}
		}

		public void WriteShort(short value)
		{
			if (this.offset + 2 <= this.data.Length)
			{
				this.offset += ExBitConverter.Write(value, this.data, this.offset);
			}
		}

		public void WriteInt(int value)
		{
			if (this.offset + 4 <= this.data.Length)
			{
				this.offset += ExBitConverter.Write(value, this.data, this.offset);
			}
		}

		public void WriteInt(uint value)
		{
			if (this.offset + 4 <= this.data.Length)
			{
				this.offset += ExBitConverter.Write(value, this.data, this.offset);
			}
		}

		public void WriteLong(long value)
		{
			if (this.offset + 8 <= this.data.Length)
			{
				this.offset += ExBitConverter.Write(value, this.data, this.offset);
			}
		}

		public void WriteDouble(double value)
		{
			if (this.offset + 8 <= this.data.Length)
			{
				this.offset += ExBitConverter.Write(value, this.data, this.offset);
			}
		}

		public void WriteCountedAsciiString(string value)
		{
			string text = value ?? string.Empty;
			int byteCount = CTSGlobals.AsciiEncoding.GetByteCount(text);
			if (byteCount < 32767)
			{
				int num = byteCount + 2 + 1;
				short value2 = (short)byteCount;
				int num2 = 0;
				if (this.offset + num <= this.data.Length)
				{
					num2 += ExBitConverter.Write(value2, this.data, this.offset);
					num2 += ExBitConverter.Write(text, false, this.data, this.offset + 2);
					this.offset += num2 - 1;
				}
			}
		}

		public void WriteAsciiString(string value)
		{
			string text = value ?? string.Empty;
			int num = CTSGlobals.AsciiEncoding.GetByteCount(text) + 1;
			if (this.offset + num <= this.data.Length)
			{
				this.offset += ExBitConverter.Write(text, false, this.data, this.offset);
			}
		}

		public void WriteCountedUnicodeString(string value)
		{
			string text = value ?? string.Empty;
			int byteCount = CTSGlobals.UnicodeEncoding.GetByteCount(text);
			if (byteCount < 32767)
			{
				int num = byteCount + 2 + 2;
				short value2 = (short)byteCount;
				int num2 = 0;
				if (this.offset + num <= this.data.Length)
				{
					num2 += ExBitConverter.Write(value2, this.data, this.offset);
					num2 += ExBitConverter.Write(text, true, this.data, this.offset + 2);
					this.offset += num2 - 2;
				}
			}
		}

		public void WriteUnicodeString(string value)
		{
			string text = value ?? string.Empty;
			int num = CTSGlobals.UnicodeEncoding.GetByteCount(text) + 2;
			if (this.offset + num <= this.data.Length)
			{
				this.offset += ExBitConverter.Write(text, true, this.data, this.offset);
			}
		}

		private readonly Guid recordGuid;

		private readonly byte[] data;

		private int offset;

		private sealed class TraceBufferInstance : TraceBuffer
		{
			private TraceBufferInstance(Guid recordGuid, byte[] data) : base(recordGuid, data)
			{
			}

			public static TraceBuffer Create(Guid recordGuid, int length)
			{
				return new TraceBuffer.TraceBufferInstance(recordGuid, new byte[length]);
			}

			protected override void InternalDispose(bool calledFromDispose)
			{
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<TraceBuffer.TraceBufferInstance>(this);
			}
		}

		private sealed class TraceBufferPool : TraceBuffer
		{
			private TraceBufferPool(Guid recordGuid, BufferPool bufferPool, byte[] data) : base(recordGuid, data)
			{
				this.bufferPool = bufferPool;
			}

			public static TraceBuffer Create(Guid recordGuid, int length)
			{
				BufferPoolCollection.BufferSize bufferSize;
				if (!BufferPoolCollection.AutoCleanupCollection.TryMatchBufferSize(length, out bufferSize))
				{
					return new TraceBuffer.TraceBufferPool(recordGuid, null, TraceBuffer.TraceBufferPool.Empty);
				}
				BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(bufferSize);
				return new TraceBuffer.TraceBufferPool(recordGuid, bufferPool, bufferPool.Acquire());
			}

			protected override void InternalDispose(bool calledFromDispose)
			{
				if (calledFromDispose && this.bufferPool != null)
				{
					this.bufferPool.Release(base.Data);
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<TraceBuffer.TraceBufferPool>(this);
			}

			private static readonly byte[] Empty = new byte[0];

			private readonly BufferPool bufferPool;
		}
	}
}
