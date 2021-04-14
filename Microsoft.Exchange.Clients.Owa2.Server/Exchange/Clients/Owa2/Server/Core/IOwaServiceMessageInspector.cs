using System;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public interface IOwaServiceMessageInspector
	{
		void AfterReceiveRequest(HttpRequest httpRequest, string methodName, object request);

		void BeforeSendReply(HttpResponse httpResponse, string methodName, object response);
	}
}
