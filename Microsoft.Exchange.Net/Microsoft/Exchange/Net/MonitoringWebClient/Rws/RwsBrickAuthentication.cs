using System;
using System.Net;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Rws
{
	internal class RwsBrickAuthentication : BaseTestStep
	{
		public CommonAccessToken Token { get; private set; }

		public Uri Uri { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.RwsBrickAuthentication;
			}
		}

		public RwsBrickAuthentication(CommonAccessToken token, Uri uri)
		{
			this.Token = token;
			this.Uri = uri;
		}

		protected override void StartTest()
		{
			this.session.AuthenticationData = new AuthenticationData?(new AuthenticationData
			{
				UseDefaultCredentials = false,
				Credentials = CredentialCache.DefaultNetworkCredentials.GetCredential(this.Uri, "Kerberos")
			});
			this.session.PersistentHeaders.Add("X-CommonAccessToken", this.Token.Serialize());
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.RwsBrickAuthentication;
	}
}
