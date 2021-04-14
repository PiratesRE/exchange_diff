using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SubscribeInternetCalendar : CalendarActionBase<CalendarActionFolderIdResponse>
	{
		public SubscribeInternetCalendar(MailboxSession subscriberSession, string iCalUrl, Guid groupId, string calendarFolderName = null) : base(subscriberSession)
		{
			this.iCalUrl = iCalUrl;
			this.groupId = groupId;
			this.calendarFolderName = calendarFolderName;
		}

		public override CalendarActionFolderIdResponse Execute()
		{
			SubscribeResultsWebCal subscribeResultsWebCal = WebCalendar.Subscribe(base.MailboxSession, this.iCalUrl, this.calendarFolderName);
			if (subscribeResultsWebCal.LocalFolderCreated || subscribeResultsWebCal.LocalFolderId != null)
			{
				SubscribeInternetCalendarCommand.TraceDebug(this, "Successfully created the entry");
				SubscribeInternetCalendarCommand.TraceDebug(this, "Creating the ClaendarGroupEntry on the CommonViews and adding it to the group passed to the request: " + this.groupId);
				CalendarGroupEntry calendarGroupEntry2;
				CalendarGroupEntry calendarGroupEntry = calendarGroupEntry2 = this.CreateCalendarGroupEntry(subscribeResultsWebCal.LocalFolderId, this.groupId);
				try
				{
					if (calendarGroupEntry != null)
					{
						FolderId folderIdFromStoreId = IdConverter.GetFolderIdFromStoreId(subscribeResultsWebCal.LocalFolderId, new MailboxId(base.MailboxSession));
						Microsoft.Exchange.Services.Core.Types.ItemId calendarEntryId = IdConverter.ConvertStoreItemIdToItemId(calendarGroupEntry.CalendarGroupEntryId, base.MailboxSession);
						return new CalendarActionFolderIdResponse(folderIdFromStoreId, calendarEntryId);
					}
				}
				finally
				{
					if (calendarGroupEntry2 != null)
					{
						((IDisposable)calendarGroupEntry2).Dispose();
					}
				}
			}
			SubscribeInternetCalendarCommand.TraceError(this, "Could not create the entry on group " + this.groupId);
			return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionUnableToSubscribeToCalendar);
		}

		private CalendarGroupEntry CreateCalendarGroupEntry(StoreObjectId folderId, Guid groupId)
		{
			CalendarGroupEntry result;
			using (CalendarGroup calendarGroup = CalendarGroup.Bind(base.MailboxSession, groupId))
			{
				using (CalendarFolder calendarFolder = CalendarFolder.Bind(base.MailboxSession, folderId))
				{
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(base.MailboxSession, calendarFolder, calendarGroup);
						disposeGuard.Add<CalendarGroupEntry>(calendarGroupEntry);
						calendarGroupEntry.CalendarName = calendarFolder.DisplayName;
						calendarGroupEntry.CalendarRecordKey = (byte[])calendarFolder.TryGetProperty(StoreObjectSchema.RecordKey);
						ConflictResolutionResult conflictResolutionResult = calendarGroupEntry.Save(SaveMode.NoConflictResolution);
						if (conflictResolutionResult.SaveStatus != SaveResult.Success)
						{
							string message = string.Format("Unable to associate calendar with the specified group. CalendarName: {0}, FolderId: {1}.", calendarFolder.DisplayName, calendarGroup.GroupName);
							SubscribeInternetCalendarCommand.TraceError(this, message);
							result = null;
						}
						else
						{
							calendarGroupEntry.Load();
							disposeGuard.Success();
							result = calendarGroupEntry;
						}
					}
				}
			}
			return result;
		}

		private readonly string iCalUrl;

		private readonly Guid groupId;

		private readonly string calendarFolderName;
	}
}
