using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class CreateCalendarCommand : ServiceCommand<CalendarActionFolderIdResponse>
	{
		public CreateCalendarCommand(CallContext callContext, string newCalendarName, string parentGroupGuid, string emailAddress) : base(callContext)
		{
			this.newCalendarName = newCalendarName;
			this.parentGroupGuid = parentGroupGuid;
			this.emailAddress = emailAddress;
		}

		protected override CalendarActionFolderIdResponse InternalExecute()
		{
			CalendarActionFolderIdResponse result;
			try
			{
				result = new CreateCalendar(base.MailboxIdentityMailboxSession, this.newCalendarName, this.parentGroupGuid, this.emailAddress, base.CallContext.ADRecipientSessionContext.GetADRecipientSession()).Execute();
			}
			catch (StoragePermanentException ex)
			{
				ExTraceGlobals.CreateCalendarCallTracer.TraceError((long)this.GetHashCode(), "StoragePermanentException thrown while trying to create calendar with name: {0}. ParentGroupGuid: {1}, ExceptionInfo: {2}. CallStack: {3}", new object[]
				{
					(this.newCalendarName == null) ? "is null" : this.newCalendarName,
					(this.parentGroupGuid == null) ? "is null" : this.parentGroupGuid,
					ex.Message,
					ex.StackTrace
				});
				result = new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionUnableToCreateCalendarFolder);
			}
			catch (StorageTransientException ex2)
			{
				ExTraceGlobals.CreateCalendarCallTracer.TraceError((long)this.GetHashCode(), "StorageTransientException thrown while trying to create calendar with name: {0}. ParentGroupGuid: {1}, ExceptionInfo: {2}. CallStack: {3}", new object[]
				{
					(this.newCalendarName == null) ? "is null" : this.newCalendarName,
					(this.parentGroupGuid == null) ? "is null" : this.parentGroupGuid,
					ex2.Message,
					ex2.StackTrace
				});
				result = new CalendarActionFolderIdResponse(CalendarActionError.CalendarActionUnableToCreateCalendarFolder);
			}
			return result;
		}

		private readonly string newCalendarName;

		private readonly string parentGroupGuid;

		private readonly string emailAddress;
	}
}
