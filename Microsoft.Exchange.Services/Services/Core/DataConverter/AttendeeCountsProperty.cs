using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class AttendeeCountsProperty : ComplexPropertyBase, IToServiceObjectCommand, IPropertyCommand
	{
		public AttendeeCountsProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static AttendeeCountsProperty CreateCommand(CommandContext commandContext)
		{
			return new AttendeeCountsProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("AttendeeCountsProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			CalendarItemBase calendarItemBase = commandSettings.StoreObject as CalendarItemBase;
			if (calendarItemBase == null)
			{
				calendarItemBase = ((MeetingRequest)commandSettings.StoreObject).GetCachedEmbeddedItem();
				serviceObject[propertyInformation] = this.GetAttendeeCounts(calendarItemBase);
				return;
			}
			serviceObject[propertyInformation] = this.GetAttendeeCounts(calendarItemBase);
		}

		private AttendeeCountsType GetAttendeeCounts(CalendarItemBase calendarItem)
		{
			if (calendarItem == null)
			{
				return null;
			}
			AttendeeCountsType attendeeCountsType = new AttendeeCountsType();
			if (calendarItem.AttendeeCollection != null)
			{
				foreach (Attendee attendee in calendarItem.AttendeeCollection)
				{
					switch (attendee.AttendeeType)
					{
					case AttendeeType.Required:
						attendeeCountsType.RequiredAttendeesCount++;
						break;
					case AttendeeType.Optional:
						attendeeCountsType.OptionalAttendeesCount++;
						break;
					case AttendeeType.Resource:
						attendeeCountsType.ResourcesCount++;
						break;
					}
				}
			}
			return attendeeCountsType;
		}
	}
}
