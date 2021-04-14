using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class InstanceKeyProperty : PropertyCommand, IToXmlForPropertyBagCommand, IToServiceObjectForPropertyBagCommand, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		private InstanceKeyProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static InstanceKeyProperty CreateCommand(CommandContext commandContext)
		{
			return new InstanceKeyProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("IntanceKeyProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			PropertyDefinition[] propertyDefinitions = this.commandContext.GetPropertyDefinitions();
			if (PropertyCommand.StorePropertyExists(storeObject, propertyDefinitions[0]))
			{
				byte[] value = (byte[])storeObject[ItemSchema.InstanceKey];
				commandSettings.ServiceObject[ItemSchema.InstanceKey] = value;
				return;
			}
			object obj;
			try
			{
				obj = storeObject.TryGetProperty(ItemSchema.Fid);
			}
			catch (NotInBagPropertyErrorException)
			{
				obj = null;
			}
			object obj2;
			try
			{
				obj2 = storeObject.TryGetProperty(MessageItemSchema.MID);
			}
			catch (NotInBagPropertyErrorException)
			{
				obj2 = null;
			}
			if (!(obj is PropertyError) && obj != null && !(obj2 is PropertyError) && obj2 != null)
			{
				byte[] bytes = BitConverter.GetBytes((long)obj);
				byte[] bytes2 = BitConverter.GetBytes((long)obj2);
				int value2 = 0;
				byte[] array = new byte[bytes.Length + bytes2.Length + BitConverter.GetBytes(value2).Length];
				Array.Copy(bytes, 0, array, 0, bytes.Length);
				Array.Copy(bytes2, 0, array, bytes.Length, bytes2.Length);
				serviceObject[ItemSchema.InstanceKey] = array;
			}
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("InstanceKeyProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			byte[] value;
			if (PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, ItemSchema.InstanceKey, out value))
			{
				serviceObject.PropertyBag[propertyInformation] = value;
			}
		}
	}
}
