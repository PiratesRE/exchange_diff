using System;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public class AutodiscoverDiscoveryHttpHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			this.GenerateResponse(context);
		}

		internal virtual void GenerateResponse(HttpContext context)
		{
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			this.AddEndpointEnabledHeaders(context.Response);
			if (request.IsAuthenticated)
			{
				response.Redirect("./Services.wsdl");
				return;
			}
			response.StatusCode = 401;
		}

		protected void AddEndpointEnabledHeaders(HttpResponse response)
		{
			if (AutodiscoverDiscoveryHttpHandler.webConfiguration.Member.SoapEndpointEnabled)
			{
				response.AddHeader("X-SOAP-Enabled", bool.TrueString);
			}
			response.AddHeader("X-WSSecurity-Enabled", AutodiscoverDiscoveryHttpHandler.webConfiguration.Member.WsSecurityEndpointEnabled ? bool.TrueString : bool.FalseString);
			response.AddHeader("X-WSSecurity-For", (VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.WsSecurityEndpoint.Enabled && AutodiscoverDiscoveryHttpHandler.webConfiguration.Member.WsSecurityEndpointEnabled) ? "Logon" : "None");
			FederationTrust federationTrust = FederationTrustCache.GetFederationTrust("MicrosoftOnline");
			if (federationTrust != null)
			{
				response.AddHeader("X-FederationTrustTokenIssuerUri", federationTrust.TokenIssuerUri.ToString());
			}
			if (AutodiscoverDiscoveryHttpHandler.webConfiguration.Member.WsSecuritySymmetricKeyEndpointEnabled)
			{
				response.AddHeader("X-WSSecurity-SymmetricKey-Enabled", bool.TrueString);
			}
			if (AutodiscoverDiscoveryHttpHandler.webConfiguration.Member.WsSecurityX509CertEndpointEnabled)
			{
				response.AddHeader("X-WSSecurity-X509Cert-Enabled", bool.TrueString);
			}
			response.AddHeader("X-OAuth-Enabled", AutodiscoverDiscoveryHttpHandler.webConfiguration.Member.OAuthEndpointEnabled ? bool.TrueString : bool.FalseString);
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
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

		private static LazyMember<AutodiscoverWebConfiguration> webConfiguration = new LazyMember<AutodiscoverWebConfiguration>(() => new AutodiscoverWebConfiguration());
	}
}
