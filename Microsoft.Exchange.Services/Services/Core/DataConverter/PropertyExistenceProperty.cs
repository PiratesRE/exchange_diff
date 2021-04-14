using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PropertyExistenceProperty : ComplexPropertyBase, IToServiceObjectCommand, IPropertyCommand
	{
		public PropertyExistenceProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static PropertyExistenceProperty CreateCommand(CommandContext commandContext)
		{
			return new PropertyExistenceProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("PropertyExistenceProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			serviceObject.PropertyBag[propertyInformation] = PropertyExistenceProperty.GetValue((PropertyDefinition propDef) => storeObject.GetValueOrDefault<bool>(propDef, false));
		}

		internal static PropertyExistenceType GetValueFromStorePropertyBag(IStorePropertyBag storePropertyBag)
		{
			return PropertyExistenceProperty.GetValue((PropertyDefinition propDef) => storePropertyBag.GetValueOrDefault<bool>(propDef, false));
		}

		private static PropertyExistenceType GetValue(Func<PropertyDefinition, bool> getterFunc)
		{
			PropertyExistenceType propertyExistenceType = new PropertyExistenceType();
			bool flag = false;
			foreach (KeyValuePair<PropertyDefinition, Action<PropertyExistenceType>> keyValuePair in PropertyExistenceProperty.propSetterMap)
			{
				if (getterFunc(keyValuePair.Key))
				{
					keyValuePair.Value(propertyExistenceType);
					flag = true;
				}
			}
			if (!flag)
			{
				return null;
			}
			return propertyExistenceType;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static PropertyExistenceProperty()
		{
			Dictionary<PropertyDefinition, Action<PropertyExistenceType>> dictionary = new Dictionary<PropertyDefinition, Action<PropertyExistenceType>>();
			dictionary.Add(ItemSchema.ExtractedMeetingsExists, delegate(PropertyExistenceType p)
			{
				p.ExtractedMeetings = true;
			});
			dictionary.Add(ItemSchema.ExtractedTasksExists, delegate(PropertyExistenceType p)
			{
				p.ExtractedTasks = true;
			});
			dictionary.Add(ItemSchema.ExtractedAddressesExists, delegate(PropertyExistenceType p)
			{
				p.ExtractedAddresses = true;
			});
			dictionary.Add(ItemSchema.ExtractedKeywordsExists, delegate(PropertyExistenceType p)
			{
				p.ExtractedKeywords = true;
			});
			dictionary.Add(ItemSchema.ExtractedUrlsExists, delegate(PropertyExistenceType p)
			{
				p.ExtractedUrls = true;
			});
			dictionary.Add(ItemSchema.ExtractedPhonesExists, delegate(PropertyExistenceType p)
			{
				p.ExtractedPhones = true;
			});
			dictionary.Add(ItemSchema.ExtractedEmailsExists, delegate(PropertyExistenceType p)
			{
				p.ExtractedEmails = true;
			});
			dictionary.Add(ItemSchema.ExtractedContactsExists, delegate(PropertyExistenceType p)
			{
				p.ExtractedContacts = true;
			});
			dictionary.Add(MessageItemSchema.ReplyToNamesExists, delegate(PropertyExistenceType p)
			{
				p.ReplyToNames = true;
			});
			dictionary.Add(MessageItemSchema.ReplyToBlobExists, delegate(PropertyExistenceType p)
			{
				p.ReplyToBlob = true;
			});
			PropertyExistenceProperty.propSetterMap = dictionary;
		}

		private static Dictionary<PropertyDefinition, Action<PropertyExistenceType>> propSetterMap;
	}
}
