using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DetailsTemplateSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition TemplateBlob = new ADPropertyDefinition("TemplateBlob", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "addressEntryDisplayTable", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TemplateBlobOriginal = new ADPropertyDefinition("TemplateBlobOriginal", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "originalDisplayTable", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HelpFileName = new ADPropertyDefinition("HelpFileName", ExchangeObjectVersion.Exchange2003, typeof(string), "helpFileName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HelpData32 = new ADPropertyDefinition("HelpData32", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "helpData32", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Pages = new ADPropertyDefinition("Pages", ExchangeObjectVersion.Exchange2003, typeof(Page), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ExchangeLegacyDN = SharedPropertyDefinitions.ExchangeLegacyDN;
	}
}
