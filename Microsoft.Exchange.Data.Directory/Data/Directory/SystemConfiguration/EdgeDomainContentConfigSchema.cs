using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class EdgeDomainContentConfigSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition DomainName = new ADPropertyDefinition("DomainName", ExchangeObjectVersion.Exchange2003, typeof(SmtpDomainWithSubdomains), "domainName", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ADNonMimeCharacterSet = new ADPropertyDefinition("ADNonMimeCharacterSet", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchNonMIMECharacterSet", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Flags = new ADPropertyDefinition("RemoteDomainFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchDomainContentConfigFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NonMimeCharacterSet = new ADPropertyDefinition("NonMimeCharacterSet", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			EdgeDomainContentConfigSchema.ADNonMimeCharacterSet
		}, null, new GetterDelegate(DomainContentConfig.NonMimeCharacterSetGetter), new SetterDelegate(DomainContentConfig.NonMimeCharacterSetSetter), null, null);

		public static readonly ADPropertyDefinition TrustedMailOutboundEnabled = new ADPropertyDefinition("TrustedMailOutboundEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			EdgeDomainContentConfigSchema.Flags
		}, null, ADObject.FlagGetterDelegate(EdgeDomainContentConfigSchema.Flags, 1), ADObject.FlagSetterDelegate(EdgeDomainContentConfigSchema.Flags, 1), null, null);

		public static readonly ADPropertyDefinition TrustedMailInboundEnabled = new ADPropertyDefinition("TrustedMailInboundEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			EdgeDomainContentConfigSchema.Flags
		}, null, ADObject.FlagGetterDelegate(EdgeDomainContentConfigSchema.Flags, 2), ADObject.FlagSetterDelegate(EdgeDomainContentConfigSchema.Flags, 2), null, null);
	}
}
