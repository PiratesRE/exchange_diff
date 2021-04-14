using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetCalendarSharingPermissionsCommand : ServiceCommand<CalendarActionResponse>
	{
		public SetCalendarSharingPermissionsCommand(CallContext callContext, SetCalendarSharingPermissionsRequest request) : base(callContext)
		{
			this.Request = request;
		}

		private SetCalendarSharingPermissionsRequest Request { get; set; }

		protected override CalendarActionResponse InternalExecute()
		{
			SetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), "Validating Request", new object[0]);
			this.Request.ValidateRequest();
			return new SetCalendarSharingPermissions(base.CallContext.SessionCache.GetMailboxIdentityMailboxSession(), this.Request, base.CallContext.AccessingPrincipal.ObjectId, base.CallContext.ADRecipientSessionContext, this.Request.CalendarStoreId, base.CallContext.AccessingPrincipal).Execute();
		}
	}
}
