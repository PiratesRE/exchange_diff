using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class RenameCalendarCommand : ServiceCommand<CalendarActionFolderIdResponse>
	{
		public RenameCalendarCommand(CallContext callContext, Microsoft.Exchange.Services.Core.Types.ItemId itemId, string newCalendarName) : base(callContext)
		{
			this.itemId = itemId;
			this.newCalendarName = newCalendarName;
		}

		protected override CalendarActionFolderIdResponse InternalExecute()
		{
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			if (this.itemId == null || string.IsNullOrEmpty(this.itemId.Id) || string.IsNullOrEmpty(this.itemId.ChangeKey))
			{
				ExTraceGlobals.RenameCalendarCallTracer.TraceError<string, string>((long)this.GetHashCode(), "ItemId provided is invalid. ItemId.Id: '{0}' ItemId.ChangeKey: '{1}'", (this.itemId == null || this.itemId.Id == null) ? "is null" : this.itemId.Id, (this.itemId == null || this.itemId.ChangeKey == null) ? "is null" : this.itemId.ChangeKey);
				return new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionInvalidItemId);
			}
			CalendarActionFolderIdResponse result;
			try
			{
				IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadWrite(this.itemId);
				StoreId id = idAndSession.Id;
				result = new RenameCalendar(base.MailboxIdentityMailboxSession, id, this.newCalendarName).Execute();
			}
			catch (StoragePermanentException ex)
			{
				ExTraceGlobals.RenameCalendarCallTracer.TraceError((long)this.GetHashCode(), "StoragePermanentException thrown while trying to rename calendar. NewName: {0}. ItemId.Id: {1}, ItemId.ChangeKey: {2} ExceptionInfo: {3}. CallStack: {4}", new object[]
				{
					(this.newCalendarName == null) ? "is null" : this.newCalendarName,
					this.itemId.Id,
					this.itemId.ChangeKey,
					ex.Message,
					ex.StackTrace
				});
				result = new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionUnableToRenameCalendar);
			}
			catch (StorageTransientException ex2)
			{
				ExTraceGlobals.RenameCalendarCallTracer.TraceError((long)this.GetHashCode(), "StorageTransientException thrown while trying to rename calendar. NewName: {0}. ItemId.Id: {1}, ItemId.ChangeKey: {2} ExceptionInfo: {3}. CallStack: {4}", new object[]
				{
					(this.newCalendarName == null) ? "is null" : this.newCalendarName,
					this.itemId.Id,
					this.itemId.ChangeKey,
					ex2.Message,
					ex2.StackTrace
				});
				result = new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionUnableToRenameCalendar);
			}
			return result;
		}

		private readonly Microsoft.Exchange.Services.Core.Types.ItemId itemId;

		private readonly string newCalendarName;
	}
}
