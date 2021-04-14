using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class OutlookProviderSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition CertPrincipalName = new ADPropertyDefinition("CertPrincipalName", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAutoDiscoverCertPrincipalName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Server = new ADPropertyDefinition("Server", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAutoDiscoverServer", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TTL = new ADPropertyDefinition("TTL", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchAutoDiscoverTTL", ADPropertyDefinitionFlags.PersistDefaultValue, 1, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(-1, 65536)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Flags = new ADPropertyDefinition("Flags", ExchangeObjectVersion.Exchange2007, typeof(OutlookProviderFlags), "msExchAutoDiscoverFlags", ADPropertyDefinitionFlags.None, OutlookProviderFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<OutlookProviderConfigXML>(OutlookProviderSchema.ConfigurationXMLRaw);

		public static readonly ADPropertyDefinition RequiredClientVersions = XMLSerializableBase.ConfigXmlProperty<OutlookProviderConfigXML, ClientVersionCollection>("RequiredClientVersions", ExchangeObjectVersion.Exchange2003, OutlookProviderSchema.ConfigurationXML, null, (OutlookProviderConfigXML configXml) => configXml.RequiredClientVersions, delegate(OutlookProviderConfigXML configXml, ClientVersionCollection value)
		{
			configXml.RequiredClientVersions = value;
		}, null, null);
	}
}
