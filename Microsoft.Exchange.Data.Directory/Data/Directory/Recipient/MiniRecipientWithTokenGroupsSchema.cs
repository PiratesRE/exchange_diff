using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class MiniRecipientWithTokenGroupsSchema : MiniRecipientSchema
	{
		public static readonly ADPropertyDefinition TokenGroupsGlobalAndUniversal = new ADPropertyDefinition("tokenGroupsGlobalAndUniversal", ExchangeObjectVersion.Exchange2003, typeof(SecurityIdentifier), "tokenGroupsGlobalAndUniversal", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
