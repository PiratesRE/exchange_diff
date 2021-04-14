using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetAttachmentDataProviders : ServiceCommand<AttachmentDataProvider[]>
	{
		public GetAttachmentDataProviders(CallContext callContext, GetAttachmentDataProvidersRequest request) : base(callContext)
		{
			this.request = request;
		}

		protected override AttachmentDataProvider[] InternalExecute()
		{
			return UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true).AttachmentDataProviderManager.GetProviders(base.CallContext, this.request);
		}

		private readonly GetAttachmentDataProvidersRequest request;
	}
}
