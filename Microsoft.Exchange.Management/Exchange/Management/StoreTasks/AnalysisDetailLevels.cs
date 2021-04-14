using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AnalysisDetailLevels
	{
		internal static IEnumerable<PropertyDefinition> GetDisplayProperties(AnalysisDetailLevel level)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			list.Add(ItemSchema.Id);
			list.Add(ItemSchema.NormalizedSubject);
			list.Add(CalendarItemInstanceSchema.StartTime);
			list.Add(CalendarItemInstanceSchema.EndTime);
			list.Add(CalendarItemBaseSchema.CalendarLogTriggerAction);
			list.Add(CalendarItemBaseSchema.ClientInfoString);
			list.Add(CalendarItemBaseSchema.OriginalLastModifiedTime);
			list.Add(CalendarItemBaseSchema.ClientIntent);
			list.Add(CalendarItemBaseSchema.CleanGlobalObjectId);
			list.Add(StoreObjectSchema.ItemClass);
			list.Add(ItemSchema.ParentDisplayName);
			list.Add(CalendarItemBaseSchema.Duration);
			list.Add(CalendarItemBaseSchema.AppointmentRecurring);
			list.Add(CalendarItemBaseSchema.OrganizerEmailAddress);
			list.Add(CalendarItemBaseSchema.SenderEmailAddress);
			list.Add(ItemSchema.SentRepresentingDisplayName);
			if (level == AnalysisDetailLevel.Basic)
			{
				return list;
			}
			list.Add(CalendarItemBaseSchema.ItemVersion);
			list.Add(CalendarItemBaseSchema.AppointmentSequenceNumber);
			list.Add(CalendarItemBaseSchema.AppointmentLastSequenceNumber);
			list.Add(ItemSchema.IsResponseRequested);
			list.Add(CalendarItemBaseSchema.ResponseState);
			list.Add(CalendarItemBaseSchema.Location);
			list.Add(CalendarItemBaseSchema.IsException);
			list.Add(CalendarItemBaseSchema.FreeBusyStatus);
			list.Add(CalendarItemBaseSchema.IntendedFreeBusyStatus);
			list.Add(CalendarItemBaseSchema.ResponsibleUserName);
			list.Add(CalendarItemBaseSchema.AppointmentState);
			list.Add(CalendarItemBaseSchema.GlobalObjectId);
			list.Add(StoreObjectSchema.CreationTime);
			list.Add(StoreObjectSchema.LastModifiedTime);
			list.Add(CalendarItemBaseSchema.AppointmentAuxiliaryFlags);
			list.Add(CalendarItemBaseSchema.IsProcessed);
			list.Add(ItemSchema.Subject);
			list.Add(CalendarItemBaseSchema.TimeZone);
			list.Add(CalendarItemBaseSchema.StartTimeZone);
			list.Add(CalendarItemBaseSchema.StartTimeZoneId);
			list.Add(CalendarItemBaseSchema.EndTimeZoneId);
			list.Add(CalendarItemBaseSchema.RecurrencePattern);
			list.Add(CalendarItemBaseSchema.RecurrenceType);
			list.Add(CalendarItemBaseSchema.AppointmentRecurrenceBlob);
			list.Add(CalendarItemBaseSchema.OwnerCriticalChangeTime);
			list.Add(CalendarItemBaseSchema.AttendeeCriticalChangeTime);
			list.Add(CalendarItemBaseSchema.ChangeHighlight);
			list.Add(CalendarItemBaseSchema.MeetingRequestType);
			list.Add(CalendarItemBaseSchema.MapiIsAllDayEvent);
			list.Add(CalendarItemBaseSchema.ChangeList);
			list.Add(CalendarItemBaseSchema.MiddleTierServerName);
			list.Add(CalendarItemBaseSchema.MiddleTierServerBuildVersion);
			list.Add(CalendarItemBaseSchema.MailboxServerName);
			list.Add(CalendarItemBaseSchema.MiddleTierProcessName);
			list.Add(ItemSchema.InternetMessageId);
			return list;
		}
	}
}
