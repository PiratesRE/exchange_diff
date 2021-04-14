using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ParentPropertyIndex
	{
		public static readonly int AppointmentRecurrenceBlob = Array.IndexOf<PropertyDefinition>(CalendarItemProperties.MeetingReplyForwardProperties, InternalSchema.AppointmentRecurrenceBlob);

		public static readonly int AppointmentRecurring = Array.IndexOf<PropertyDefinition>(CalendarItemProperties.MeetingReplyForwardProperties, InternalSchema.AppointmentRecurring);

		public static readonly int IsRecurring = Array.IndexOf<PropertyDefinition>(CalendarItemProperties.MeetingReplyForwardProperties, InternalSchema.IsRecurring);

		public static readonly int IsException = Array.IndexOf<PropertyDefinition>(CalendarItemProperties.MeetingReplyForwardProperties, InternalSchema.IsException);

		public static readonly int TimeZoneBlob = Array.IndexOf<PropertyDefinition>(CalendarItemProperties.MeetingReplyForwardProperties, InternalSchema.TimeZoneBlob);

		public static readonly int RecurrencePattern = Array.IndexOf<PropertyDefinition>(CalendarItemProperties.MeetingReplyForwardProperties, InternalSchema.RecurrencePattern);

		public static readonly int RecurrenceType = Array.IndexOf<PropertyDefinition>(CalendarItemProperties.MeetingReplyForwardProperties, InternalSchema.MapiRecurrenceType);
	}
}
