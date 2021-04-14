using System;
using System.ComponentModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OAB;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DataCompression : DisposeTrackableBase
	{
		public DataCompression(int uncompressedBufferSize, int compressedBufferSize)
		{
			this.uncompressedBuffer = new NativeBuffer(uncompressedBufferSize);
			this.compressedBuffer = new NativeBuffer(compressedBufferSize);
			DataCompression.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "DataCompression: allocated {0} bytes for uncompressedBuffer and {1} bytes for compressedBuffer.", uncompressedBufferSize, compressedBufferSize);
		}

		public bool TryCompress(byte[] uncompressedData, out byte[] compressedData)
		{
			this.uncompressedBuffer.CopyIn(uncompressedData);
			int num2;
			uint num = LzxInterop.RawLzxCompressBuffer(this.uncompressedBuffer.Buffer, uncompressedData.Length, this.compressedBuffer.Buffer, this.compressedBuffer.Size, out num2);
			uint num3 = num;
			if (num3 == 0U)
			{
				DataCompression.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "DataCompression: compressed input of {0} bytes into {1} bytes", uncompressedData.Length, num2);
				compressedData = this.compressedBuffer.CopyOut(num2);
				return true;
			}
			if (num3 != 122U)
			{
				DataCompression.Tracer.TraceError<int, uint>((long)this.GetHashCode(), "DataCompression: RawLzxCompressBuffer failed to compress {0} bytes, error: {0}", uncompressedData.Length, num);
				throw new Win32Exception((int)num, "RawLzxCompressBuffer");
			}
			DataCompression.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "DataCompression: unable to compressed input of {0} bytes into {1} bytes because output buffer is not large enough.", uncompressedData.Length, num2);
			compressedData = null;
			return false;
		}

		public byte[] Decompress(byte[] compressedData, int decompressedDataSize)
		{
			if (decompressedDataSize > this.uncompressedBuffer.Size)
			{
				throw new ArgumentException("decompressedDataSize > uncompressedBuffer.Size");
			}
			this.compressedBuffer.CopyIn(compressedData);
			uint num = LzxInterop.ApplyRawLzxPatchToBuffer(IntPtr.Zero, 0, this.compressedBuffer.Buffer, compressedData.Length, this.uncompressedBuffer.Buffer, decompressedDataSize);
			if (num != 0U)
			{
				DataCompression.Tracer.TraceError<int, uint>((long)this.GetHashCode(), "DataCompression: ApplyRawLzxPatchToBuffer failed to decompress {0} bytes, error: {1}", compressedData.Length, num);
				throw new Win32Exception((int)num, "ApplyRawLzxPatchToBuffer");
			}
			DataCompression.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "DataCompression: decompressed input of {0} bytes into {1} bytes", compressedData.Length, decompressedDataSize);
			return this.uncompressedBuffer.CopyOut(decompressedDataSize);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.uncompressedBuffer.Dispose();
				this.compressedBuffer.Dispose();
				DataCompression.Tracer.TraceDebug((long)this.GetHashCode(), "DataCompression: released allocated memory for uncompressedBuffer and compressedBuffer.");
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DataCompression>(this);
		}

		private static readonly Trace Tracer = ExTraceGlobals.DataTracer;

		private readonly NativeBuffer uncompressedBuffer;

		private readonly NativeBuffer compressedBuffer;
	}
}
