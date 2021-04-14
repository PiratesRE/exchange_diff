using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class SystemMessageSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition Text = new ADPropertyDefinition("Text", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchDSNText", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Internal = new ADPropertyDefinition("Internal", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(SystemMessage.InternalGetter), null, null, null);

		public static readonly ADPropertyDefinition Language = new ADPropertyDefinition("Language", ExchangeObjectVersion.Exchange2007, typeof(CultureInfo), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(SystemMessage.LanguageGetter), null, null, null);

		public static readonly ADPropertyDefinition DsnCode = new ADPropertyDefinition("DsnCode", ExchangeObjectVersion.Exchange2007, typeof(EnhancedStatusCode), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, EnhancedStatusCode.Parse("5.0.0"), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(SystemMessage.CodeGetter), null, null, null);

		public static readonly ADPropertyDefinition QuotaMessageType = new ADPropertyDefinition("QuotaMessageType", ExchangeObjectVersion.Exchange2007, typeof(QuotaMessageType?), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(SystemMessage.QuotaMessageTypeGetter), null, null, null);
	}
}
