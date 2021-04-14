using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class RenameCalendarGroupCommand : ServiceCommand<CalendarActionGroupIdResponse>
	{
		public RenameCalendarGroupCommand(CallContext callContext, Microsoft.Exchange.Services.Core.Types.ItemId groupId, string newGroupName) : base(callContext)
		{
			this.newGroupName = newGroupName;
			this.groupId = groupId;
		}

		protected override CalendarActionGroupIdResponse InternalExecute()
		{
			if (this.groupId == null || string.IsNullOrEmpty(this.groupId.Id) || string.IsNullOrEmpty(this.groupId.ChangeKey))
			{
				ExTraceGlobals.RenameCalendarCallTracer.TraceError<string, string>((long)this.GetHashCode(), "ItemId provided is invalid. ItemId.Id: '{0}' ItemId.ChangeKey: '{1}'", (this.groupId == null || this.groupId.Id == null) ? "is null" : this.groupId.Id, (this.groupId == null || this.groupId.ChangeKey == null) ? "is null" : this.groupId.ChangeKey);
				return new CalendarActionGroupIdResponse(CalendarActionError.CalendarActionInvalidItemId);
			}
			CalendarActionGroupIdResponse result;
			try
			{
				string id = this.groupId.Id;
				MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
				ExTraceGlobals.RenameCalendarGroupCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Attempting to rename group. GroupId: '{0}', NewName: '{1}'", id, this.newGroupName);
				EntitiesHelper entitiesHelper = new EntitiesHelper(base.CallContext);
				ICalendarGroups calendarGroups = entitiesHelper.GetCalendarGroups(mailboxIdentityMailboxSession);
				Microsoft.Exchange.Entities.DataModel.Calendaring.CalendarGroup calendarGroup = new Microsoft.Exchange.Entities.DataModel.Calendaring.CalendarGroup
				{
					Id = id,
					Name = this.newGroupName
				};
				calendarGroup = entitiesHelper.Execute<Microsoft.Exchange.Entities.DataModel.Calendaring.CalendarGroup, Microsoft.Exchange.Entities.DataModel.Calendaring.CalendarGroup>(new Func<Microsoft.Exchange.Entities.DataModel.Calendaring.CalendarGroup, CommandContext, Microsoft.Exchange.Entities.DataModel.Calendaring.CalendarGroup>(calendarGroups.Update<Microsoft.Exchange.Entities.DataModel.Calendaring.CalendarGroup>), mailboxIdentityMailboxSession, BasicTypes.Item, calendarGroup);
				ExTraceGlobals.RenameCalendarGroupCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Group name updated. GroupId: '{0}', NewName: '{1}'", id, this.newGroupName);
				Microsoft.Exchange.Services.Core.Types.ItemId groupItemId = IdConverter.ConvertStoreItemIdToItemId(calendarGroup.StoreId, mailboxIdentityMailboxSession);
				result = new CalendarActionGroupIdResponse(calendarGroup.ClassId, groupItemId);
			}
			catch (InvalidCalendarGroupNameException ex)
			{
				ExTraceGlobals.RenameCalendarGroupCallTracer.TraceError((long)this.GetHashCode(), "The supplied new group name is not valid. GroupId: {0}, NewGroupName: {1}", new object[]
				{
					this.groupId.Id,
					(this.newGroupName == null) ? "is null" : this.newGroupName,
					ex.Message,
					ex.StackTrace
				});
				result = new CalendarActionGroupIdResponse(CalendarActionError.CalendarActionInvalidGroupName);
			}
			catch (StoragePermanentException ex2)
			{
				ExTraceGlobals.RenameCalendarGroupCallTracer.TraceError((long)this.GetHashCode(), "StoragePermanentException thrown while trying to rename calendar group. GroupName: {0}. ItemId.Id: {1}, ItemId.ChangeKey: {2} ExceptionInfo: {3}. CallStack: {4}", new object[]
				{
					(this.newGroupName == null) ? "is null" : this.newGroupName,
					this.groupId.Id,
					this.groupId.ChangeKey,
					ex2.Message,
					ex2.StackTrace
				});
				result = new CalendarActionGroupIdResponse(CalendarActionError.CalendarActionUnableToRenameCalendarGroup);
			}
			catch (StorageTransientException ex3)
			{
				ExTraceGlobals.RenameCalendarGroupCallTracer.TraceError((long)this.GetHashCode(), "StorageTransientException thrown while trying to rename calendar group. GroupName: {0}. ItemId.Id: {1}, ItemId.ChangeKey: {2} ExceptionInfo: {3}. CallStack: {4}", new object[]
				{
					(this.newGroupName == null) ? "is null" : this.newGroupName,
					this.groupId.Id,
					this.groupId.ChangeKey,
					ex3.Message,
					ex3.StackTrace
				});
				result = new CalendarActionGroupIdResponse(CalendarActionError.CalendarActionUnableToRenameCalendarGroup);
			}
			return result;
		}

		private readonly Microsoft.Exchange.Services.Core.Types.ItemId groupId;

		private readonly string newGroupName;
	}
}
