using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal class MapTable
	{
		internal string[] Domain
		{
			get
			{
				return this.domain;
			}
		}

		internal MapTable(string internalDomain, string externalDomain)
		{
			this.domain[0] = internalDomain;
			this.domain[1] = externalDomain;
			BlockArray<IntBlock> pointersArray = new BlockArray<IntBlock>();
			BlockArray<IntBlock> pointersArray2 = new BlockArray<IntBlock>();
			this.addressDataArray = new BlockArray<StringBlock>();
			this.internalIndex = new Index(pointersArray, this.addressDataArray);
			this.externalIndex = new Index(pointersArray2, this.addressDataArray);
		}

		internal bool IsCorrectMapTable(string domain, MapTable.MapEntryType direction)
		{
			return string.Compare(domain, this.domain[(int)direction], StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal void AddEntry(string internalAddress, string externalAddress)
		{
			this.internalIndex.Add(internalAddress, externalAddress.Length + 1);
			this.externalIndex.Add(externalAddress, 0);
		}

		internal string Remap(string address, MapTable.MapEntryType entryType)
		{
			Index index = (entryType == MapTable.MapEntryType.Internal) ? this.internalIndex : this.externalIndex;
			int num = index.BinarySearch(address);
			if (num == -1)
			{
				return null;
			}
			uint address2 = index[num];
			int offset = Macros.Offset(address2);
			StringBlock stringBlock = this.addressDataArray.Block(address2);
			int offset2;
			if (entryType == MapTable.MapEntryType.Internal)
			{
				offset2 = stringBlock.FindOffsetNextString(offset);
			}
			else
			{
				offset2 = stringBlock.FindOffsetPreviousString(offset);
			}
			byte[] bytes = new byte[256];
			int count;
			stringBlock.ReadStringUnsafe(offset2, ref bytes, out count);
			return Encoding.ASCII.GetString(bytes, 0, count);
		}

		internal void Sort()
		{
			this.internalIndex.HeapSort();
			this.externalIndex.HeapSort();
		}

		internal void Dump(List<string> internalIndex, List<string> externalIndex)
		{
			this.internalIndex.Dump(internalIndex);
			this.externalIndex.Dump(externalIndex);
		}

		private string[] domain = new string[2];

		private Index internalIndex;

		private Index externalIndex;

		private BlockArray<StringBlock> addressDataArray;

		internal enum MapEntryType
		{
			Internal,
			External
		}
	}
}
