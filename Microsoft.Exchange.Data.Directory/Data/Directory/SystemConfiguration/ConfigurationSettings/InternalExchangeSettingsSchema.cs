using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	internal class InternalExchangeSettingsSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<SettingsXml>(InternalExchangeSettingsSchema.ConfigurationXMLRaw);
	}
}
