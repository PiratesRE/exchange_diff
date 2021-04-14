using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.Calendaring.EntitySets;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendaringContainer : StorageEntitySetScope<IMailboxSession>, ICalendaringContainer
	{
		public CalendaringContainer(IStorageEntitySetScope<IMailboxSession> parentScope) : base(parentScope)
		{
			this.description = string.Format("{0}.Calendaring", parentScope);
			this.calendarGroups = new CalendarGroups(this, null);
		}

		public CalendaringContainer(IStoreSession session, IXSOFactory xsoFactory = null) : this(new StorageEntitySetScope<IMailboxSession>((IMailboxSession)session, session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), xsoFactory ?? XSOFactory.Default, null))
		{
		}

		public ICalendarGroups CalendarGroups
		{
			get
			{
				return this.calendarGroups;
			}
		}

		public IMailboxCalendars Calendars
		{
			get
			{
				MailboxCalendars result;
				if ((result = this.calendars) == null)
				{
					result = (this.calendars = new MailboxCalendars(this, this.calendarGroups.MyCalendars));
				}
				return result;
			}
		}

		public IMeetingRequestMessages MeetingRequestMessages
		{
			get
			{
				MeetingRequestMessages result;
				if ((result = this.meetingRequestMessages) == null)
				{
					result = (this.meetingRequestMessages = new MeetingRequestMessages(this, this.Calendars.Default.Events));
				}
				return result;
			}
		}

		public override string ToString()
		{
			return this.description;
		}

		private readonly string description;

		private readonly CalendarGroups calendarGroups;

		private MailboxCalendars calendars;

		private MeetingRequestMessages meetingRequestMessages;
	}
}
