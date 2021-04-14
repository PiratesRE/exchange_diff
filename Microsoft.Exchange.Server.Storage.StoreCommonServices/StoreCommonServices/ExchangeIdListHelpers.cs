using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class ExchangeIdListHelpers
	{
		public static IList<ExchangeId> EmptyList
		{
			get
			{
				return ExchangeIdListHelpers.emptyList;
			}
		}

		public static IList<ExchangeId> ListFromBytes(Context context, IReplidGuidMap replidGuidMap, byte[] buff, ref int pos)
		{
			int num = 0;
			if (buff != null)
			{
				num = (int)ParseSerialize.GetDword(buff, ref pos, buff.Length);
				ParseSerialize.CheckCount((uint)num, 8, buff.Length - pos);
			}
			IList<ExchangeId> result = ExchangeIdListHelpers.EmptyList;
			if (num != 0)
			{
				ExchangeId[] array = new ExchangeId[num];
				for (int i = 0; i < num; i++)
				{
					long qword = (long)ParseSerialize.GetQword(buff, ref pos, buff.Length);
					array[i] = ExchangeId.CreateFromInt64(context, replidGuidMap, qword);
				}
				result = array;
			}
			return result;
		}

		public static byte[] BytesFromList(IList<ExchangeId> data, bool returnNullIfEmpty)
		{
			if (data != null && data.Count != 0)
			{
				byte[] array = new byte[4 + data.Count * 8];
				ParseSerialize.SerializeInt32(data.Count, array, 0);
				int num = 4;
				int i = 0;
				while (i < data.Count)
				{
					ParseSerialize.SerializeInt64(data[i].ToLong(), array, num);
					i++;
					num += 8;
				}
				return array;
			}
			if (!returnNullIfEmpty)
			{
				return ExchangeIdListHelpers.emptySerializedList;
			}
			return null;
		}

		private static readonly ExchangeId[] emptyList = new ExchangeId[0];

		private static readonly byte[] emptySerializedList = new byte[4];
	}
}
