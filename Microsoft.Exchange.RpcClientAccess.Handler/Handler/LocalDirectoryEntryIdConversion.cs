using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal sealed class LocalDirectoryEntryIdConversion : PropertyConversion
	{
		internal LocalDirectoryEntryIdConversion() : base(PropertyTag.LocalDirectoryEntryId, PropertyTag.LocalDirectoryEntryId)
		{
		}

		protected override object ConvertValueFromClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			byte[] array = propertyValue as byte[];
			if (array == null || array.Length != 16)
			{
				throw new RopExecutionException("Invalid size of fidmid", (ErrorCode)2147746075U);
			}
			return PropertyConversionHelper.ConvertFidMidPairToEntryId(session, array);
		}

		protected override object ConvertValueToClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue)
		{
			byte[] array = propertyValue as byte[];
			if (array == null)
			{
				throw new RopExecutionException(string.Format("entryId is not byte[]: {0}.", propertyValue), (ErrorCode)2147746055U);
			}
			return PropertyConversionHelper.ConvertEntryIdToFidMidPair(session, array);
		}
	}
}
