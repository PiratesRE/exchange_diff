using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetAttachmentDataProviderTypes : ServiceCommand<AttachmentDataProviderType>
	{
		public GetAttachmentDataProviderTypes(CallContext callContext) : base(callContext)
		{
		}

		protected override AttachmentDataProviderType InternalExecute()
		{
			return AttachmentDataProviderType.OneDrivePro;
		}
	}
}
