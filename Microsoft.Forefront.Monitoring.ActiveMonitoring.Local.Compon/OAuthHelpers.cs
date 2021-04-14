using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Hygiene.Deployment.Common;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Security.OAuth.OAuthProtocols;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class OAuthHelpers
	{
		internal static OAuthTokenRequest RequestTokenFromAcs(IHygieneLogger logger, string acsUrl, X509Certificate2 acsTenantCertificate, string resourceHost, string resourcePartnerId, string tenantId)
		{
			logger.LogMessage("FAM.OAuthHelpers.RequestTokenFromAcs()");
			OAuthTokenRequest oauthTokenRequest = new OAuthTokenRequest();
			oauthTokenRequest.AcsUrl = acsUrl;
			oauthTokenRequest.Audience = string.Format("{0}/{1}@{2}", WellknownAuthServerIssuerNames.MicrosoftSts, new Uri(acsUrl).Authority, tenantId);
			oauthTokenRequest.Issuer = string.Format("{0}@{1}", resourcePartnerId, tenantId);
			oauthTokenRequest.Resource = string.Format("{0}/{1}@{2}", resourcePartnerId, resourceHost, tenantId);
			logger.LogVerbose(string.Format("acsUrl: '{0}'", acsUrl));
			logger.LogVerbose(string.Format("acsTenantCertificate.Subject: '{0}'", acsTenantCertificate.Subject));
			logger.LogVerbose(string.Format("audience: '{0}'", oauthTokenRequest.Audience));
			logger.LogVerbose(string.Format("issuer: '{0}'", oauthTokenRequest.Issuer));
			logger.LogVerbose(string.Format("resource: '{0}'", oauthTokenRequest.Resource));
			oauthTokenRequest.JwtInputToken = new JwtSecurityToken(oauthTokenRequest.Issuer, oauthTokenRequest.Audience, new Claim[0], new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddDays(2.0)), new X509SigningCredentials(acsTenantCertificate, "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", "http://www.w3.org/2001/04/xmlenc#sha256"));
			logger.LogVerbose(string.Format("jwtToken: '{0}'", oauthTokenRequest.JwtInputToken.ToString()));
			oauthTokenRequest.AcsTokenRequest = OAuth2MessageFactory.CreateAccessTokenRequestWithAssertion(oauthTokenRequest.JwtInputToken, oauthTokenRequest.Resource);
			logger.LogVerbose(string.Format("AcsTokenRequest: '{0}'", oauthTokenRequest.AcsTokenRequest.ToString()));
			string text = oauthTokenRequest.AcsTokenRequest.ToString();
			WebRequest webRequest = WebRequest.Create(acsUrl);
			webRequest.AuthenticationLevel = AuthenticationLevel.None;
			webRequest.ContentLength = (long)text.Length;
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";
			webRequest.Timeout = 30000;
			webRequest.Headers["client-request-id"] = Guid.NewGuid().ToString();
			webRequest.Headers["return-client-request-id"] = "true";
			oauthTokenRequest.AcsNetResponse = NetHelpers.DoWebRequest(logger, webRequest, text);
			string content = oauthTokenRequest.AcsNetResponse.Content;
			logger.LogVerbose(string.Format("responseBody:{0}", content));
			if (!string.IsNullOrEmpty(content))
			{
				using (MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(content)))
				{
					using (StreamReader streamReader = new StreamReader(memoryStream))
					{
						oauthTokenRequest.AcsTokenResponse = (OAuth2MessageFactory.CreateFromEncodedResponse(streamReader) as OAuth2AccessTokenResponse);
						if (oauthTokenRequest.AcsTokenResponse != null)
						{
							TokenResult tokenResult = new TokenResult(oauthTokenRequest.AcsTokenResponse);
							oauthTokenRequest.AcsTokenResultString = tokenResult.TokenString;
						}
					}
				}
			}
			logger.LogVerbose(string.Format("req.AcsTokenResultString: '{0}'", oauthTokenRequest.AcsTokenResultString));
			return oauthTokenRequest;
		}
	}
}
