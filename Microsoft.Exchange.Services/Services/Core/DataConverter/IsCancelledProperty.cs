using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class IsCancelledProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		private IsCancelledProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static IsCancelledProperty CreateCommand(CommandContext commandContext)
		{
			return new IsCancelledProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("IsCancelledProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("IsCancelledProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			CalendarItemBase calendarItemBase = commandSettings.StoreObject as CalendarItemBase;
			bool isCancelled;
			if (calendarItemBase == null)
			{
				calendarItemBase = ((MeetingRequest)commandSettings.StoreObject).GetCachedEmbeddedItem();
				isCancelled = calendarItemBase.IsCancelled;
			}
			else
			{
				isCancelled = calendarItemBase.IsCancelled;
			}
			serviceObject[propertyInformation] = isCancelled;
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			int num;
			if (PropertyCommand.TryGetValueFromPropertyBag<int>(propertyBag, CalendarItemBaseSchema.AppointmentState, out num))
			{
				bool flag = (num & 4) != 0;
				serviceObject[propertyInformation] = flag;
			}
		}
	}
}
