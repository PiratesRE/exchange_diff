using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ExtendedPropertyProperty : ComplexPropertyBase, IDeleteUpdateCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		public ExtendedPropertyProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ExtendedPropertyProperty CreateCommand(CommandContext commandContext)
		{
			return new ExtendedPropertyProperty(commandContext);
		}

		public static ExtendedPropertyType GetExtendedPropertyForValues(ExtendedPropertyUri propertyUri, PropertyDefinition propertyDefinition, object values)
		{
			ExtendedPropertyType result = null;
			BaseConverter converter;
			if (BaseConverter.TryGetConverterForPropertyDefinition(propertyDefinition, out converter))
			{
				if (ExtendedPropertyProperty.IsArrayType(propertyDefinition.Type))
				{
					Array array = (Array)values;
					int length = array.GetLength(0);
					string[] array2 = new string[length];
					for (int i = 0; i < array.GetLength(0); i++)
					{
						array2[i] = ExtendedPropertyProperty.ConvertToStringValue(array.GetValue(i), converter);
					}
					result = new ExtendedPropertyType(propertyUri, array2);
				}
				else
				{
					string value = ExtendedPropertyProperty.ConvertToStringValue(values, converter);
					result = new ExtendedPropertyType(propertyUri, value);
				}
			}
			return result;
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			PropertyDefinition propertyDefinition = ((ExtendedPropertyUri)deletePropertyUpdate.PropertyPath).ToPropertyDefinition();
			base.DeleteProperties(storeObject, deletePropertyUpdate.PropertyPath, new PropertyDefinition[]
			{
				propertyDefinition
			});
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			this.SetProperty(serviceObject, storeObject);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
			this.SetProperty(serviceObject, storeObject);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("ExtendedPropertyProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			ExtendedPropertyUri extendedPropertyUri = (ExtendedPropertyUri)commandSettings.PropertyPath;
			if (PropertyCommand.StorePropertyExists(storeObject, this.propertyDefinitions[0]))
			{
				object values = this.RenderValuesFromStoreObject(storeObject, commandSettings.IdAndSession, extendedPropertyUri);
				ExtendedPropertyType extendedPropertyForValues = ExtendedPropertyProperty.GetExtendedPropertyForValues(extendedPropertyUri, this.propertyDefinitions[0], values);
				if (extendedPropertyForValues != null)
				{
					serviceObject.AddExtendedPropertyValue(extendedPropertyForValues);
				}
			}
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("ExtendedPropertyProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			ExtendedPropertyUri extendedPropertyUri = (ExtendedPropertyUri)commandSettings.PropertyPath;
			NativeStorePropertyDefinition nativeStorePropertyDefinition = (NativeStorePropertyDefinition)this.propertyDefinitions[0];
			object obj;
			if (PropertyCommand.TryGetValueFromPropertyBag<object>(propertyBag, nativeStorePropertyDefinition, out obj))
			{
				object values = ExtendedPropertyProperty.RenderValuesFromPropertyBag(propertyBag, commandSettings.IdAndSession, nativeStorePropertyDefinition, extendedPropertyUri);
				ExtendedPropertyType extendedPropertyForValues = ExtendedPropertyProperty.GetExtendedPropertyForValues(extendedPropertyUri, this.propertyDefinitions[0], values);
				if (extendedPropertyForValues != null)
				{
					serviceObject.AddExtendedPropertyValue(extendedPropertyForValues);
				}
			}
		}

		private static object RenderValuesFromPropertyBag(IDictionary<PropertyDefinition, object> propertyBag, IdAndSession idAndSession, NativeStorePropertyDefinition propertyDefinition, ExtendedPropertyUri extendedPropertyUri)
		{
			PropertyInformation propertyInformation = ExtendedPropertyProperty.CreatePropertyInformationFromPropertyDefinition(propertyDefinition, extendedPropertyUri);
			ServiceObject serviceObject = new ItemType();
			ToServiceObjectForPropertyBagCommandSettings commandSettings = new ToServiceObjectForPropertyBagCommandSettings(extendedPropertyUri)
			{
				ServiceObject = serviceObject,
				PropertyBag = propertyBag,
				IdAndSession = idAndSession
			};
			CommandContext commandContext = new CommandContext(commandSettings, propertyInformation);
			IToServiceObjectForPropertyBagCommand toServiceObjectForPropertyBagCommand = propertyInformation.CreatePropertyCommand(commandContext) as IToServiceObjectForPropertyBagCommand;
			toServiceObjectForPropertyBagCommand.ToServiceObjectForPropertyBag();
			return serviceObject[propertyInformation];
		}

		private static bool IsArrayType(Type type)
		{
			return type.IsArray && type.GetElementType() != typeof(byte);
		}

		private static PropertyInformation CreatePropertyInformationFromPropertyDefinition(PropertyDefinition propertyDefinition, ExtendedPropertyUri extendedPropertyUri)
		{
			if (propertyDefinition.Equals(StoreObjectSchema.MapiStoreEntryId))
			{
				return new PropertyInformation("Value", ExchangeVersion.Exchange2010SP2, propertyDefinition, extendedPropertyUri, new PropertyCommand.CreatePropertyCommand(StoreEntryIdProperty.CreateCommand));
			}
			if (ExtendedPropertyProperty.IsArrayType(propertyDefinition.Type))
			{
				return new ArrayPropertyInformation("Values", ExchangeVersion.Exchange2007, "Value", propertyDefinition, extendedPropertyUri, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));
			}
			return new PropertyInformation("Value", ExchangeVersion.Exchange2007, propertyDefinition, extendedPropertyUri, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));
		}

		private static string ConvertToStringValue(object value, BaseConverter converter)
		{
			string text = value as string;
			if (text == null)
			{
				text = converter.ConvertToString(value);
			}
			return text;
		}

		private object RenderValuesFromStoreObject(StoreObject storeObject, IdAndSession idAndSession, ExtendedPropertyUri extendedPropertyUri)
		{
			NativeStorePropertyDefinition propertyDefinition = (NativeStorePropertyDefinition)this.propertyDefinitions[0];
			PropertyInformation propertyInformation = ExtendedPropertyProperty.CreatePropertyInformationFromPropertyDefinition(propertyDefinition, extendedPropertyUri);
			ServiceObject serviceObject = new ItemType();
			ToServiceObjectCommandSettings commandSettings = new ToServiceObjectCommandSettings(extendedPropertyUri)
			{
				ServiceObject = serviceObject,
				StoreObject = storeObject,
				IdAndSession = idAndSession
			};
			CommandContext commandContext = new CommandContext(commandSettings, propertyInformation);
			IToServiceObjectCommand toServiceObjectCommand = propertyInformation.CreatePropertyCommand(commandContext) as IToServiceObjectCommand;
			toServiceObjectCommand.ToServiceObject();
			return serviceObject[propertyInformation];
		}

		private void SetProperty(ServiceObject serviceObject, StoreObject storeObject)
		{
			ExtendedPropertyType[] valueOrDefault = serviceObject.GetValueOrDefault<ExtendedPropertyType[]>(this.commandContext.PropertyInformation);
			if (valueOrDefault != null)
			{
				for (int i = 0; i < valueOrDefault.Length; i++)
				{
					ExtendedPropertyType extendedPropertyType = valueOrDefault[i];
					ExtendedPropertyUri extendedFieldURI = extendedPropertyType.ExtendedFieldURI;
					NativeStorePropertyDefinition propertyDefinition = (NativeStorePropertyDefinition)this.propertyDefinitions[i];
					PropertyInformation propertyInformation = ExtendedPropertyProperty.CreatePropertyInformationFromPropertyDefinition(propertyDefinition, extendedFieldURI);
					ServiceObject serviceObject2 = new ItemType();
					if (extendedPropertyType.Values != null)
					{
						serviceObject2[propertyInformation] = extendedPropertyType.Values;
					}
					else
					{
						serviceObject2[propertyInformation] = extendedPropertyType.Value;
					}
					SetCommandSettings commandSettings = new SetCommandSettings(serviceObject2, storeObject);
					CommandContext commandContext = new CommandContext(commandSettings, propertyInformation);
					ISetCommand setCommand = propertyInformation.CreatePropertyCommand(commandContext) as ISetCommand;
					if (setCommand == null)
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceError<ExtendedPropertyUri>(0L, "[ExtendedPropertyProperty::SetProperty] PropertyCommand does not support set. extendedPropertyUri: {0}", extendedFieldURI);
						throw new ServiceInvalidOperationException((CoreResources.IDs)3890629732U);
					}
					try
					{
						setCommand.Set();
					}
					catch (FormatException innerException)
					{
						throw new InvalidExtendedPropertyValueException(extendedFieldURI, innerException);
					}
					catch (OverflowException innerException2)
					{
						throw new InvalidExtendedPropertyValueException(extendedFieldURI, innerException2);
					}
				}
			}
		}
	}
}
