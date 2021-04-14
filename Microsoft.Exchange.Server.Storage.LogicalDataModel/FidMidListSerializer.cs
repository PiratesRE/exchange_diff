using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class FidMidListSerializer
	{
		public static List<FidMid> FromBytes(byte[] buffer, ref int offset, IReplidGuidMap replidGuidMap)
		{
			if (buffer == null)
			{
				return new List<FidMid>(0);
			}
			int dword = (int)ParseSerialize.GetDword(buffer, ref offset, buffer.Length);
			List<FidMid> list = new List<FidMid>(dword);
			for (int i = 0; i < dword; i++)
			{
				ExchangeId folderId = ExchangeId.CreateFrom26ByteArray(null, null, ParseSerialize.GetByteArray(buffer, ref offset, buffer.Length));
				ExchangeId messageId = ExchangeId.CreateFrom26ByteArray(null, null, ParseSerialize.GetByteArray(buffer, ref offset, buffer.Length));
				list.Add(new FidMid(folderId, messageId));
			}
			return list;
		}

		public static byte[] ToBytes(IList<FidMid> data)
		{
			if (data == null)
			{
				return new byte[4];
			}
			byte[] array = new byte[4 + data.Count * 28 * 2];
			int num = ParseSerialize.SerializeInt32(data.Count, array, 0);
			for (int i = 0; i < data.Count; i++)
			{
				FidMid fidMid = data[i];
				ParseSerialize.SetByteArray(array, ref num, fidMid.FolderId.To26ByteArray());
				ParseSerialize.SetByteArray(array, ref num, fidMid.MessageId.To26ByteArray());
			}
			return array;
		}

		private const int SizeOfSerializedExchangeId = 28;
	}
}
