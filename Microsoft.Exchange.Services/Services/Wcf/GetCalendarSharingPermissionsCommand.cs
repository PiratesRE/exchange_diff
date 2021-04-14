using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetCalendarSharingPermissionsCommand : ServiceCommand<GetCalendarSharingPermissionsResponse>
	{
		public GetCalendarSharingPermissionsCommand(CallContext callContext, GetCalendarSharingPermissionsRequest request) : base(callContext)
		{
			this.Request = request;
		}

		private GetCalendarSharingPermissionsRequest Request { get; set; }

		protected override GetCalendarSharingPermissionsResponse InternalExecute()
		{
			GetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), "Validating Request", new object[0]);
			this.Request.ValidateRequest();
			return new GetCalendarSharingPermissions(base.CallContext.SessionCache.GetMailboxIdentityMailboxSession(), this.Request.CalendarStoreId, base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId, base.CallContext.ADRecipientSessionContext).Execute();
		}

		private const string AnonymousDomainName = "Anonymous";
	}
}
