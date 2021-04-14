using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class EntryIdHelpers
	{
		public static byte[] FormatServerEntryId(bool export, ExchangeId fid, ExchangeId mid, int instanceNum)
		{
			if (!export)
			{
				return ExchangeIdHelpers.BuildOursServerEntryId(fid.ToLong(), mid.ToLong(), instanceNum);
			}
			byte[] array = new byte[23 + (mid.IsValid ? 22 : 0)];
			int num = 0;
			array[num++] = 2;
			num += ExchangeIdHelpers.To22ByteArray(fid.Guid, fid.Counter, array, num);
			if (mid.IsValid)
			{
				ExchangeIdHelpers.To22ByteArray(mid.Guid, mid.Counter, array, num);
			}
			return array;
		}

		public static bool ParseServerEntryId(Context context, IReplidGuidMap replidGuidMap, byte[] entryId, bool export, out ExchangeId fid, out ExchangeId mid, out int instanceNum)
		{
			fid = ExchangeId.Null;
			mid = ExchangeId.Null;
			instanceNum = 0;
			if (entryId == null || entryId.Length < 1)
			{
				return false;
			}
			byte b = entryId[0];
			if (b == 1 && !export)
			{
				long legacyId;
				long legacyId2;
				if (!ExchangeIdHelpers.ParseOursServerEntryId(entryId, out legacyId, out legacyId2, out instanceNum))
				{
					return false;
				}
				fid = ExchangeId.CreateFromInt64(context, replidGuidMap, legacyId);
				mid = ExchangeId.CreateFromInt64(context, replidGuidMap, legacyId2);
			}
			else
			{
				if (b != 2 || !export)
				{
					return false;
				}
				if (entryId.Length != 23 && entryId.Length != 45)
				{
					return false;
				}
				fid = ExchangeId.CreateFrom22ByteArray(context, replidGuidMap, entryId, 1);
				if (entryId.Length == 45)
				{
					mid = ExchangeId.CreateFrom22ByteArray(context, replidGuidMap, entryId, 23);
				}
			}
			return true;
		}

		public static byte[] ExchangeIdTo46ByteEntryId(ExchangeId exchangeId, Guid entryIdGuid, EntryIdHelpers.EIDType eidType)
		{
			byte[] array = new byte[46];
			int num = 0;
			array[num++] = 0;
			array[num++] = 0;
			array[num++] = 0;
			array[num++] = 0;
			num += ParseSerialize.SerializeGuid(entryIdGuid, array, num);
			array[num++] = (byte)eidType;
			array[num++] = 0;
			num += ExchangeIdHelpers.To22ByteArray(exchangeId.Guid, exchangeId.Counter, array, num);
			array[num++] = 0;
			array[num++] = 0;
			return array;
		}

		public enum EIDType
		{
			eitSTPrivateFolder,
			eitLTPrivateFolder,
			eitSTPublicFolder,
			eitLTPublicFolder,
			eitSTWackyFolder,
			eitLTWackyFolder,
			eitSTPrivateMessage,
			eitLTPrivateMessage,
			eitSTPublicMessage,
			eitLTPublicMessage,
			eitSTWackyMessage,
			eitLTWackyMessage,
			eitLTPublicFolderByName
		}

		public enum SVREIDType : byte
		{
			NotOurs,
			Ours,
			Export,
			Gids,
			Max
		}
	}
}
