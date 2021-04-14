using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class RidManagerContainerSchema : ADNonExchangeObjectSchema
	{
		public static readonly ADPropertyDefinition FsmoRoleOwner = new ADPropertyDefinition("FsmoRoleOwner", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "fSMORoleOwner", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplicationAttributeMetadata = new ADPropertyDefinition("ReplicationAttributeMetadata", ExchangeObjectVersion.Exchange2003, typeof(string), "msDS-ReplAttributeMetadata", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
