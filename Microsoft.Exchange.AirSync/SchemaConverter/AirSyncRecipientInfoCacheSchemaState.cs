using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.RecipientInfoCache;

namespace Microsoft.Exchange.AirSync.SchemaConverter
{
	internal class AirSyncRecipientInfoCacheSchemaState : AirSyncSchemaState, IDataObjectGenerator
	{
		public RecipientInfoCacheDataObject GetRecipientInfoCacheDataObject()
		{
			return new RecipientInfoCacheDataObject(base.GetSchema(1));
		}
	}
}
