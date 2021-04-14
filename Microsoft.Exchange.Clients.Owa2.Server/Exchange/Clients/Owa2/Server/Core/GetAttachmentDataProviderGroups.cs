using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetAttachmentDataProviderGroups : ServiceCommand<GetAttachmentDataProviderItemsResponse>
	{
		public GetAttachmentDataProviderGroups(CallContext callContext) : base(callContext)
		{
		}

		protected override GetAttachmentDataProviderItemsResponse InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			return userContext.AttachmentDataProviderManager.GetGroups(base.CallContext, base.MailboxIdentityMailboxSession);
		}
	}
}
