using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class DeleteCalendar : CalendarActionBase<CalendarActionResponse>
	{
		public DeleteCalendar(MailboxSession session, StoreId calendarEntryId) : base(session)
		{
			this.calendarEntryId = calendarEntryId;
		}

		public override CalendarActionResponse Execute()
		{
			MailboxSession mailboxSession = base.MailboxSession;
			StoreObjectId storeObjectId = null;
			bool flag = false;
			using (CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Bind(mailboxSession, this.calendarEntryId, null))
			{
				ExTraceGlobals.DeleteCalendarCallTracer.TraceDebug<string, VersionedId>((long)this.GetHashCode(), "Successfully bound to CalendarGroupEntry for deletion. CalendarName: {0} Id: {1}", (calendarGroupEntry.CalendarName == null) ? string.Empty : calendarGroupEntry.CalendarName, calendarGroupEntry.Id);
				if (calendarGroupEntry.IsLocalMailboxCalendar)
				{
					storeObjectId = calendarGroupEntry.CalendarId;
					CalendarActionError calendarActionError = this.DeleteCalendarFolder(storeObjectId);
					if (calendarActionError != CalendarActionError.None)
					{
						return new CalendarActionResponse(calendarActionError);
					}
				}
				flag = !calendarGroupEntry.IsLocalMailboxCalendar;
			}
			AggregateOperationResult aggregateOperationResult = mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
			{
				this.calendarEntryId
			});
			if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
			{
				ExTraceGlobals.DeleteCalendarCallTracer.TraceError<string, StoreId, OperationResult>((long)this.GetHashCode(), "Unable to delete Calendar group entry. CalendarId: '{0}'. CalendarNodeId: '{1}' Result: {2}", (storeObjectId == null) ? string.Empty : storeObjectId.ToBase64String(), this.calendarEntryId, aggregateOperationResult.OperationResult);
				if (flag)
				{
					return new CalendarActionResponse(CalendarActionError.CalendarActionCannotDeleteCalendar);
				}
			}
			return new CalendarActionResponse();
		}

		private CalendarActionError DeleteCalendarFolder(StoreObjectId calendarObjectId)
		{
			MailboxSession mailboxSession = base.MailboxSession;
			if (mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar).Equals(this.calendarEntryId))
			{
				ExTraceGlobals.DeleteCalendarCallTracer.TraceError((long)this.GetHashCode(), "FolderId is the default calendar");
				return CalendarActionError.CalendarActionFolderIdIsDefaultCalendar;
			}
			ExTraceGlobals.DeleteCalendarCallTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "Attempting to delete calendar folder. Id: '{0}'", calendarObjectId);
			AggregateOperationResult aggregateOperationResult = mailboxSession.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
			{
				calendarObjectId
			});
			if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
			{
				ExTraceGlobals.DeleteCalendarCallTracer.TraceError<StoreObjectId, OperationResult>((long)this.GetHashCode(), "Unable to delete calendar. Id: '{0}'. Result: {1}", calendarObjectId, aggregateOperationResult.OperationResult);
				return CalendarActionError.CalendarActionCannotDeleteCalendar;
			}
			return CalendarActionError.None;
		}

		private readonly StoreId calendarEntryId;
	}
}
