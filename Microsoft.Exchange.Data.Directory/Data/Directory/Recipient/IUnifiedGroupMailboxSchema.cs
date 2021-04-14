using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal abstract class IUnifiedGroupMailboxSchema
	{
		public static readonly ADPropertyDefinition UnifiedGroupMembersLink = new ADPropertyDefinition("UnifiedGroupMembersLink", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), "msExchUGMemberLink", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UnifiedGroupMembersBL = new ADPropertyDefinition("UnifiedGroupMembersBL", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), "msExchUGMemberBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
