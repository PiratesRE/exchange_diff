using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal sealed class ConflictMsgKeyConversion : PropertyConversion
	{
		internal ConflictMsgKeyConversion() : base(PropertyTag.ConflictMsgKey, PropertyTag.ConflictEntryId)
		{
		}

		protected override object ConvertValueFromClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			byte[] array = (byte[])propertyValue;
			if (array.Length != 22 && array.Length != 44)
			{
				throw new RopExecutionException(string.Format("Invalid conflict message key {0}.", new ArrayTracer<byte>(array)), (ErrorCode)2147942487U);
			}
			IdConverter idConverter = session.IdConverter;
			byte[] array2 = new byte[22];
			Array.Copy(array, 0, array2, 0, 22);
			long idFromLongTermId = idConverter.GetIdFromLongTermId(array2);
			StoreObjectId storeObjectId;
			if (array.Length == 44)
			{
				byte[] array3 = new byte[22];
				Array.Copy(array, 22, array3, 0, 22);
				long idFromLongTermId2 = idConverter.GetIdFromLongTermId(array3);
				storeObjectId = idConverter.CreatePublicMessageId(idFromLongTermId, idFromLongTermId2);
			}
			else
			{
				storeObjectId = idConverter.CreatePublicFolderId(idFromLongTermId);
			}
			return storeObjectId.ProviderLevelItemId;
		}

		protected override object ConvertValueToClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId((byte[])propertyValue);
			long fidFromId = session.IdConverter.GetFidFromId(storeObjectId);
			byte[] longTermIdFromId = session.IdConverter.GetLongTermIdFromId(fidFromId);
			byte[] array;
			if (IdConverter.IsMessageId(storeObjectId))
			{
				long midFromMessageId = session.IdConverter.GetMidFromMessageId(storeObjectId);
				byte[] longTermIdFromId2 = session.IdConverter.GetLongTermIdFromId(midFromMessageId);
				array = new byte[44];
				Array.Copy(longTermIdFromId, 0, array, 0, 22);
				Array.Copy(longTermIdFromId2, 0, array, 22, 22);
			}
			else
			{
				array = new byte[22];
				Array.Copy(longTermIdFromId, 0, array, 0, 22);
			}
			return array;
		}
	}
}
