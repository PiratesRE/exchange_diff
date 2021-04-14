using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class BrickAuthentication : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public string UserName { get; private set; }

		public string UserDomain { get; private set; }

		public AuthenticationParameters AuthenticationParameters { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.BrickAuthentication;
			}
		}

		public BrickAuthentication(Uri uri, string userName, string userDomain, AuthenticationParameters authenticationParameters)
		{
			this.Uri = uri;
			this.UserName = userName;
			this.UserDomain = userDomain;
			this.AuthenticationParameters = authenticationParameters;
		}

		protected override void StartTest()
		{
			if (this.AuthenticationParameters == null)
			{
				throw new ArgumentException("Authentication parameters cannot be null. Check if the parameter was created properly");
			}
			CommonAccessToken commonAccessToken = this.AuthenticationParameters.CommonAccessToken;
			this.session.PersistentHeaders.Add("X-CommonAccessToken", commonAccessToken.Serialize());
			this.session.AuthenticationData = new AuthenticationData?(new AuthenticationData
			{
				UseDefaultCredentials = true,
				Credentials = CredentialCache.DefaultNetworkCredentials.GetCredential(this.Uri, "Kerberos")
			});
			Uri uri = new Uri(this.Uri, "/owa/proxylogon.owa");
			RequestBody body = RequestBody.Format(commonAccessToken.Serialize(), new object[0]);
			this.session.BeginPost(this.Id, uri.ToString(), body, null, delegate(IAsyncResult resultObj)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.BrickCscPostResponseReceived), resultObj);
			}, null);
		}

		private void BrickCscPostResponseReceived(IAsyncResult result)
		{
			this.session.EndPost<HttpStatusCode>(result, new HttpStatusCode[]
			{
				(HttpStatusCode)241,
				HttpStatusCode.Found
			}, delegate(HttpWebResponseWrapper response)
			{
				if (response.StatusCode == HttpStatusCode.Found && !OwaLanguageSelectionPage.ContainsLanguagePageRedirection(response))
				{
					throw new MissingKeywordException(MonitoringWebClientStrings.BrickAuthenticationMissingOkOrLanguageSelection, response.Request, response, HttpStatusCode.Found.ToString());
				}
				if (response.Headers["Set-Cookie"] == null || (response.Headers["Set-Cookie"].IndexOf("UserContext", StringComparison.OrdinalIgnoreCase) < 0 && response.Headers["Set-Cookie"].IndexOf("UC", StringComparison.OrdinalIgnoreCase) < 0))
				{
					throw new MissingKeywordException(MonitoringWebClientStrings.MissingUserContextCookie, response.Request, response, "User Context");
				}
				return response.StatusCode;
			});
			base.ExecutionCompletedSuccessfully();
		}

		private byte[] GetSecurityContext(ClientSecurityContext clientSecurityContext)
		{
			SerializedSecurityAccessToken serializedSecurityAccessToken = new SerializedSecurityAccessToken();
			using (ClientSecurityContext clientSecurityContext2 = clientSecurityContext.Clone())
			{
				clientSecurityContext2.SetSecurityAccessToken(serializedSecurityAccessToken);
			}
			if (serializedSecurityAccessToken.GroupSids != null)
			{
				SidStringAndAttributes[] groupSids = serializedSecurityAccessToken.GroupSids;
			}
			if (serializedSecurityAccessToken.RestrictedGroupSids != null)
			{
				SidStringAndAttributes[] restrictedGroupSids = serializedSecurityAccessToken.RestrictedGroupSids;
			}
			byte[] bytes = serializedSecurityAccessToken.GetBytes();
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					gzipStream.Write(bytes, 0, bytes.Length);
				}
				memoryStream.Flush();
				result = memoryStream.ToArray();
			}
			return result;
		}

		private const TestId ID = TestId.BrickAuthentication;

		private const string OWAProxyLogonURL = "/owa/proxylogon.owa";
	}
}
