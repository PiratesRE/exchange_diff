using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class SchemaContainerSchema : ADNonExchangeObjectSchema
	{
		public static readonly ADPropertyDefinition FsmoRoleOwner = new ADPropertyDefinition("FsmoRoleOwner", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "fSMORoleOwner", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
