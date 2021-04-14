using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	internal sealed class ExchangeSettingsSchema : InternalExchangeSettingsSchema
	{
		public static readonly ADPropertyDefinition History = XMLSerializableBase.ConfigXmlProperty<SettingsXml, XMLSerializableDictionary<string, SettingsHistory>>("SettingsHistory", ExchangeObjectVersion.Exchange2007, InternalExchangeSettingsSchema.ConfigurationXML, null, (SettingsXml configXml) => configXml.History.Value, null, null, null);
	}
}
