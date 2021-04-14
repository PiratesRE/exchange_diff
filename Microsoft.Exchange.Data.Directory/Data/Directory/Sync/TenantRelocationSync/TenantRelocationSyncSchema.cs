using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class TenantRelocationSyncSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition AllAttributes = new ADPropertyDefinition("AllAttributes", ExchangeObjectVersion.Exchange2003, typeof(Guid), "*", ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.DoNotProvisionalClone, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition ObjectId = SyncObjectSchema.ObjectId;

		public static ADPropertyDefinition LastKnownParent = SyncObjectSchema.LastKnownParent;

		public static ADPropertyDefinition Deleted = SyncObjectSchema.Deleted;

		public static ADPropertyDefinition ConfigurationXMLRaw = ADRecipientSchema.ConfigurationXMLRaw;
	}
}
