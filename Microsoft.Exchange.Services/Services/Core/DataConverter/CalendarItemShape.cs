using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class CalendarItemShape : Shape
	{
		static CalendarItemShape()
		{
			CalendarItemShape.defaultPropertiesForOrganizer.Add(ItemSchema.ItemId);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(ItemSchema.Subject);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(ItemSchema.Attachments);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(ItemSchema.ResponseObjects);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(ItemSchema.HasAttachments);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(ItemSchema.IsAssociated);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(CalendarItemSchema.Start);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(CalendarItemSchema.End);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(CalendarItemSchema.OriginalStart);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(CalendarItemSchema.LegacyFreeBusyStatus);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(CalendarItemSchema.Location);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(CalendarItemSchema.CalendarItemType);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(CalendarItemSchema.Organizer);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(CalendarItemSchema.OrganizerSpecific.RequiredAttendees);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(CalendarItemSchema.OrganizerSpecific.OptionalAttendees);
			CalendarItemShape.defaultPropertiesForOrganizer.Add(CalendarItemSchema.OrganizerSpecific.Resources);
			CalendarItemShape.defaultPropertiesForAttendee = new List<PropertyInformation>();
			CalendarItemShape.defaultPropertiesForAttendee.AddRange(CalendarItemShape.defaultPropertiesForOrganizer);
			CalendarItemShape.defaultPropertiesForAttendee[CalendarItemShape.defaultPropertiesForAttendee.IndexOf(CalendarItemSchema.OrganizerSpecific.RequiredAttendees)] = CalendarItemSchema.AttendeeSpecific.RequiredAttendees;
			CalendarItemShape.defaultPropertiesForAttendee[CalendarItemShape.defaultPropertiesForAttendee.IndexOf(CalendarItemSchema.OrganizerSpecific.OptionalAttendees)] = CalendarItemSchema.AttendeeSpecific.OptionalAttendees;
			CalendarItemShape.defaultPropertiesForAttendee[CalendarItemShape.defaultPropertiesForAttendee.IndexOf(CalendarItemSchema.OrganizerSpecific.Resources)] = CalendarItemSchema.AttendeeSpecific.Resources;
		}

		private CalendarItemShape(bool forOrganizer) : base(Schema.CalendarItem, CalendarItemSchema.GetSchema(forOrganizer), ItemShape.CreateShape(), forOrganizer ? CalendarItemShape.defaultPropertiesForOrganizer : CalendarItemShape.defaultPropertiesForAttendee)
		{
		}

		internal static CalendarItemShape CreateShapeForAttendee()
		{
			return new CalendarItemShape(false);
		}

		internal static CalendarItemShape CreateShape(StoreObject storeObject)
		{
			CalendarItemBase calendarItemBase = storeObject as CalendarItemBase;
			return new CalendarItemShape(calendarItemBase.IsOrganizer());
		}

		private static List<PropertyInformation> defaultPropertiesForOrganizer = new List<PropertyInformation>();

		private static List<PropertyInformation> defaultPropertiesForAttendee;
	}
}
