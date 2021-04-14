using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class CreateCalendar : CalendarActionBase<CalendarActionFolderIdResponse>
	{
		public CreateCalendar(MailboxSession session, string newCalendarName, string parentGroupGuid, string emailAddress, IRecipientSession adRecipientSession) : base(session)
		{
			this.newCalendarName = ((newCalendarName != null) ? newCalendarName.Trim() : null);
			this.parentGroupGuid = parentGroupGuid;
			this.emailAddress = ((emailAddress != null) ? emailAddress.Trim() : null);
			this.adRecipientSession = adRecipientSession;
		}

		public override CalendarActionFolderIdResponse Execute()
		{
			bool flag = this.emailAddress != null;
			if (string.IsNullOrEmpty(this.newCalendarName))
			{
				ExTraceGlobals.CreateCalendarCallTracer.TraceError<string>((long)this.GetHashCode(), "Calendar name is invalid. Name: '{0}'", (this.newCalendarName == null) ? "is null" : this.newCalendarName);
				return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionInvalidCalendarName);
			}
			Guid guid;
			if (!Guid.TryParse(this.parentGroupGuid, out guid))
			{
				ExTraceGlobals.CreateCalendarCallTracer.TraceError<string, string>((long)this.GetHashCode(), "Parent group id is invalid. CalendarName: '{0}'. ParentGroupId: '{1}'", this.newCalendarName, (this.parentGroupGuid == null) ? "is null" : this.parentGroupGuid);
				return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionInvalidGroupId);
			}
			ADRecipient sharedUser = null;
			if (flag)
			{
				if (!SmtpAddress.IsValidSmtpAddress(this.emailAddress))
				{
					ExTraceGlobals.CreateCalendarCallTracer.TraceError<string, string, string>((long)this.GetHashCode(), "The specified email address is invalid. EmailAddress: '{0}'. CalendarName: '{1}' ParentGroupId: '{2}'", this.emailAddress, this.newCalendarName, this.parentGroupGuid);
					return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionInvalidCalendarEmailAddress);
				}
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					sharedUser = this.adRecipientSession.FindByProxyAddress(ProxyAddress.Parse(this.emailAddress));
				});
				if (sharedUser == null)
				{
					ExTraceGlobals.CreateCalendarCallTracer.TraceError((long)this.GetHashCode(), "Unable to find a matching user in AD for the specified email address. EmailAddress: '{0}'. CalendarName: '{1}' ParentGroupId: '{2}'. ADOperationResult:{3} ADOperationException:{4}", new object[]
					{
						this.emailAddress,
						this.newCalendarName,
						this.parentGroupGuid,
						adoperationResult.ErrorCode,
						(adoperationResult.Exception == null) ? "isNull" : adoperationResult.Exception.ToString()
					});
					return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionUnresolvedCalendarEmailAddress);
				}
			}
			ExTraceGlobals.CreateCalendarCallTracer.TraceDebug<string, Guid, string>((long)this.GetHashCode(), "Creating calendar with name: '{0}', ParentGroupId: '{1}' EmailAddress:'{2}'", this.newCalendarName, guid, (this.emailAddress == null) ? "Local calendar" : this.emailAddress);
			MailboxSession mailboxSession = base.MailboxSession;
			CalendarActionFolderIdResponse result;
			using (CalendarGroup calendarGroup = CalendarGroup.Bind(mailboxSession, guid))
			{
				ExTraceGlobals.CreateCalendarCallTracer.TraceDebug((long)this.GetHashCode(), "Sucessfully bound to parent group. Creating calendar.");
				if (flag)
				{
					result = this.CreateLinkedCalendar(calendarGroup, sharedUser);
				}
				else
				{
					result = this.CreateLocalCalendar(calendarGroup);
				}
			}
			return result;
		}

		private CalendarActionFolderIdResponse CreateLinkedCalendar(CalendarGroup group, ADRecipient sharedUser)
		{
			MailboxSession mailboxSession = base.MailboxSession;
			CalendarActionFolderIdResponse result;
			using (CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(mailboxSession, sharedUser.LegacyExchangeDN, group))
			{
				calendarGroupEntry.CalendarName = this.newCalendarName;
				ConflictResolutionResult conflictResolutionResult = calendarGroupEntry.Save(SaveMode.NoConflictResolution);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					ExTraceGlobals.CreateCalendarCallTracer.TraceError<string, string, string>((long)this.GetHashCode(), "Could not create Calendar group entry for shared calendar. CalendarName: '{0}', EmailAddress: '{1}' LegDN:'{2}'", this.newCalendarName, this.emailAddress, sharedUser.LegacyExchangeDN);
					result = new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionUnableToCreateCalendarNode);
				}
				else
				{
					calendarGroupEntry.Load();
					result = new CalendarActionFolderIdResponse(null, IdConverter.ConvertStoreItemIdToItemId(calendarGroupEntry.Id, base.MailboxSession));
				}
			}
			return result;
		}

		private CalendarActionFolderIdResponse CreateLocalCalendar(CalendarGroup group)
		{
			MailboxSession mailboxSession = base.MailboxSession;
			Microsoft.Exchange.Services.Core.Types.ItemId calendarEntryId = null;
			StoreObjectId objectId;
			FolderId folderIdFromStoreId;
			byte[] calendarRecordKey;
			using (CalendarFolder calendarFolder = CalendarFolder.Create(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar), StoreObjectType.CalendarFolder, this.newCalendarName, CreateMode.CreateNew))
			{
				try
				{
					calendarFolder[FolderSchema.IsHidden] = false;
					FolderSaveResult folderSaveResult = calendarFolder.Save();
					if (folderSaveResult.OperationResult != OperationResult.Succeeded)
					{
						ExTraceGlobals.CreateCalendarCallTracer.TraceError<string, Guid>((long)this.GetHashCode(), "Unable to create new calendar folder. Name: '{0}', ParentGroupId: '{1}'", this.newCalendarName, group.GroupClassId);
						return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionUnableToCreateCalendarFolder);
					}
				}
				catch (ObjectExistedException)
				{
					return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionCalendarAlreadyExists);
				}
				ExTraceGlobals.CreateCalendarCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Successfully created calendar with Name: '{0}'.", this.newCalendarName);
				calendarFolder.Load();
				objectId = calendarFolder.Id.ObjectId;
				folderIdFromStoreId = IdConverter.GetFolderIdFromStoreId(calendarFolder.Id, new MailboxId(mailboxSession));
				calendarRecordKey = (calendarFolder[StoreObjectSchema.RecordKey] as byte[]);
			}
			ExTraceGlobals.CreateCalendarCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Creating Calendar group entry for calendar. Adding to calendar group. CalendarId: '{0}'", (objectId == null) ? "is null" : objectId.ToBase64String());
			using (CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(mailboxSession, objectId, group))
			{
				calendarGroupEntry.CalendarName = this.newCalendarName;
				calendarGroupEntry.CalendarRecordKey = calendarRecordKey;
				ConflictResolutionResult conflictResolutionResult = calendarGroupEntry.Save(SaveMode.NoConflictResolution);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					ExTraceGlobals.CreateCalendarCallTracer.TraceError<string, string>((long)this.GetHashCode(), "Could not create Calendar group entry. CalendarName: '{0}', CalendarId: '{1}'", this.newCalendarName, (objectId == null) ? "is null" : objectId.ToBase64String());
					AggregateOperationResult aggregateOperationResult = mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
					{
						objectId
					});
					if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
					{
						ExTraceGlobals.CreateCalendarCallTracer.TraceError<string, string>((long)this.GetHashCode(), "Could not delete created calendar after creation of calendar group entry failed. CalendarName: '{0}', CalendarId: '{1}'", this.newCalendarName, (objectId == null) ? "is null" : objectId.ToBase64String());
					}
					return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionUnableToCreateCalendarNode);
				}
				calendarGroupEntry.Load();
				calendarEntryId = IdConverter.ConvertStoreItemIdToItemId(calendarGroupEntry.Id, base.MailboxSession);
			}
			return new CalendarActionFolderIdResponse(folderIdFromStoreId, calendarEntryId);
		}

		private readonly string newCalendarName;

		private readonly string parentGroupGuid;

		private readonly string emailAddress;

		private readonly IRecipientSession adRecipientSession;
	}
}
