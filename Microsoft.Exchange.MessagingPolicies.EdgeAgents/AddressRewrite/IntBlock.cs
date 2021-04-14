using System;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal class IntBlock : Block<uint>
	{
		public IntBlock() : base(IntBlock.BlockSize)
		{
		}

		internal void Add(uint data)
		{
			this.buffer[this.written++] = data;
			this.free--;
		}

		internal static int BlockSize = 16384;
	}
}
