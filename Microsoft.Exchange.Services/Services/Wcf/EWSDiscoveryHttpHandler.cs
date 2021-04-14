using System;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Wcf
{
	public class EWSDiscoveryHttpHandler : IHttpHandler
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
			if (!request.IsAuthenticated)
			{
				response.StatusCode = 401;
				return;
			}
			if (request.HttpMethod == "GET")
			{
				response.Redirect("/ews/Services.wsdl");
				return;
			}
			if (request.HttpMethod == "HEAD")
			{
				response.StatusCode = 302;
				response.RedirectLocation = "/ews/Services.wsdl";
				return;
			}
			response.StatusCode = 400;
		}

		protected void AddEndpointEnabledHeaders(HttpResponse response)
		{
			response.AddHeader("X-WSSecurity-Enabled", EWSSettings.IsWsSecurityEndpointEnabled ? bool.TrueString : bool.FalseString);
			response.AddHeader("X-WSSecurity-For", (VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.WsSecurityEndpoint.Enabled && EWSSettings.IsWsSecurityEndpointEnabled) ? "Logon" : "None");
			FederationTrust federationTrust = FederationTrustCache.GetFederationTrust("MicrosoftOnline");
			if (federationTrust != null)
			{
				response.AddHeader("X-FederationTrustTokenIssuerUri", federationTrust.TokenIssuerUri.ToString());
			}
			if (EWSSettings.IsWsSecuritySymmetricKeyEndpointEnabled)
			{
				response.AddHeader("X-WSSecurity-SymmetricKey-Enabled", bool.TrueString);
			}
			if (EWSSettings.IsWsSecurityX509CertEndpointEnabled)
			{
				response.AddHeader("X-WSSecurity-X509Cert-Enabled", bool.TrueString);
			}
			response.AddHeader("X-OAuth-Enabled", EWSSettings.IsOAuthEndpointEnabled ? bool.TrueString : bool.FalseString);
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		private const string OAuthEnabledHeaderName = "X-OAuth-Enabled";

		private const string WsSecurityEnabledHeaderName = "X-WSSecurity-Enabled";

		private const string WsSecuritySymmetricKeyEnabledHeaderName = "X-WSSecurity-SymmetricKey-Enabled";

		private const string WsSecurityX509CertEnabledHeaderName = "X-WSSecurity-X509Cert-Enabled";

		private const string WsSecurityForHeaderName = "X-WSSecurity-For";

		private const string FederationTrustTokenIssuerUriHeaderName = "X-FederationTrustTokenIssuerUri";

		private const string NoneHeaderValue = "None";

		private const string LogonHeaderValue = "Logon";

		private const string RedirectLocation = "/ews/Services.wsdl";
	}
}
