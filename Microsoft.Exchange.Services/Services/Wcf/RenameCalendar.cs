using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class RenameCalendar : CalendarActionBase<CalendarActionFolderIdResponse>
	{
		public RenameCalendar(MailboxSession session, StoreId calendarToRename, string newCalendarName) : base(session)
		{
			this.calendarToRename = calendarToRename;
			this.newCalendarName = ((newCalendarName != null) ? newCalendarName.Trim() : null);
		}

		public override CalendarActionFolderIdResponse Execute()
		{
			MailboxSession mailboxSession = base.MailboxSession;
			if (string.IsNullOrEmpty(this.newCalendarName))
			{
				ExTraceGlobals.RenameCalendarCallTracer.TraceError<string>((long)this.GetHashCode(), "New calendar name provided is invalid. Name: '{0}'", (this.newCalendarName == null) ? "is null" : this.newCalendarName);
				return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionInvalidCalendarName);
			}
			FolderId folderId = null;
			Microsoft.Exchange.Services.Core.Types.ItemId calendarEntryId = null;
			using (CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Bind(mailboxSession, this.calendarToRename, null))
			{
				string calendarName = calendarGroupEntry.CalendarName;
				ExTraceGlobals.RenameCalendarCallTracer.TraceDebug<VersionedId, string>((long)this.GetHashCode(), "Successfully bound to MEDS.CalendarGroupEntry. Id: '{0}', OldCalendarName: '{1}'", calendarGroupEntry.Id, (calendarName == null) ? "is null" : calendarName);
				if (calendarGroupEntry.IsLocalMailboxCalendar)
				{
					CalendarActionError calendarActionError = this.RenameCalendarFolder(calendarGroupEntry.CalendarId, out folderId);
					if (calendarActionError != CalendarActionError.None)
					{
						return new CalendarActionFolderIdResponse(calendarActionError);
					}
					ExTraceGlobals.RenameCalendarCallTracer.TraceDebug((long)this.GetHashCode(), "Updating Calendar group entry after renaming calendar.");
				}
				calendarGroupEntry.CalendarName = this.newCalendarName;
				ConflictResolutionResult conflictResolutionResult = calendarGroupEntry.Save(SaveMode.NoConflictResolution);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					ExTraceGlobals.RenameCalendarCallTracer.TraceError((long)this.GetHashCode(), "Could not update Calendar group entry with new calendar name.");
					return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionCannotRenameCalendarNode);
				}
				calendarGroupEntry.Load();
				calendarEntryId = IdConverter.ConvertStoreItemIdToItemId(calendarGroupEntry.Id, base.MailboxSession);
			}
			return new CalendarActionFolderIdResponse(folderId, calendarEntryId);
		}

		private CalendarActionError RenameCalendarFolder(StoreObjectId calendarFolderObjectId, out FolderId newCalendarFolderId)
		{
			newCalendarFolderId = null;
			MailboxSession mailboxSession = base.MailboxSession;
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			if (defaultFolderId.Equals(calendarFolderObjectId))
			{
				ExTraceGlobals.RenameCalendarCallTracer.TraceError((long)this.GetHashCode(), "FolderId is the default calendar");
				return CalendarActionError.CalendarActionFolderIdIsDefaultCalendar;
			}
			ExTraceGlobals.RenameCalendarCallTracer.TraceDebug<StoreObjectId, string>((long)this.GetHashCode(), "Renaming calendar with Id: '{0}'. NewName: '{1}'", calendarFolderObjectId, this.newCalendarName);
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSession, calendarFolderObjectId))
			{
				ExTraceGlobals.RenameCalendarCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Successfully bound to calendar. Old calendar name: '{0}'", (calendarFolder.DisplayName == null) ? "is null" : calendarFolder.DisplayName);
				calendarFolder.DisplayName = this.newCalendarName;
				FolderSaveResult folderSaveResult = calendarFolder.Save(SaveMode.NoConflictResolution);
				if (folderSaveResult.OperationResult == OperationResult.Failed)
				{
					ExTraceGlobals.RenameCalendarCallTracer.TraceError<string, StoreObjectId>((long)this.GetHashCode(), "Could not change calendar folder name. NewName: '{0}', FolderId: '{1}'", this.newCalendarName, calendarFolderObjectId);
					return CalendarActionError.CalendarActionCannotRename;
				}
				if (folderSaveResult.OperationResult == OperationResult.PartiallySucceeded && folderSaveResult.PropertyErrors.Length == 1 && folderSaveResult.PropertyErrors[0].PropertyDefinition == FolderSchema.DisplayName)
				{
					return CalendarActionError.CalendarActionCalendarAlreadyExists;
				}
				calendarFolder.Load();
				newCalendarFolderId = IdConverter.GetFolderIdFromStoreId(calendarFolder.Id, new MailboxId(mailboxSession));
			}
			return CalendarActionError.None;
		}

		private readonly StoreId calendarToRename;

		private readonly string newCalendarName;
	}
}
