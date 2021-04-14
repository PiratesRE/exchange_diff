using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.HttpProxy
{
	internal class SpeechRecoProxyRequestHandler : BEServerCookieProxyRequestHandler<WebServicesService>
	{
		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.InternalNLBBypass;
			}
		}
	}
}
