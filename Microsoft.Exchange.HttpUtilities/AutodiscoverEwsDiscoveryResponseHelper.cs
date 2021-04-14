using System;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpUtilities
{
	internal static class AutodiscoverEwsDiscoveryResponseHelper
	{
		internal static void AddEndpointEnabledHeaders(HttpResponse response)
		{
			if (AutodiscoverEwsWebConfiguration.SoapEndpointEnabled)
			{
				response.AddHeader("X-SOAP-Enabled", bool.TrueString);
			}
			response.AddHeader("X-WSSecurity-Enabled", AutodiscoverEwsWebConfiguration.WsSecurityEndpointEnabled ? bool.TrueString : bool.FalseString);
			string value;
			if ((HttpProxyGlobals.IsPartnerHostedOnly || VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.WsSecurityEndpoint.Enabled) && AutodiscoverEwsWebConfiguration.WsSecurityEndpointEnabled)
			{
				value = "Logon";
			}
			else
			{
				value = "None";
			}
			response.AddHeader("X-WSSecurity-For", value);
			FederationTrust federationTrust = FederationTrustCache.GetFederationTrust("MicrosoftOnline");
			if (federationTrust != null)
			{
				response.AddHeader("X-FederationTrustTokenIssuerUri", federationTrust.TokenIssuerUri.ToString());
			}
			if (AutodiscoverEwsWebConfiguration.WsSecuritySymmetricKeyEndpointEnabled)
			{
				response.AddHeader("X-WSSecurity-SymmetricKey-Enabled", bool.TrueString);
			}
			if (AutodiscoverEwsWebConfiguration.WsSecurityX509CertEndpointEnabled)
			{
				response.AddHeader("X-WSSecurity-X509Cert-Enabled", bool.TrueString);
			}
			HttpApplication applicationInstance = HttpContext.Current.ApplicationInstance;
			IHttpModule httpModule = applicationInstance.Modules["OAuthAuthModule"];
			response.AddHeader("X-OAuth-Enabled", (httpModule != null) ? bool.TrueString : bool.FalseString);
		}

		private const string SoapEnabledHeaderName = "X-SOAP-Enabled";

		private const string OAuthEnabledHeaderName = "X-OAuth-Enabled";

		private const string WsSecurityEnabledHeaderName = "X-WSSecurity-Enabled";

		private const string WsSecuritySymmetricKeyEnabledHeaderName = "X-WSSecurity-SymmetricKey-Enabled";

		private const string WsSecurityX509CertEnabledHeaderName = "X-WSSecurity-X509Cert-Enabled";

		private const string WsSecurityForHeaderName = "X-WSSecurity-For";

		private const string FederationTrustTokenIssuerUriHeaderName = "X-FederationTrustTokenIssuerUri";

		private const string NoneHeaderValue = "None";

		private const string LogonHeaderValue = "Logon";
	}
}
