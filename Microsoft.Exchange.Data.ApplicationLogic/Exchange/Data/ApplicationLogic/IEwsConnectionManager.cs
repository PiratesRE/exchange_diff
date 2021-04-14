using System;
using System.Net.Security;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal interface IEwsConnectionManager
	{
		string GetSmtpAddress();

		string GetPrincipalInfoForTracing();

		void ReloadPrincipal();

		Uri GetBackEndWebServicesUrl();

		IExchangeService CreateBinding(RemoteCertificateValidationCallback certificateErrorHandler);
	}
}
