using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class FullSyncObjectRequestSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition ServiceInstanceId = new SimpleProviderPropertyDefinition("ServiceInstanceId", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Options = new SimpleProviderPropertyDefinition("Options", ExchangeObjectVersion.Exchange2010, typeof(FullSyncObjectRequestOptions), PropertyDefinitionFlags.None, FullSyncObjectRequestOptions.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition State = new SimpleProviderPropertyDefinition("State", ExchangeObjectVersion.Exchange2010, typeof(FullSyncObjectRequestState), PropertyDefinitionFlags.None, FullSyncObjectRequestState.New, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CreationTime = new SimpleProviderPropertyDefinition("CreationTime", ExchangeObjectVersion.Exchange2010, typeof(ExDateTime), PropertyDefinitionFlags.None, ExDateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
