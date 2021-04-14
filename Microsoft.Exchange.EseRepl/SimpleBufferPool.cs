using System;

namespace Microsoft.Exchange.EseRepl
{
	internal class SimpleBufferPool : Pool<SimpleBuffer>, ISimpleBufferPool, IPool<SimpleBuffer>
	{
		public int BufferSize { get; private set; }

		public SimpleBufferPool(int bufSize, int preAllocCount) : base(preAllocCount)
		{
			this.BufferSize = bufSize;
			for (int i = 0; i < preAllocCount; i++)
			{
				SimpleBuffer o = new SimpleBuffer(bufSize, true);
				this.TryReturnObject(o);
			}
		}

		public SimpleBuffer TryGetObject(int bufSizeRequired)
		{
			if (bufSizeRequired > this.BufferSize)
			{
				return null;
			}
			return base.TryGetObject();
		}

		public override bool TryReturnObject(SimpleBuffer b)
		{
			return b.Buffer.Length == this.BufferSize && base.TryReturnObject(b);
		}
	}
}
