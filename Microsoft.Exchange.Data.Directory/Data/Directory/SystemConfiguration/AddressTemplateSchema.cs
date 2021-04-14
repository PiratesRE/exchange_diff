using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AddressTemplateSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition DisplayName = SharedPropertyDefinitions.MandatoryDisplayName;

		public static readonly ADPropertyDefinition AddressSyntax = new ADPropertyDefinition("AddressSyntax", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "addressSyntax", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ByteArrayLengthConstraint(1, 4096)
		}, null, null);

		public static readonly ADPropertyDefinition AddressType = new ADPropertyDefinition("AddressType", ExchangeObjectVersion.Exchange2003, typeof(string), "addressType", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PerMsgDialogDisplayTable = new ADPropertyDefinition("PerMsgDialogDisplayTable", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "perMsgDialogDisplayTable", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ByteArrayLengthConstraint(1, 32768)
		}, null, null);

		public static readonly ADPropertyDefinition PerRecipDialogDisplayTable = new ADPropertyDefinition("PerRecipDialogDisplayTable", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "perRecipDialogDisplayTable", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ByteArrayLengthConstraint(1, 32768)
		}, null, null);

		public static readonly ADPropertyDefinition ProxyGenerationEnabled = new ADPropertyDefinition("ProxyGenerationEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "proxyGenerationEnabled", ADPropertyDefinitionFlags.Binary, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TemplateBlob = DetailsTemplateSchema.TemplateBlob;

		public static readonly ADPropertyDefinition ExchangeLegacyDN = SharedPropertyDefinitions.ExchangeLegacyDN;
	}
}
