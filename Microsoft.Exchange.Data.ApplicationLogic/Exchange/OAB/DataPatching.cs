using System;
using System.ComponentModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OAB;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DataPatching : DisposeTrackableBase
	{
		public DataPatching(int oldDataBufferSize, int patchBufferSize, int newDataBufferSize)
		{
			this.oldBuffer = new NativeBuffer(oldDataBufferSize);
			this.patchBuffer = new NativeBuffer(patchBufferSize);
			this.newBuffer = new NativeBuffer(newDataBufferSize);
			DataPatching.Tracer.TraceDebug<int, int, int>((long)this.GetHashCode(), "DataPatching: allocated {0} bytes for oldBuffer, {1} bytes for patchBuffer and {1} bytes for newBuffer.", oldDataBufferSize, patchBufferSize, newDataBufferSize);
		}

		public bool TryGenerate(byte[] oldData, byte[] newData, out byte[] patchData)
		{
			this.oldBuffer.CopyIn(oldData);
			this.newBuffer.CopyIn(newData);
			int num2;
			uint num = LzxInterop.CreateRawLzxPatchDataFromBuffers(this.oldBuffer.Buffer, oldData.Length, this.newBuffer.Buffer, newData.Length, this.patchBuffer.Buffer, this.patchBuffer.Size, out num2);
			uint num3 = num;
			if (num3 == 0U)
			{
				DataPatching.Tracer.TraceDebug<int, int, int>((long)this.GetHashCode(), "DataPatching: generated patch of {0} bytes from old data of {1} bytes and new data of {2} bytes", num2, oldData.Length, newData.Length);
				patchData = this.patchBuffer.CopyOut(num2);
				return true;
			}
			if (num3 != 122U)
			{
				DataPatching.Tracer.TraceError<int, int, uint>((long)this.GetHashCode(), "DataPatching: CreateRawLzxPatchDataFromBuffers failed to generate patch from old data of {0} bytes and new data of {1} bytes, error: {2}", oldData.Length, newData.Length, num);
				throw new Win32Exception((int)num, "CreateRawLzxPatchDataFromBuffers");
			}
			DataPatching.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "DataPatching: unable to generated patch from from old data of {0} bytes and new data of {1} bytes because patch buffer is not large enough.", oldData.Length, newData.Length);
			patchData = null;
			return false;
		}

		public byte[] Apply(byte[] oldData, byte[] patchData, int newDataSize)
		{
			if (newDataSize > this.newBuffer.Size)
			{
				throw new ArgumentException("newDataSize > newBuffer.Size");
			}
			this.oldBuffer.CopyIn(oldData);
			this.patchBuffer.CopyIn(patchData);
			uint num = LzxInterop.ApplyRawLzxPatchToBuffer(this.oldBuffer.Buffer, oldData.Length, this.patchBuffer.Buffer, patchData.Length, this.newBuffer.Buffer, newDataSize);
			if (num != 0U)
			{
				DataPatching.Tracer.TraceError((long)this.GetHashCode(), "DataPatching: ApplyRawLzxPatchToBuffer failed to apply patch of {0} bytes to old data of {1} bytes and generate new data of {2} bytes, error: {3}", new object[]
				{
					oldData.Length,
					patchData.Length,
					newDataSize,
					num
				});
				throw new Win32Exception((int)num, "ApplyRawLzxPatchToBuffer");
			}
			DataPatching.Tracer.TraceDebug<int, int, int>((long)this.GetHashCode(), "DataPatching: applied patch of {0} bytes to old data of {1} bytes and generated new data of {2} bytes", patchData.Length, oldData.Length, newDataSize);
			return this.newBuffer.CopyOut(newDataSize);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.oldBuffer.Dispose();
				this.patchBuffer.Dispose();
				this.newBuffer.Dispose();
				DataPatching.Tracer.TraceDebug((long)this.GetHashCode(), "DataPatching: released oldBuffer, patchBuffer and newBuffer.");
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DataPatching>(this);
		}

		private static readonly Trace Tracer = ExTraceGlobals.DataTracer;

		private readonly NativeBuffer oldBuffer;

		private readonly NativeBuffer patchBuffer;

		private readonly NativeBuffer newBuffer;
	}
}
