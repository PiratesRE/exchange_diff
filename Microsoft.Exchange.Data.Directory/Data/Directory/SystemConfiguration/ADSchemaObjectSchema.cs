using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADSchemaObjectSchema : ADNonExchangeObjectSchema
	{
		public static readonly ADPropertyDefinition DisplayName = new ADPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2003, typeof(string), "ldapDisplayName", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SchemaIDGuid = new ADPropertyDefinition("SchemaIDGuid", ExchangeObjectVersion.Exchange2003, typeof(Guid), "SchemaIDGuid", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MayContain = new ADPropertyDefinition("MayContain", ExchangeObjectVersion.Exchange2003, typeof(string), "mayContain", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SystemMayContain = new ADPropertyDefinition("SystemMayContain", ExchangeObjectVersion.Exchange2003, typeof(string), "systemMayContain", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MustContain = new ADPropertyDefinition("MustContain", ExchangeObjectVersion.Exchange2003, typeof(string), "mustContain", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SystemMustContain = new ADPropertyDefinition("SystemMustContain", ExchangeObjectVersion.Exchange2003, typeof(string), "systemMustContain", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultObjectCategory = new ADPropertyDefinition("DefaultObjectCategory", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "DefaultObjectCategory", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
