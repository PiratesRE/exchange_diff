using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	internal class MsoRawObjectSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition SyncObjectData = new SimpleProviderPropertyDefinition("SyncObjectData", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExternalObjectId = new SimpleProviderPropertyDefinition("ExternalObjectId", ExchangeObjectVersion.Exchange2010, typeof(SyncObjectId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ServiceInstanceId = new SimpleProviderPropertyDefinition("ServiceInstanceId", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ObjectAndLinks = new SimpleProviderPropertyDefinition("ObjectAndLinks", ExchangeObjectVersion.Exchange2010, typeof(DirectoryObjectsAndLinks), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SerializedObjectAndLinks = new SimpleProviderPropertyDefinition("SerializedObjectAndLinks", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DisplayName = new SimpleProviderPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WindowsLiveNetId = new SimpleProviderPropertyDefinition("WindowsLiveNetId", ExchangeObjectVersion.Exchange2010, typeof(NetID), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AllLinksCollected = new SimpleProviderPropertyDefinition("AllLinksCollected", ExchangeObjectVersion.Exchange2010, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition LinksCollected = new SimpleProviderPropertyDefinition("LinksCollected", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AssignedPlanCapabilities = new SimpleProviderPropertyDefinition("AssignedPlanCapabilities", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExchangeValidationError = new SimpleProviderPropertyDefinition("ExchangeValidationError", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
