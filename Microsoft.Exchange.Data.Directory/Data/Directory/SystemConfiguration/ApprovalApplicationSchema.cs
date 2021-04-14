using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ApprovalApplicationSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition ArbitrationMailboxesBacklink = new ADPropertyDefinition("ArbitrationMailboxesBacklink", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchArbitrationMailboxesBL", ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ELCRetentionPolicyTag = new ADPropertyDefinition("ELCRetentionPolicyTag", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchRetentionPolicyTag", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
