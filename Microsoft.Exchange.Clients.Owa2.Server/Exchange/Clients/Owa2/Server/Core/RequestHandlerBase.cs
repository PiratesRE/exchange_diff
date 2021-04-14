using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class RequestHandlerBase
	{
		internal abstract void OnPostAuthorizeRequest(object sender, EventArgs e);

		internal abstract void OnPreRequestHandlerExecute(object sender, EventArgs e);

		internal abstract void OnEndRequest(object sender, EventArgs e);

		internal abstract void OnPreSendRequestHeaders(object sender, EventArgs e);
	}
}
