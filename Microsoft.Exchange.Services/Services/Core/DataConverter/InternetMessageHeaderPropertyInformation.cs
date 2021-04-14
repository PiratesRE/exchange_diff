using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class InternetMessageHeaderPropertyInformation : PropertyInformation
	{
		public InternetMessageHeaderPropertyInformation() : base("InternetMessageHeaders", null, ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007, null, new DictionaryPropertyUriBase(DictionaryUriEnum.InternetMessageHeader), new PropertyCommand.CreatePropertyCommand(InternetMessageHeadersProperty.CreateCommand), true)
		{
		}

		public override PropertyDefinition[] GetPropertyDefinitions(CommandSettings commandSettings)
		{
			DictionaryPropertyUri dictionaryPropertyUri = null;
			ToXmlCommandSettingsBase toXmlCommandSettingsBase = commandSettings as ToXmlCommandSettingsBase;
			if (toXmlCommandSettingsBase != null)
			{
				dictionaryPropertyUri = (DictionaryPropertyUri)toXmlCommandSettingsBase.PropertyPath;
			}
			else
			{
				ToServiceObjectCommandSettings toServiceObjectCommandSettings = commandSettings as ToServiceObjectCommandSettings;
				if (toServiceObjectCommandSettings != null)
				{
					dictionaryPropertyUri = (DictionaryPropertyUri)toServiceObjectCommandSettings.PropertyPath;
				}
			}
			if (dictionaryPropertyUri == null)
			{
				return null;
			}
			if (ExchangeVersion.Current == ExchangeVersion.Exchange2007)
			{
				return new PropertyDefinition[]
				{
					MessageItemSchema.TransportMessageHeaders,
					PropertyDefinitionHelper.GenerateInternetHeaderPropertyDefinition(dictionaryPropertyUri.Key)
				};
			}
			return new PropertyDefinition[]
			{
				MessageItemSchema.TransportMessageHeaders
			};
		}
	}
}
