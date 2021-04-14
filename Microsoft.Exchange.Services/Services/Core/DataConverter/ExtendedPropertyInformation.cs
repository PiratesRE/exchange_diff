using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ExtendedPropertyInformation : PropertyInformation
	{
		public ExtendedPropertyInformation() : base("ExtendedProperty", ServiceXml.GetFullyQualifiedName("ExtendedProperty"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007, null, ExtendedPropertyUri.Placeholder, new PropertyCommand.CreatePropertyCommand(ExtendedPropertyProperty.CreateCommand), true)
		{
		}

		public override PropertyDefinition[] GetPropertyDefinitions(CommandSettings commandSettings)
		{
			ExtendedPropertyUri extendedPropertyUri = null;
			ToXmlCommandSettingsBase toXmlCommandSettingsBase = commandSettings as ToXmlCommandSettingsBase;
			if (toXmlCommandSettingsBase != null)
			{
				extendedPropertyUri = (ExtendedPropertyUri)toXmlCommandSettingsBase.PropertyPath;
			}
			else
			{
				ToServiceObjectCommandSettingsBase toServiceObjectCommandSettingsBase = commandSettings as ToServiceObjectCommandSettingsBase;
				if (toServiceObjectCommandSettingsBase != null)
				{
					extendedPropertyUri = (ExtendedPropertyUri)toServiceObjectCommandSettingsBase.PropertyPath;
				}
				else
				{
					SetCommandSettings setCommandSettings = commandSettings as SetCommandSettings;
					if (setCommandSettings != null)
					{
						if (setCommandSettings.ServiceObject != null)
						{
							PropertyInformation propertyInfo = (setCommandSettings.ServiceObject is BaseFolderType) ? BaseFolderSchema.ExtendedProperty : ItemSchema.ExtendedProperty;
							ExtendedPropertyType[] valueOrDefault = setCommandSettings.ServiceObject.GetValueOrDefault<ExtendedPropertyType[]>(propertyInfo);
							if (valueOrDefault != null)
							{
								PropertyDefinition[] array = new PropertyDefinition[valueOrDefault.Length];
								for (int i = 0; i < valueOrDefault.Length; i++)
								{
									array[i] = valueOrDefault[i].ExtendedFieldURI.ToPropertyDefinition();
								}
								return array;
							}
						}
						else
						{
							extendedPropertyUri = this.GetPropertyUri(setCommandSettings);
						}
					}
					else
					{
						UpdateCommandSettings updateCommandSettings = commandSettings as UpdateCommandSettings;
						extendedPropertyUri = (ExtendedPropertyUri)updateCommandSettings.PropertyUpdate.PropertyPath;
					}
				}
			}
			return new PropertyDefinition[]
			{
				extendedPropertyUri.ToPropertyDefinition()
			};
		}

		private ExtendedPropertyUri GetPropertyUri(SetCommandSettings setCommandSettings)
		{
			return ExtendedPropertyUri.Parse(setCommandSettings.ServiceProperty["ExtendedFieldURI", "http://schemas.microsoft.com/exchange/services/2006/types"]);
		}
	}
}
