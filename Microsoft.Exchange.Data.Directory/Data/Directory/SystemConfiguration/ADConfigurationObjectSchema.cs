using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal abstract class ADConfigurationObjectSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition AdminDisplayName = new ADPropertyDefinition("AdminDisplayName", ExchangeObjectVersion.Exchange2003, typeof(string), "adminDisplayName", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.AdminDisplayName);

		public static readonly ADPropertyDefinition SystemFlags = new ADPropertyDefinition("SystemFlags", ExchangeObjectVersion.Exchange2003, typeof(SystemFlagsEnum), "systemFlags", ADPropertyDefinitionFlags.WriteOnce, SystemFlagsEnum.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
