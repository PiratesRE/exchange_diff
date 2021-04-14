using System;
using System.Net;
using System.Net.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.OnlineMeetings.Autodiscover;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class OAuthRequestFactory : UcwaRequestFactory
	{
		static OAuthRequestFactory()
		{
			CertificateValidationManager.RegisterCallback("OnlineMeeting", new RemoteCertificateValidationCallback(AutodiscoverWorker.ServerCertificateValidator));
		}

		public OAuthRequestFactory(OAuthCredentials oauthCredentials)
		{
			this.oauthCredentials = oauthCredentials;
		}

		public override string LandingPageToken
		{
			get
			{
				return "oauth";
			}
		}

		internal static string UserAgent
		{
			get
			{
				return string.Format("Exchange/{0}/OnlineMeeting", OAuthUtilities.ServerVersionString);
			}
		}

		protected override HttpWebRequest CreateHttpWebRequest(string httpMethod, string url)
		{
			HttpWebRequest httpWebRequest = base.CreateHttpWebRequest(httpMethod, url);
			httpWebRequest.Credentials = this.oauthCredentials;
			httpWebRequest.UserAgent = OAuthRequestFactory.UserAgent;
			if (this.oauthCredentials.ClientRequestId != null)
			{
				httpWebRequest.Headers.Add("client-request-id", this.oauthCredentials.ClientRequestId.Value.ToString());
				httpWebRequest.Headers.Add("return-client-request-id", bool.TrueString);
			}
			CertificateValidationManager.SetComponentId(httpWebRequest, "OnlineMeeting");
			return httpWebRequest;
		}

		internal const string CertificateValidationComponentId = "OnlineMeeting";

		private const string UserAgentFormat = "Exchange/{0}/OnlineMeeting";

		private readonly OAuthCredentials oauthCredentials;
	}
}
