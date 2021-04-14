using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class IsClutterProperty : ComplexPropertyBase, IToServiceObjectCommand, IToXmlForPropertyBagCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public IsClutterProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static IsClutterProperty CreateCommand(CommandContext commandContext)
		{
			return new IsClutterProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("IsClutterProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("IsClutterProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			bool flag;
			if (PropertyCommand.TryGetValueFromPropertyBag<bool>(commandSettings.PropertyBag, ItemSchema.IsClutter, out flag))
			{
				serviceObject.PropertyBag[this.commandContext.PropertyInformation] = flag;
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ItemResponseShape itemResponseShape = commandSettings.ResponseShape as ItemResponseShape;
			if (!itemResponseShape.InferenceEnabled)
			{
				return;
			}
			StoreObject storeObject = commandSettings.StoreObject;
			if (PropertyCommand.StorePropertyExists(storeObject, ItemSchema.IsClutter))
			{
				object propertyValueFromStoreObject = PropertyCommand.GetPropertyValueFromStoreObject(storeObject, ItemSchema.IsClutter);
				if (propertyValueFromStoreObject != null)
				{
					bool flag = IsClutterProperty.ConvertStoreValueToServiceValue(propertyValueFromStoreObject);
					ServiceObject serviceObject = commandSettings.ServiceObject;
					PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
					serviceObject.PropertyBag[propertyInformation] = flag;
				}
			}
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			this.SetClutterFlag(commandSettings.ServiceObject, commandSettings.StoreObject);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			this.SetClutterFlag(setPropertyUpdate.ServiceObject, updateCommandSettings.StoreObject);
		}

		internal static bool GetFlagValueOrDefaultFromStorePropertyBag(IStorePropertyBag storePropertyBag, ItemResponseShape responseShape)
		{
			if (responseShape.InferenceEnabled)
			{
				object obj = storePropertyBag.TryGetProperty(ItemSchema.IsClutter);
				if (obj != null)
				{
					return IsClutterProperty.ConvertStoreValueToServiceValue(obj);
				}
			}
			return false;
		}

		private static bool ConvertStoreValueToServiceValue(object storeValue)
		{
			return storeValue is bool && (bool)storeValue;
		}

		private void SetClutterFlag(ServiceObject serviceObject, StoreObject storeObject)
		{
			bool valueOrDefault = serviceObject.GetValueOrDefault<bool>(this.commandContext.PropertyInformation);
			base.SetPropertyValueOnStoreObject(storeObject, ItemSchema.IsClutter, valueOrDefault);
			base.SetPropertyValueOnStoreObject(storeObject, MessageItemSchema.IsClutterOverridden, true);
		}
	}
}
