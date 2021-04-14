using System;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SendCalendarSharingInviteCommand : ServiceCommand<CalendarShareInviteResponse>
	{
		public SendCalendarSharingInviteCommand(CallContext callContext, CalendarShareInviteRequest calendarShareInviteRequest) : base(callContext)
		{
			this.Request = calendarShareInviteRequest;
		}

		private CalendarShareInviteRequest Request { get; set; }

		protected override CalendarShareInviteResponse InternalExecute()
		{
			this.TraceDebug("Validating Request", new object[0]);
			this.Request.ValidateRequest(base.CallContext.SessionCache.GetMailboxIdentityMailboxSession(), base.CallContext.ADRecipientSessionContext);
			return new SendCalendarSharingInvite(base.CallContext.SessionCache.GetMailboxIdentityMailboxSession(), this.Request, base.CallContext.AccessingPrincipal, base.CallContext.ADRecipientSessionContext).Execute();
		}

		private void TraceDebug(string messageFormat, params object[] args)
		{
			ExTraceGlobals.GetCalendarSharingRecipientInfoCallTracer.TraceDebug((long)this.GetHashCode(), messageFormat, args);
		}
	}
}
