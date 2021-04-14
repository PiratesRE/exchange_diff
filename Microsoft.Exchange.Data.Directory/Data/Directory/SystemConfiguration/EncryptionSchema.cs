using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class EncryptionSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition DefaultMessageFormat = new ADPropertyDefinition("DefaultMessageFormat", ExchangeObjectVersion.Exchange2003, typeof(bool), "defaultMessageFormat", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EncryptAlgListNA = new ADPropertyDefinition("EncryptAlgListNA", ExchangeObjectVersion.Exchange2003, typeof(string), "encryptAlgListNA", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EncryptAlgListOther = new ADPropertyDefinition("EncryptAlgListOther", ExchangeObjectVersion.Exchange2003, typeof(string), "encryptAlgListOther", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EncryptAlgSelectedNA = new ADPropertyDefinition("EncryptAlgSelectedNA", ExchangeObjectVersion.Exchange2003, typeof(string), "encryptAlgSelectedNA", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EncryptAlgSelectedOther = new ADPropertyDefinition("EncryptAlgSelectedOther", ExchangeObjectVersion.Exchange2003, typeof(string), "encryptAlgSelectedOther", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SMimeAlgListNA = new ADPropertyDefinition("SMimeAlgListNA", ExchangeObjectVersion.Exchange2003, typeof(string), "sMIMEAlgListNA", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SMimeAlgListOther = new ADPropertyDefinition("SMimeAlgListOther", ExchangeObjectVersion.Exchange2003, typeof(string), "sMIMEAlgListOther", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SMimeAlgSelectedNA = new ADPropertyDefinition("SMimeAlgSelectedNA", ExchangeObjectVersion.Exchange2003, typeof(string), "sMIMEAlgSelectedNA", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SMimeAlgSelectedOther = new ADPropertyDefinition("SMimeAlgSelectedOther", ExchangeObjectVersion.Exchange2003, typeof(string), "sMIMEAlgSelectedOther", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LegacyExchangeDN = SharedPropertyDefinitions.LegacyExchangeDN;
	}
}
