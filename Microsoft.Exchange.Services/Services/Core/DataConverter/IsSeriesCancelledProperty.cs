using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class IsSeriesCancelledProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		private IsSeriesCancelledProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static IsSeriesCancelledProperty CreateCommand(CommandContext commandContext)
		{
			return new IsSeriesCancelledProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("IsSeriesCancelledProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("IsSeriesCancelledProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			CalendarItemBase calendarItemBase = commandSettings.StoreObject as CalendarItemBase;
			if (calendarItemBase != null && calendarItemBase.CalendarItemType != CalendarItemType.Single)
			{
				serviceObject[propertyInformation] = calendarItemBase.GetValueOrDefault<bool>(CalendarItemOccurrenceSchema.IsSeriesCancelled);
			}
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			bool flag;
			if (PropertyCommand.TryGetValueFromPropertyBag<bool>(propertyBag, CalendarItemOccurrenceSchema.IsSeriesCancelled, out flag))
			{
				serviceObject[propertyInformation] = flag;
			}
		}
	}
}
