using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExternalUserCalendarResponseShape : ExternalUserResponseShape
	{
		private static List<PropertyPath> CreateList(List<PropertyPath> originalList, params PropertyPath[] additionalProperties)
		{
			List<PropertyPath> list = new List<PropertyPath>(originalList);
			foreach (PropertyPath item in additionalProperties)
			{
				list.Add(item);
			}
			return list;
		}

		protected override List<PropertyPath> PropertiesAllowedForReadAccess
		{
			get
			{
				return ExternalUserCalendarResponseShape.calendarItemReadProps;
			}
		}

		public ExternalUserCalendarResponseShape(Permission permissionGranted)
		{
			base.Permissions = permissionGranted;
		}

		protected override PropertyPath[] GetPropertiesForCustomPermissions(ItemResponseShape requestedResponseShape)
		{
			CalendarFolderPermission calendarFolderPermission = base.Permissions as CalendarFolderPermission;
			switch (calendarFolderPermission.FreeBusyAccess)
			{
			case FreeBusyAccess.Basic:
				ExTraceGlobals.ExternalUserTracer.TraceDebug<ExternalUserCalendarResponseShape>((long)this.GetHashCode(), "{0}: overriding shape for FreeBusy Basic permissions.", this);
				return ExternalUserResponseShape.GetAllowedProperties(requestedResponseShape, ExternalUserCalendarResponseShape.calendarItemFreeBusyProps);
			case FreeBusyAccess.Details:
				ExTraceGlobals.ExternalUserTracer.TraceDebug<ExternalUserCalendarResponseShape>((long)this.GetHashCode(), "{0}: overriding shape for FreeBusy Detailed permissions.", this);
				return ExternalUserResponseShape.GetAllowedProperties(requestedResponseShape, ExternalUserCalendarResponseShape.calendarItemFreeBusyDetailsProps);
			default:
				return null;
			}
		}

		private static List<PropertyPath> calendarItemFreeBusyProps = new List<PropertyPath>
		{
			ItemSchema.ItemId.PropertyPath,
			CalendarItemSchema.CalendarItemType.PropertyPath,
			CalendarItemSchema.Start.PropertyPath,
			CalendarItemSchema.End.PropertyPath,
			CalendarItemSchema.IsAllDayEvent.PropertyPath,
			CalendarItemSchema.IsCancelled.PropertyPath,
			CalendarItemSchema.IsRecurring.PropertyPath,
			CalendarItemSchema.LegacyFreeBusyStatus.PropertyPath,
			CalendarItemSchema.OrganizerSpecific.Recurrence.PropertyPath,
			CalendarItemSchema.ModifiedOccurrences.PropertyPath,
			CalendarItemSchema.DeletedOccurrences.PropertyPath,
			CalendarItemSchema.Duration.PropertyPath,
			CalendarItemSchema.OrganizerSpecific.StartTimeZone.PropertyPath,
			CalendarItemSchema.OrganizerSpecific.EndTimeZone.PropertyPath,
			CalendarItemSchema.TimeZone.PropertyPath
		};

		public static List<PropertyPath> CalendarPropertiesPrivateItem = ExternalUserCalendarResponseShape.CreateList(ExternalUserCalendarResponseShape.calendarItemFreeBusyProps, new PropertyPath[]
		{
			ItemSchema.Sensitivity.PropertyPath
		});

		public static List<PropertyPath> CalendarPropertiesPrivateItemWithSubject = ExternalUserCalendarResponseShape.CreateList(ExternalUserCalendarResponseShape.CalendarPropertiesPrivateItem, new PropertyPath[]
		{
			ItemSchema.Subject.PropertyPath
		});

		private static List<PropertyPath> calendarItemFreeBusyDetailsProps = ExternalUserCalendarResponseShape.CreateList(ExternalUserCalendarResponseShape.CalendarPropertiesPrivateItem, new PropertyPath[]
		{
			ItemSchema.Subject.PropertyPath,
			CalendarItemSchema.Location.PropertyPath
		});

		private static List<PropertyPath> calendarItemReadProps = ExternalUserCalendarResponseShape.CreateList(ExternalUserCalendarResponseShape.calendarItemFreeBusyDetailsProps, new PropertyPath[]
		{
			ItemSchema.Body.PropertyPath
		});
	}
}
