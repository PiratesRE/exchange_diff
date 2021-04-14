using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AddressEntryList
	{
		public static AddressEntryList Parse(byte[] entryIdBlob, Encoding string8Encoding)
		{
			AddressEntryList result;
			using (Reader reader = Reader.CreateBufferReader(entryIdBlob))
			{
				result = AddressEntryList.InternalParse(reader, string8Encoding);
			}
			return result;
		}

		public byte[] Serialize()
		{
			return AddressEntryId.ToBytes(delegate(Writer writer)
			{
				writer.WriteUInt32((uint)this.internalList.Count);
				writer.WriteSizedBlock(delegate
				{
					int num = 0;
					using (IEnumerator<AddressEntryId> enumerator = this.internalList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							AddressEntryId entryId = enumerator.Current;
							uint size = writer.WriteSizedBlock(delegate
							{
								entryId.Serialize(writer);
							});
							if (num < this.internalList.Count - 1)
							{
								writer.WriteBytes(new byte[AddressEntryList.PaddingSize(size)]);
							}
							num++;
						}
					}
				});
			});
		}

		public void SetUnicode()
		{
			foreach (AddressEntryId addressEntryId in this.internalList)
			{
				addressEntryId.SetUnicode();
			}
		}

		public void SetString8(Encoding string8Encoding)
		{
			foreach (AddressEntryId addressEntryId in this.internalList)
			{
				addressEntryId.SetString8(string8Encoding);
			}
		}

		private static uint PaddingSize(uint size)
		{
			uint num = size & 3U;
			if (num != 0U)
			{
				return 4U - num;
			}
			return 0U;
		}

		private static AddressEntryList InternalParse(Reader reader, Encoding string8Encoding)
		{
			uint num = reader.ReadUInt32();
			uint num2 = reader.ReadUInt32();
			if (num * 4U > num2 || (ulong)num2 != (ulong)(reader.Length - 8L))
			{
				throw new BufferParseException(string.Format("The total size of the AddressEntryList is not correct. Entry count = {0}, Size = {1}, Length = {2}.", num, num2, reader.Length));
			}
			AddressEntryList addressEntryList = new AddressEntryList();
			for (uint num3 = 0U; num3 < num; num3 += 1U)
			{
				uint num4 = reader.ReadUInt32();
				if (AddressBookEntryId.IsAddressBookEntryId(reader, num4))
				{
					addressEntryList.Add(AddressEntryId.Parse(reader, string8Encoding, num4));
				}
				else
				{
					addressEntryList.Add(OneOffEntryId.Parse(reader, string8Encoding, num4));
				}
				if (num3 != num - 1U)
				{
					reader.ReadArraySegment(AddressEntryList.PaddingSize(num4));
				}
			}
			return addressEntryList;
		}

		public void Add(AddressEntryId item)
		{
			this.internalList.Add(item);
		}

		private readonly IList<AddressEntryId> internalList = new List<AddressEntryId>();
	}
}
