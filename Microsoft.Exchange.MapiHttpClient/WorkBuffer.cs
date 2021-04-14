using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WorkBuffer : BaseObject
	{
		public WorkBuffer(int maxSizeOfBuffer)
		{
			if (maxSizeOfBuffer < 0)
			{
				throw new ArgumentException("Max buffer size cannot be negative.");
			}
			this.maxSizeOfBuffer = Math.Min(maxSizeOfBuffer, 16777216);
			if (this.maxSizeOfBuffer <= 0)
			{
				this.arraySegment = Array<byte>.EmptySegment;
				return;
			}
			if (this.maxSizeOfBuffer > AsyncBufferPools.MaxBufferSize)
			{
				this.oneOffBuffer = new byte[this.maxSizeOfBuffer];
				this.arraySegment = new ArraySegment<byte>(this.oneOffBuffer);
				return;
			}
			this.leasedBuffer = AsyncBufferPools.GetBuffer(this.maxSizeOfBuffer);
			this.arraySegment = new ArraySegment<byte>(this.leasedBuffer, 0, this.maxSizeOfBuffer);
		}

		public WorkBuffer(ArraySegment<byte> data)
		{
			this.maxSizeOfBuffer = data.Count;
			this.arraySegment = data;
		}

		public ArraySegment<byte> ArraySegment
		{
			get
			{
				base.CheckDisposed();
				return this.arraySegment;
			}
		}

		public byte[] Array
		{
			get
			{
				base.CheckDisposed();
				return this.arraySegment.Array;
			}
		}

		public int Offset
		{
			get
			{
				base.CheckDisposed();
				return this.arraySegment.Offset;
			}
		}

		public int Count
		{
			get
			{
				base.CheckDisposed();
				return this.arraySegment.Count;
			}
			set
			{
				base.CheckDisposed();
				if (value > this.maxSizeOfBuffer)
				{
					throw new ArgumentException("Cannot set count to more than maximum size of buffer.");
				}
				if (this.leasedBuffer != null)
				{
					this.arraySegment = new ArraySegment<byte>(this.leasedBuffer, 0, value);
					return;
				}
				if (this.oneOffBuffer != null)
				{
					this.arraySegment = new ArraySegment<byte>(this.oneOffBuffer, 0, value);
					return;
				}
				this.arraySegment = new ArraySegment<byte>(this.arraySegment.Array, this.arraySegment.Offset, value);
			}
		}

		public int MaxSize
		{
			get
			{
				base.CheckDisposed();
				return this.maxSizeOfBuffer;
			}
		}

		protected override void InternalDispose()
		{
			if (this.leasedBuffer != null)
			{
				AsyncBufferPools.ReleaseBuffer(this.leasedBuffer);
			}
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<WorkBuffer>(this);
		}

		private const int MaximumAllowedAllocationSize = 16777216;

		private readonly byte[] leasedBuffer;

		private readonly byte[] oneOffBuffer;

		private readonly int maxSizeOfBuffer;

		private ArraySegment<byte> arraySegment;
	}
}
