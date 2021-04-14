using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class StoreEntryIdProperty : PropertyCommand, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		private StoreEntryIdProperty(CommandContext commandContext) : base(commandContext)
		{
			this.propertyConverter = BaseConverter.GetConverterForPropertyDefinition(commandContext.GetPropertyDefinitions()[0]);
		}

		public static StoreEntryIdProperty CreateCommand(CommandContext commandContext)
		{
			return new StoreEntryIdProperty(commandContext);
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			byte[] idFromExrpc = PropertyCommand.GetPropertyValueFromStoreObject(storeObject, StoreObjectSchema.MapiStoreEntryId) as byte[];
			serviceObject.PropertyBag[propertyInformation] = StoreEntryId.WrapStoreId(idFromExrpc);
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			byte[] idFromExrpc;
			if (PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, StoreObjectSchema.MapiStoreEntryId, out idFromExrpc))
			{
				serviceObject.PropertyBag[propertyInformation] = StoreEntryId.WrapStoreId(idFromExrpc);
			}
		}

		private void WriteXml(byte[] storeEntryIdValue, XmlElement serviceItem)
		{
			string text = this.propertyConverter.ConvertToString(storeEntryIdValue);
			if (text != null)
			{
				base.CreateXmlTextElement(serviceItem, this.xmlLocalName, text);
			}
		}

		public void ToXml()
		{
			ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			XmlElement serviceItem = commandSettings.ServiceItem;
			byte[] idFromExrpc = PropertyCommand.GetPropertyValueFromStoreObject(storeObject, StoreObjectSchema.MapiStoreEntryId) as byte[];
			byte[] storeEntryIdValue = StoreEntryId.WrapStoreId(idFromExrpc);
			this.WriteXml(storeEntryIdValue, serviceItem);
		}

		public void ToXmlForPropertyBag()
		{
			ToXmlForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToXmlForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			XmlElement serviceItem = commandSettings.ServiceItem;
			byte[] idFromExrpc = null;
			if (PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, StoreObjectSchema.MapiStoreEntryId, out idFromExrpc))
			{
				byte[] storeEntryIdValue = StoreEntryId.WrapStoreId(idFromExrpc);
				this.WriteXml(storeEntryIdValue, serviceItem);
			}
		}

		private BaseConverter propertyConverter;
	}
}
