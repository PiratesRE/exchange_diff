using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync.CookieManager;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	internal class MsoTenantSyncRequestSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition TenantSyncType = new SimpleProviderPropertyDefinition("TenantSyncType", ExchangeObjectVersion.Exchange2010, typeof(TenantSyncType), PropertyDefinitionFlags.PersistDefaultValue, Microsoft.Exchange.Data.Directory.Sync.CookieManager.TenantSyncType.Full, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Requestor = new SimpleProviderPropertyDefinition("Requestor", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WhenCreated = new SimpleProviderPropertyDefinition("WhenCreated", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WhenSyncStarted = new SimpleProviderPropertyDefinition("WhenSyncStarted", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WhenLastRecipientCookieCommitted = new SimpleProviderPropertyDefinition("WhenLastRecipientCookieCommitted", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WhenLastCompanyCookieCommitted = new SimpleProviderPropertyDefinition("WhenLastCompanyCookieCommitted", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExternalDirectoryOrganizationId = new SimpleProviderPropertyDefinition("ExternalDirectoryOrganizationId", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ServiceInstanceId = new SimpleProviderPropertyDefinition("ServiceInstanceId", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
