using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class CommonSyncProperties : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition ObjectIdProp = new HygienePropertyDefinition("ObjectId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition TenantIdProp = new HygienePropertyDefinition("TenantId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition ServiceInstanceProp = new HygienePropertyDefinition("ServiceInstance", typeof(string));

		public static readonly HygienePropertyDefinition SyncOnlyProp = new HygienePropertyDefinition("SyncOnly", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition MailEnabledGroupProp = new HygienePropertyDefinition("IsMailEnabledGroup", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition PublishedProp = new HygienePropertyDefinition("isPublished", typeof(bool), false, ExchangeObjectVersion.Exchange2007, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ProvisioningFlagsProperty = new HygienePropertyDefinition("ProvisioningFlags", typeof(ProvisioningFlags), ProvisioningFlags.Default, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition SyncTypeProp = new HygienePropertyDefinition("SyncType", typeof(SyncType));

		public static readonly HygienePropertyDefinition ObjectTypeProp = new HygienePropertyDefinition("ObjectType", typeof(DirectoryObjectClass));

		public static readonly HygienePropertyDefinition BatchIdProp = new HygienePropertyDefinition("BatchId", typeof(Guid));

		public static readonly HygienePropertyDefinition ForwardSyncObjectProp = new HygienePropertyDefinition("ForwardSyncObject", typeof(bool), false, ExchangeObjectVersion.Exchange2007, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
