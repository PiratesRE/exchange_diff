using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class OriginalStartProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		private OriginalStartProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static OriginalStartProperty CreateCommand(CommandContext commandContext)
		{
			return new OriginalStartProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("OriginalStartProperty.ToXml should not be called");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			MeetingRequest meetingRequest = storeObject as MeetingRequest;
			if (meetingRequest != null)
			{
				CalendarItemBase cachedEmbeddedItem = meetingRequest.GetCachedEmbeddedItem();
				CalendarItemOccurrence calendarItemOccurrence = cachedEmbeddedItem as CalendarItemOccurrence;
				this.ToServiceObject(serviceObject, calendarItemOccurrence);
				return;
			}
			this.ToServiceObject(serviceObject, storeObject as CalendarItemOccurrence);
		}

		private void ToServiceObject(ServiceObject serviceObject, CalendarItemOccurrence calendarItemOccurrence)
		{
			if (calendarItemOccurrence == null)
			{
				return;
			}
			serviceObject.PropertyBag[CalendarItemSchema.OriginalStart] = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(calendarItemOccurrence.OriginalStartTime);
		}
	}
}
