using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetAttachmentDataProvidersRecentItems : ServiceCommand<GetAttachmentDataProviderItemsResponse>
	{
		public GetAttachmentDataProvidersRecentItems(CallContext callContext) : base(callContext)
		{
		}

		protected override GetAttachmentDataProviderItemsResponse InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			return userContext.AttachmentDataProviderManager.GetRecentItems(base.CallContext);
		}
	}
}
