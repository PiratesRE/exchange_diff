using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	internal class SettingOverrideSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition ConfigurationXmlRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXml = XMLSerializableBase.ConfigurationXmlProperty<SettingOverrideXml>(SettingOverrideSchema.ConfigurationXmlRaw);
	}
}
