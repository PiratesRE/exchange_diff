using System;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetCalendarSharingRecipientInfoCommand : ServiceCommand<GetCalendarSharingRecipientInfoResponse>
	{
		public GetCalendarSharingRecipientInfoCommand(CallContext callContext, GetCalendarSharingRecipientInfoRequest request) : base(callContext)
		{
			this.Request = request;
		}

		private GetCalendarSharingRecipientInfoRequest Request { get; set; }

		protected override GetCalendarSharingRecipientInfoResponse InternalExecute()
		{
			this.TraceDebug("Validating Request", new object[0]);
			this.Request.ValidateRequest(base.CallContext.ADRecipientSessionContext);
			return new GetCalendarSharingRecipientInfo(base.CallContext.SessionCache.GetMailboxIdentityMailboxSession(), this.Request, base.CallContext.AccessingPrincipal).Execute();
		}

		private void TraceDebug(string messageFormat, params object[] args)
		{
			ExTraceGlobals.GetCalendarSharingRecipientInfoCallTracer.TraceDebug((long)this.GetHashCode(), messageFormat, args);
		}
	}
}
