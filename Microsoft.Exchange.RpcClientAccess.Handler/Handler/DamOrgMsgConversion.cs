using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal sealed class DamOrgMsgConversion : PropertyConversion
	{
		internal DamOrgMsgConversion() : base(PropertyTag.DamOrgMsgServerId, PropertyTag.DamOrgMsgEntryId)
		{
		}

		protected override object ConvertValueFromClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			return ServerIdConverter.MakeEntryIdFromServerId(session, (byte[])propertyValue);
		}

		protected override object ConvertValueToClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			return ServerIdConverter.MakeServerIdFromEntryId(session, (byte[])propertyValue);
		}
	}
}
