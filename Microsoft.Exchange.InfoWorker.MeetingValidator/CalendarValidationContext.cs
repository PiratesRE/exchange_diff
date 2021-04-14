using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	public class CalendarValidationContext : IDisposable
	{
		private CalendarValidationContext()
		{
		}

		internal static CalendarValidationContext CreateInstance(CalendarItemBase calendarItem, bool isOrganizer, UserObject localUser, UserObject remoteUser, CalendarVersionStoreGateway cvsGateway, AttendeeExtractor attendeeExtractor)
		{
			CalendarValidationContext calendarValidationContext = new CalendarValidationContext();
			calendarValidationContext.LocalUser = localUser;
			calendarValidationContext.RemoteUser = remoteUser;
			if (isOrganizer)
			{
				calendarValidationContext.BaseRole = RoleType.Organizer;
				calendarValidationContext.OppositeRole = RoleType.Attendee;
				calendarValidationContext.Organizer = localUser;
				calendarValidationContext.Attendee = remoteUser;
			}
			else
			{
				calendarValidationContext.BaseRole = RoleType.Attendee;
				calendarValidationContext.OppositeRole = RoleType.Organizer;
				calendarValidationContext.Organizer = remoteUser;
				calendarValidationContext.Attendee = localUser;
			}
			calendarValidationContext.calendarItems = new Dictionary<RoleType, CalendarItemBase>(2);
			calendarValidationContext.calendarItems.Add(calendarValidationContext.BaseRole, calendarItem);
			calendarValidationContext.calendarItems.Add(calendarValidationContext.OppositeRole, null);
			calendarValidationContext.OrganizerRecurrence = null;
			calendarValidationContext.OrganizerExceptions = null;
			calendarValidationContext.OrganizerDeletions = null;
			calendarValidationContext.AttendeeRecurrence = null;
			calendarValidationContext.AttendeeExceptions = null;
			calendarValidationContext.AttendeeDeletions = null;
			calendarValidationContext.OppositeRoleOrganizerIsValid = false;
			calendarValidationContext.CvsGateway = cvsGateway;
			calendarValidationContext.AttendeeExtractor = attendeeExtractor;
			return calendarValidationContext;
		}

		internal UserObject Organizer { get; set; }

		internal UserObject Attendee { get; set; }

		internal CalendarItemBase OrganizerItem
		{
			get
			{
				return this.calendarItems[RoleType.Organizer];
			}
		}

		internal CalendarItemBase AttendeeItem
		{
			get
			{
				return this.calendarItems[RoleType.Attendee];
			}
		}

		internal RecurrenceInfo OrganizerRecurrence { get; set; }

		internal RecurrenceInfo AttendeeRecurrence { get; set; }

		internal bool AreItemsOccurrences
		{
			get
			{
				return this.AttendeeItem != null && this.AttendeeItem.CalendarItemType == CalendarItemType.Occurrence && this.OrganizerItem != null && this.OrganizerItem.CalendarItemType == CalendarItemType.Occurrence;
			}
		}

		internal RoleType BaseRole { get; private set; }

		internal RoleType OppositeRole { get; private set; }

		internal bool IsRoleGroupMailbox(RoleType roleType)
		{
			if (roleType != RoleType.Organizer)
			{
				return this.Attendee.IsGroupMailbox;
			}
			return this.Organizer.IsGroupMailbox;
		}

		internal bool OppositeRoleOrganizerIsValid { get; set; }

		internal CalendarItemBase BaseItem
		{
			get
			{
				return this.calendarItems[this.BaseRole];
			}
			set
			{
				this.calendarItems[this.BaseRole] = value;
			}
		}

		internal CalendarItemBase OppositeItem
		{
			get
			{
				return this.calendarItems[this.OppositeRole];
			}
			set
			{
				this.calendarItems[this.OppositeRole] = value;
			}
		}

		internal List<OccurrenceInfo> OrganizerExceptions { get; set; }

		internal List<OccurrenceInfo> AttendeeExceptions { get; set; }

		internal ExDateTime[] OrganizerDeletions { get; set; }

		internal ExDateTime[] AttendeeDeletions { get; set; }

		internal CalendarInstance CalendarInstance { get; set; }

		internal UserObject LocalUser { get; private set; }

		internal UserObject RemoteUser { get; private set; }

		internal bool HasSentUpdateForItemOrMaster { get; set; }

		internal CalendarVersionStoreGateway CvsGateway { get; private set; }

		internal AttendeeExtractor AttendeeExtractor { get; private set; }

		internal string ErrorString { get; set; }

		public void Dispose()
		{
			if (this.BaseItem != null)
			{
				this.BaseItem.Dispose();
				this.BaseItem = null;
			}
			if (this.OppositeItem != null)
			{
				this.OppositeItem.Dispose();
				this.OppositeItem = null;
			}
		}

		private Dictionary<RoleType, CalendarItemBase> calendarItems;
	}
}
