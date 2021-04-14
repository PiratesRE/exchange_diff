using System;
using System.Net;

namespace Microsoft.Exchange.Net.XropService
{
	internal interface IClientDiagnosticsHandler
	{
		object BeforeSendRequest(WebHeaderCollection transportRequestHeaders, string soapRequest);

		void AfterRecieveReply(WebHeaderCollection transportResponseHeaders, HttpStatusCode httpStatusCode, bool isfault, string soapResponse, object correlationState);
	}
}
