using System;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal class Block<ElementType> : IBlock where ElementType : struct
	{
		internal Block(int blockSize)
		{
			this.written = 0;
			this.free = blockSize;
			this.buffer = new ElementType[blockSize];
		}

		internal override int Written
		{
			get
			{
				return this.written;
			}
		}

		internal override int Free
		{
			get
			{
				return this.free;
			}
		}

		internal ElementType this[int index]
		{
			get
			{
				return this.buffer[index];
			}
			set
			{
				this.buffer[index] = value;
			}
		}

		protected ElementType[] buffer;

		protected int written;

		protected int free;
	}
}
