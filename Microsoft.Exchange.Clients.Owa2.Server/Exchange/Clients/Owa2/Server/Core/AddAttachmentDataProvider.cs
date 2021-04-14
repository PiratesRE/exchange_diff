using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AddAttachmentDataProvider : ServiceCommand<AttachmentDataProvider>
	{
		public AddAttachmentDataProvider(CallContext callContext, AttachmentDataProvider attachmentDataProvider) : base(callContext)
		{
			if (attachmentDataProvider == null)
			{
				throw new ArgumentNullException("attachmentDataProvider");
			}
			this.attachmentDataProvider = attachmentDataProvider;
		}

		protected override AttachmentDataProvider InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			return userContext.AttachmentDataProviderManager.AddProvider(base.CallContext, this.attachmentDataProvider);
		}

		private AttachmentDataProvider attachmentDataProvider;
	}
}
