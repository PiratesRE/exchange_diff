using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class MoveCalendar : CalendarActionBase<CalendarActionResponse>
	{
		public MoveCalendar(MailboxSession session, StoreObjectId calendarToMove, string parentGroupId, StoreObjectId calendarBefore) : base(session)
		{
			this.calendarToMove = calendarToMove;
			this.parentGroupId = parentGroupId;
			this.calendarBefore = calendarBefore;
		}

		public override CalendarActionResponse Execute()
		{
			Guid guid;
			if (!Guid.TryParse(this.parentGroupId, out guid))
			{
				ExTraceGlobals.MoveCalendarCallTracer.TraceError<string>((long)this.GetHashCode(), "Provided parent group id is not valid. GroupId: '{0}'", (this.parentGroupId == null) ? "is null" : this.parentGroupId);
				return new CalendarActionResponse(CalendarActionError.CalendarActionInvalidGroupId);
			}
			byte[] array = null;
			byte[] nodeAfter = null;
			VersionedId versionedId = this.FindCalendarNodeId(guid, out array, out nodeAfter);
			if (versionedId == null)
			{
				ExTraceGlobals.MoveCalendarCallTracer.TraceError<StoreObjectId, string, Guid>((long)this.GetHashCode(), "Unable to find a calendar node for the corresponding FolderId. CalendarToMove: '{0}', CalendarBefore: '{1}', ParentGroupId: '{2}'", this.calendarToMove, (this.calendarBefore == null) ? "is null" : this.calendarBefore.ToBase64String(), guid);
				return new CalendarActionResponse(CalendarActionError.CalendarActionInvalidCalendarNodeOrder);
			}
			if (this.calendarBefore != null && array == null)
			{
				ExTraceGlobals.MoveCalendarCallTracer.TraceError<StoreObjectId>((long)this.GetHashCode(), "Unable to find ordinal values for the before and after nodes. CalendarBefore: '{0}'", this.calendarBefore);
				return new CalendarActionResponse(CalendarActionError.CalendarActionInvalidCalendarNodeOrder);
			}
			using (CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Bind(base.MailboxSession, versionedId, null))
			{
				string calendarName = calendarGroupEntry.CalendarName;
				ExTraceGlobals.MoveCalendarCallTracer.TraceDebug<string, Guid, Guid>((long)this.GetHashCode(), "Successfully bound to calendar node - name: '{0}'. Changing itsordinal value and groupclassid. PreviousGroupId: '{1}' NewGroupId: '{2}'. ", (calendarName == null) ? "is null" : calendarName, calendarGroupEntry.ParentGroupClassId, guid);
				calendarGroupEntry.ParentGroupClassId = guid;
				calendarGroupEntry.SetNodeOrdinal(array, nodeAfter);
				ConflictResolutionResult conflictResolutionResult = calendarGroupEntry.Save(SaveMode.NoConflictResolution);
				if (conflictResolutionResult.SaveStatus != SaveResult.Success)
				{
					ExTraceGlobals.MoveCalendarCallTracer.TraceError<string, VersionedId>((long)this.GetHashCode(), "Uanble to update Calendar group entry. CalendarName: '{0}'. CalendarId: '{1}'", (calendarName == null) ? "is null" : calendarName, calendarGroupEntry.Id);
					return new CalendarActionResponse(CalendarActionError.CalendarActionUnableToUpdateCalendarNode);
				}
			}
			return new CalendarActionResponse();
		}

		private VersionedId FindCalendarNodeId(Guid groupId, out byte[] calendarBeforeOrdinal, out byte[] calendarAfterOrdinal)
		{
			calendarBeforeOrdinal = null;
			calendarAfterOrdinal = null;
			VersionedId versionedId = null;
			ExTraceGlobals.MoveCalendarCallTracer.TraceDebug<Guid, string, StoreObjectId>((long)this.GetHashCode(), "Binding to the parent group id. GroupId: '{0}'. CalendarBefore: '{1}' CalendarId: '{2}'", groupId, (this.calendarBefore == null) ? "is null" : this.calendarBefore.ToBase64String(), this.calendarToMove);
			using (CalendarGroup calendarGroup = CalendarGroup.Bind(base.MailboxSession, groupId))
			{
				ExTraceGlobals.MoveCalendarCallTracer.TraceDebug((long)this.GetHashCode(), "Successfully bound to calendar group.");
				ReadOnlyCollection<CalendarGroupEntryInfo> childCalendars = calendarGroup.GetChildCalendars();
				bool flag = this.calendarBefore == null;
				foreach (CalendarGroupEntryInfo calendarGroupEntryInfo in childCalendars)
				{
					StoreId id = (calendarGroupEntryInfo is LocalCalendarGroupEntryInfo) ? ((LocalCalendarGroupEntryInfo)calendarGroupEntryInfo).CalendarId : null;
					if (flag)
					{
						ExTraceGlobals.MoveCalendarCallTracer.TraceDebug((long)this.GetHashCode(), "calendarAfter was found");
						calendarAfterOrdinal = calendarGroupEntryInfo.Ordinal;
						flag = false;
					}
					else if (this.calendarBefore != null && this.calendarBefore.Equals(id))
					{
						ExTraceGlobals.MoveCalendarCallTracer.TraceDebug((long)this.GetHashCode(), "calendarBefore was found");
						flag = true;
						calendarBeforeOrdinal = calendarGroupEntryInfo.Ordinal;
					}
					else if (this.calendarToMove.Equals(id))
					{
						ExTraceGlobals.MoveCalendarCallTracer.TraceDebug((long)this.GetHashCode(), "calendarToMove was found");
						versionedId = calendarGroupEntryInfo.Id;
					}
				}
				if (versionedId == null)
				{
					ExTraceGlobals.MoveCalendarCallTracer.TraceDebug((long)this.GetHashCode(), "Calendar was not present in group list. This will happen if the calendar is being moved from a different group.");
					return CalendarGroupEntry.GetGroupEntryIdFromFolderId(base.MailboxSession, this.calendarToMove);
				}
			}
			return versionedId;
		}

		private readonly StoreObjectId calendarToMove;

		private readonly string parentGroupId;

		private readonly StoreObjectId calendarBefore;
	}
}
