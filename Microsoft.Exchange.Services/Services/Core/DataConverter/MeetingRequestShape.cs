using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MeetingRequestShape : Shape
	{
		static MeetingRequestShape()
		{
			MeetingRequestShape.defaultProperties.Add(ItemSchema.ItemId);
			MeetingRequestShape.defaultProperties.Add(ItemSchema.Attachments);
			MeetingRequestShape.defaultProperties.Add(ItemSchema.Subject);
			MeetingRequestShape.defaultProperties.Add(ItemSchema.Sensitivity);
			MeetingRequestShape.defaultProperties.Add(ItemSchema.ResponseObjects);
			MeetingRequestShape.defaultProperties.Add(ItemSchema.HasAttachments);
			MeetingRequestShape.defaultProperties.Add(ItemSchema.IsAssociated);
			MeetingRequestShape.defaultProperties.Add(MessageSchema.ToRecipients);
			MeetingRequestShape.defaultProperties.Add(MessageSchema.CcRecipients);
			MeetingRequestShape.defaultProperties.Add(MessageSchema.BccRecipients);
			MeetingRequestShape.defaultProperties.Add(MessageSchema.IsRead);
			MeetingRequestShape.defaultProperties.Add(MeetingMessageSchema.AssociatedCalendarItemId);
			MeetingRequestShape.defaultProperties.Add(MeetingMessageSchema.IsDelegated);
			MeetingRequestShape.defaultProperties.Add(MeetingMessageSchema.IsOutOfDate);
			MeetingRequestShape.defaultProperties.Add(MeetingMessageSchema.HasBeenProcessed);
			MeetingRequestShape.defaultProperties.Add(MeetingMessageSchema.ResponseType);
			MeetingRequestShape.defaultProperties.Add(MeetingRequestSchema.MeetingRequestType);
			MeetingRequestShape.defaultProperties.Add(MeetingRequestSchema.IntendedFreeBusyStatus);
			MeetingRequestShape.defaultProperties.Add(MeetingRequestSchema.Start);
			MeetingRequestShape.defaultProperties.Add(MeetingRequestSchema.End);
			MeetingRequestShape.defaultProperties.Add(MeetingRequestSchema.Location);
			MeetingRequestShape.defaultProperties.Add(CalendarItemSchema.Organizer);
			MeetingRequestShape.defaultProperties.Add(CalendarItemSchema.AttendeeSpecific.RequiredAttendees);
			MeetingRequestShape.defaultProperties.Add(CalendarItemSchema.AttendeeSpecific.OptionalAttendees);
			MeetingRequestShape.defaultProperties.Add(CalendarItemSchema.AttendeeSpecific.Resources);
			MeetingRequestShape.defaultProperties.Add(CalendarItemSchema.AdjacentMeetingCount);
			MeetingRequestShape.defaultProperties.Add(CalendarItemSchema.ConflictingMeetingCount);
		}

		private MeetingRequestShape() : base(Schema.MeetingRequest, MeetingRequestSchema.GetSchema(), MeetingMessageShape.CreateShape(), MeetingRequestShape.defaultProperties)
		{
		}

		internal static MeetingRequestShape CreateShape()
		{
			return new MeetingRequestShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
