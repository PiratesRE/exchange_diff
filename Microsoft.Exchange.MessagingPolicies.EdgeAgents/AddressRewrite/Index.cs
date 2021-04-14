using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal class Index
	{
		internal Index(BlockArray<IntBlock> pointersArray, BlockArray<StringBlock> dataArray)
		{
			this.pointersArray = pointersArray;
			this.dataArray = dataArray;
		}

		internal void Add(string data, int extraSpaceNeeded)
		{
			int blockIndex = this.dataArray.FindBlockToAppendData(data.Length + 1 + extraSpaceNeeded);
			int offset = this.dataArray[blockIndex].AppendUnsafe(data);
			uint data2 = Macros.Address(blockIndex, offset);
			blockIndex = this.pointersArray.FindBlockToAppendData(1);
			this.pointersArray[blockIndex].Add(data2);
			this.count++;
		}

		internal uint this[int index]
		{
			get
			{
				int blockIndex = index / IntBlock.BlockSize;
				int index2 = index % IntBlock.BlockSize;
				return this.pointersArray[blockIndex][index2];
			}
			set
			{
				int blockIndex = index / IntBlock.BlockSize;
				int index2 = index % IntBlock.BlockSize;
				this.pointersArray[blockIndex][index2] = value;
			}
		}

		internal DataReference Ptr(uint dataPointer)
		{
			int blockIndex = Macros.BlockIndex(dataPointer);
			int offset = Macros.Offset(dataPointer);
			byte[] buffer;
			int length;
			this.dataArray[blockIndex].GetDataReference(offset, out buffer, out length);
			return new DataReference(buffer, offset, length);
		}

		internal int BinarySearch(string searchData)
		{
			if (this.count == 0)
			{
				return -1;
			}
			int num = -1;
			int num2 = this.count;
			DataReference dataReference = new DataReference(searchData);
			int num3;
			for (;;)
			{
				num3 = (num + num2) / 2;
				DataReference other = this.Ptr(this[num3]);
				int num4 = dataReference.CompareTo(other);
				if (num4 == 0)
				{
					break;
				}
				if (num4 < 0)
				{
					num2 = num3;
				}
				else if (num4 > 0)
				{
					num = num3;
				}
				if (num == num2 || num2 == num + 1)
				{
					return -1;
				}
			}
			return num3;
		}

		internal void HeapSort()
		{
			this.BuildHeap();
			for (int i = this.count - 1; i >= 1; i--)
			{
				uint value = this[0];
				this[0] = this[i];
				this[i] = value;
				this.heapSize--;
				this.Heapify(0);
			}
		}

		private void BuildHeap()
		{
			this.heapSize = this.count;
			for (int i = this.count / 2 - 1; i >= 0; i--)
			{
				this.Heapify(i);
			}
		}

		private void Heapify(int i)
		{
			for (;;)
			{
				int num = Macros.Left(i);
				int num2 = Macros.Right(i);
				int num3 = i;
				if (num < this.heapSize)
				{
					DataReference dataReference = this.Ptr(this[num]);
					DataReference other = this.Ptr(this[i]);
					num3 = ((dataReference.CompareTo(other) > 0) ? num : i);
				}
				if (num2 < this.heapSize && this.Ptr(this[num2]).CompareTo(this.Ptr(this[num3])) > 0)
				{
					num3 = num2;
				}
				if (num3 == i)
				{
					break;
				}
				uint value = this[num3];
				this[num3] = this[i];
				this[i] = value;
				i = num3;
			}
		}

		internal void Dump(List<string> outputList)
		{
			for (int i = 0; i < this.count; i++)
			{
				uint dataPointer = this[i];
				outputList.Add(this.Ptr(dataPointer).ToString());
			}
		}

		private BlockArray<IntBlock> pointersArray;

		private BlockArray<StringBlock> dataArray;

		private int count;

		private int heapSize;
	}
}
