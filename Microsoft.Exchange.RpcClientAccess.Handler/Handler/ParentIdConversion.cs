using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal sealed class ParentIdConversion : PropertyConversion
	{
		internal ParentIdConversion() : base(PropertyTag.ParentFid, PropertyTag.ParentEntryId)
		{
		}

		protected override object ConvertValueFromClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			return ServerIdConverter.MakeEntryIdFromFid(session, (long)propertyValue);
		}

		protected override object ConvertValueToClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId((byte[])propertyValue);
			return session.IdConverter.GetFidFromId(storeObjectId);
		}
	}
}
