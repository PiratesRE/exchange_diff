using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal sealed class MessageSubmissionIdConversion : PropertyConversion
	{
		internal MessageSubmissionIdConversion() : base(PropertyTag.MessageSubmissionIdFromClient, PropertyTag.MessageSubmissionId)
		{
		}

		protected override object ConvertValueFromClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			return propertyValue;
		}

		protected override object ConvertValueToClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			return propertyValue;
		}
	}
}
