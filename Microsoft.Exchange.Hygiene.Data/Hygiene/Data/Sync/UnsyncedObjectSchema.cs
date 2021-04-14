using System;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class UnsyncedObjectSchema : UnpublishedObjectSchema
	{
		public static readonly HygienePropertyDefinition SyncTypeProp = CommonSyncProperties.SyncTypeProp;

		public static readonly HygienePropertyDefinition BatchIdProp = CommonSyncProperties.BatchIdProp;
	}
}
