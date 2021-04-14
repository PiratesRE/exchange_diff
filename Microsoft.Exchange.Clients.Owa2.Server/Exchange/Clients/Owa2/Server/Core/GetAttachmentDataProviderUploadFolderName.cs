using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetAttachmentDataProviderUploadFolderName : ServiceCommand<string>
	{
		public GetAttachmentDataProviderUploadFolderName(CallContext callContext) : base(callContext)
		{
		}

		protected override string InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			AttachmentDataProvider defaultUploadDataProvider = userContext.AttachmentDataProviderManager.GetDefaultUploadDataProvider(base.CallContext);
			if (defaultUploadDataProvider is OneDriveProAttachmentDataProvider)
			{
				return ((OneDriveProAttachmentDataProvider)defaultUploadDataProvider).GetUploadFolderName(userContext);
			}
			return null;
		}
	}
}
