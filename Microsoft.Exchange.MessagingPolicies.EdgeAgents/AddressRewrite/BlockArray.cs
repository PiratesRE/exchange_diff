using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal class BlockArray<BlockType> where BlockType : IBlock, new()
	{
		internal int Count
		{
			get
			{
				return this.count;
			}
		}

		internal BlockType this[int blockIndex]
		{
			get
			{
				return this.blocks[blockIndex];
			}
		}

		internal BlockType Block(uint address)
		{
			return this.blocks[Macros.BlockIndex(address)];
		}

		internal int FindBlockToAppendData(int toWrite)
		{
			BlockType blockType = (this.count == 0) ? default(BlockType) : this.blocks[this.count - 1];
			if (blockType == null || toWrite > blockType.Free)
			{
				blockType = Activator.CreateInstance<BlockType>();
				if (this.count >= this.blocks.Count)
				{
					this.blocks.Add(blockType);
				}
				if (toWrite > blockType.Free)
				{
					throw new OutOfMemoryException();
				}
				this.blocks[this.count++] = blockType;
			}
			return this.count - 1;
		}

		private int count;

		private List<BlockType> blocks = new List<BlockType>(BlockArray<BlockType>.NumBlocks);

		internal static int NumBlocks = 256;
	}
}
