using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class IsAllDayEventProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		private IsAllDayEventProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static IsAllDayEventProperty CreateCommand(CommandContext commandContext)
		{
			return new IsAllDayEventProperty(commandContext);
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			bool value = (bool)commandSettings.ServiceObject[this.commandContext.PropertyInformation];
			this.SetProperty(commandSettings.StoreObject, value);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			bool value = (bool)setPropertyUpdate.ServiceObject[this.commandContext.PropertyInformation];
			StoreObject storeObject = updateCommandSettings.StoreObject;
			this.SetProperty(storeObject, value);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("IsAllDayEventProperty.ToXml should not be called");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("IsAllDayEventProperty.ToXmlForPropertyBag should not be called");
		}

		private void SetProperty(StoreObject storeObject, bool value)
		{
			storeObject[IsAllDayEventProperty.StorageIsAllDayEventPropDef] = value;
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			if (PropertyCommand.StorePropertyExists(storeObject, IsAllDayEventProperty.MapiIsAllDayEventPropDef))
			{
				bool flag = (bool)storeObject[IsAllDayEventProperty.MapiIsAllDayEventPropDef];
				serviceObject[propertyInformation] = flag;
			}
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			bool flag = false;
			if (PropertyCommand.TryGetValueFromPropertyBag<bool>(propertyBag, IsAllDayEventProperty.MapiIsAllDayEventPropDef, out flag))
			{
				serviceObject[propertyInformation] = flag;
			}
		}

		private static readonly PropertyDefinition MapiIsAllDayEventPropDef = CalendarItemBaseSchema.MapiIsAllDayEvent;

		private static readonly PropertyDefinition StorageIsAllDayEventPropDef = CalendarItemBaseSchema.IsAllDayEvent;
	}
}
