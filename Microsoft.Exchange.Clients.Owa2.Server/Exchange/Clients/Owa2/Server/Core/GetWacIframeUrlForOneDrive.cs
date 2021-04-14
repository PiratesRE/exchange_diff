using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetWacIframeUrlForOneDrive : ServiceCommand<string>
	{
		public GetWacIframeUrlForOneDrive(CallContext callContext, GetWacIframeUrlForOneDriveRequest request) : base(callContext)
		{
			throw new OwaInvalidRequestException();
		}

		protected override string InternalExecute()
		{
			throw new OwaInvalidRequestException();
		}
	}
}
