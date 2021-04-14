using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal static class PropertyConversionHelper
	{
		public static byte[] ConvertFidMidPairToEntryId(StoreSession session, byte[] fidmid)
		{
			byte[] providerLevelItemId;
			using (BufferReader bufferReader = Reader.CreateBufferReader(fidmid))
			{
				StoreIdPair storeIdPair = StoreIdPair.Parse(bufferReader);
				IdConverter idConverter = session.IdConverter;
				try
				{
					StoreObjectId storeObjectId;
					if (storeIdPair.Second != StoreId.Empty)
					{
						storeObjectId = idConverter.CreateMessageId(storeIdPair.First, storeIdPair.Second);
					}
					else
					{
						storeObjectId = idConverter.CreateFolderId(storeIdPair.First);
					}
					providerLevelItemId = storeObjectId.ProviderLevelItemId;
				}
				catch (ObjectNotFoundException innerException)
				{
					throw new RopExecutionException("Invalid fidmid", (ErrorCode)2147746063U, innerException);
				}
				catch (CorruptDataException innerException2)
				{
					throw new RopExecutionException("Corrupt fidmid", (ErrorCode)2147746075U, innerException2);
				}
			}
			return providerLevelItemId;
		}

		public static byte[] ConvertEntryIdToFidMidPair(StoreSession session, byte[] entryId)
		{
			long nativeId = 0L;
			long nativeId2 = 0L;
			try
			{
				StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(entryId);
				if (IdConverter.IsMessageId(storeObjectId))
				{
					nativeId = session.IdConverter.GetFidFromId(storeObjectId);
					nativeId2 = session.IdConverter.GetMidFromMessageId(storeObjectId);
				}
				else
				{
					if (!IdConverter.IsFolderId(storeObjectId))
					{
						throw new RopExecutionException("Corrupt entry id.", (ErrorCode)2147746075U);
					}
					nativeId = session.IdConverter.GetFidFromId(storeObjectId);
				}
			}
			catch (CorruptDataException innerException)
			{
				throw new RopExecutionException("Corrupt entry id.", (ErrorCode)2147746075U, innerException);
			}
			byte[] array = new byte[16];
			using (BufferWriter bufferWriter = new BufferWriter(array))
			{
				StoreIdPair storeIdPair = new StoreIdPair(new StoreId(nativeId), new StoreId(nativeId2));
				storeIdPair.Serialize(bufferWriter);
			}
			return array;
		}
	}
}
