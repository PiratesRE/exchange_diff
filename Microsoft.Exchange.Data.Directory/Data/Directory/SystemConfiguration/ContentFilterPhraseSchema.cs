using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ContentFilterPhraseSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Phrase = new SimpleProviderPropertyDefinition("Phrase", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Influence = new SimpleProviderPropertyDefinition("Influence", ExchangeObjectVersion.Exchange2007, typeof(Influence), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.SystemConfiguration.Influence.GoodWord, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
